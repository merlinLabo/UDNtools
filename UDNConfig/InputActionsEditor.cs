using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;

namespace UDNConfig
{

    public static class InputActionsEditor
    {

        static readonly string[] Anchors =
        {
            "{\n    \"name\": \"UDNInputAction\"",
            "{\"name\":\"UDNInputAction\"",
        };

        public class Region
        {
            public int ByteStart;    
            public int CharLength;   
            public string Text;      
            public string Json;     
        }

        public static Region Locate(byte[] dll)
        {
            int at = -1;
            foreach (string a in Anchors)
            {
                byte[] pat = Encoding.Unicode.GetBytes(a);
                int p = IndexOf(dll, pat, 0);
                if (p < 0) continue;
                if (IndexOf(dll, pat, p + 2) >= 0)
                    throw new InvalidDataException("The embedded UDNInputAction config appears more than once; aborting for safety.");
                at = p; break;
            }
            if (at < 0) throw new InvalidDataException(
                "Could not find the embedded UDNInputAction config in Konami.GameSystem.dll. The DLL version may differ.");

            int braceEnd = FindJsonEnd(dll, at);
            int totalBytes = ReadUserStringLength(dll, at, braceEnd - at);

            var r = new Region();
            r.ByteStart = at;
            r.CharLength = totalBytes / 2;
            r.Text = Encoding.Unicode.GetString(dll, at, totalBytes);
            r.Json = r.Text.TrimEnd(' ', '\t', '\r', '\n');
            return r;
        }

        static int FindJsonEnd(byte[] dll, int start)
        {
            int depth = 0; bool inStr = false, esc = false;
            int p = start;
            while (p + 1 < dll.Length)
            {
                char c = (char)(dll[p] | (dll[p + 1] << 8));
                if (inStr)
                {
                    if (esc) esc = false;
                    else if (c == '\\') esc = true;
                    else if (c == '"') inStr = false;
                }
                else
                {
                    if (c == '"') inStr = true;
                    else if (c == '{' || c == '[') depth++;
                    else if (c == '}' || c == ']')
                    {
                        depth--;
                        if (depth == 0) return p + 2;
                    }
                }
                p += 2;
            }
            throw new InvalidDataException("The embedded config's JSON structure is incomplete.");
        }

        static int ReadUserStringLength(byte[] d, int at, int minBytes)
        {
            // 4-byte prefix: 110xxxxx
            if (at >= 4 && (d[at - 4] & 0xE0) == 0xC0)
            {
                int n = ((d[at - 4] & 0x1F) << 24) | (d[at - 3] << 16) | (d[at - 2] << 8) | d[at - 1];
                if (Plausible(n, minBytes, d.Length - at)) return n - 1;
            }
            // 2-byte prefix: 10xxxxxx
            if (at >= 2 && (d[at - 2] & 0xC0) == 0x80)
            {
                int n = ((d[at - 2] & 0x3F) << 8) | d[at - 1];
                if (Plausible(n, minBytes, d.Length - at)) return n - 1;
            }
            // 1-byte prefix: 0xxxxxxx
            if (at >= 1 && (d[at - 1] & 0x80) == 0)
            {
                int n = d[at - 1];
                if (Plausible(n, minBytes, d.Length - at)) return n - 1;
            }
            return minBytes;
        }

        static bool Plausible(int n, int minBytes, int remaining)
        {
            return (n % 2) == 1 && n - 1 >= minBytes && n <= remaining;
        }

        // reading bindings

        public class Binding
        {
            public string MapName;
            public string ActionName;
            public string Id;
            public string Path;
            public bool IsKeyboard { get { return Path != null && Path.StartsWith("<Keyboard>/"); } }
            public string KeyName { get { return IsKeyboard ? Path.Substring("<Keyboard>/".Length) : ""; } }
        }

        public static List<Binding> Parse(string json)
        {
            var result = new List<Binding>();
            var ser = new JavaScriptSerializer();
            ser.MaxJsonLength = int.MaxValue;
            var root = (Dictionary<string, object>)ser.DeserializeObject(json);
            var maps = root.ContainsKey("maps") ? root["maps"] as object[] : null;
            if (maps == null) return result;
            foreach (Dictionary<string, object> map in maps)
            {
                string mapName = map.ContainsKey("name") ? map["name"] as string : "?";
                var bindings = map.ContainsKey("bindings") ? map["bindings"] as object[] : null;
                if (bindings == null) continue;
                foreach (Dictionary<string, object> b in bindings)
                {
                    result.Add(new Binding
                    {
                        MapName = mapName,
                        ActionName = b.ContainsKey("action") ? b["action"] as string : "",
                        Id = b.ContainsKey("id") ? b["id"] as string : "",
                        Path = b.ContainsKey("path") ? b["path"] as string : "",
                    });
                }
            }
            return result;
        }

        public static string ApplyChanges(string json, Dictionary<string, string> changes)
        {
            string s = json;
            foreach (var kv in changes)
            {
                int idAt = IndexOfField(s, "id", kv.Key, 0);
                if (idAt < 0) throw new InvalidDataException("Binding id not found: " + kv.Key);

                int valStart, valEnd;
                if (!FindFieldValue(s, "path", idAt, out valStart, out valEnd))
                    throw new InvalidDataException("Binding " + kv.Key + " has no following 'path' field");

                s = s.Substring(0, valStart) + kv.Value + s.Substring(valEnd);
            }
            return s;
        }

        static int IndexOfField(string s, string name, string value, int from)
        {
            int i = s.IndexOf("\"" + name + "\": \"" + value + "\"", from, StringComparison.Ordinal);
            if (i >= 0) return i;
            return s.IndexOf("\"" + name + "\":\"" + value + "\"", from, StringComparison.Ordinal);
        }

        static bool FindFieldValue(string s, string name, int from, out int valStart, out int valEnd)
        {
            valStart = valEnd = -1;
            int a = s.IndexOf("\"" + name + "\": \"", from, StringComparison.Ordinal);
            int b = s.IndexOf("\"" + name + "\":\"", from, StringComparison.Ordinal);
            int at, skip;
            if (a >= 0 && (b < 0 || a <= b)) { at = a; skip = name.Length + 5; }
            else if (b >= 0) { at = b; skip = name.Length + 4; }
            else return false;
            valStart = at + skip;
            valEnd = s.IndexOf('"', valStart);
            return valEnd >= 0;
        }

        public static string Minify(string json)
        {
            var sb = new StringBuilder(json.Length);
            bool inStr = false, esc = false;
            foreach (char c in json)
            {
                if (inStr)
                {
                    sb.Append(c);
                    if (esc) esc = false;
                    else if (c == '\\') esc = true;
                    else if (c == '"') inStr = false;
                }
                else
                {
                    if (c == '"') { inStr = true; sb.Append(c); }
                    else if (c == ' ' || c == '\t' || c == '\r' || c == '\n') { }
                    else sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static void Save(string dllPath, string newJson)
        {
            byte[] dll = File.ReadAllBytes(dllPath);
            Region r = Locate(dll);

            newJson = Minify(newJson);

            if (newJson.Length > r.CharLength)
                throw new InvalidDataException(string.Format(
                    "The modified config is longer than the original ({0} > {1} characters) and won't fit. Please reduce the changes.",
                    newJson.Length, r.CharLength));

            string padded = newJson.PadRight(r.CharLength, ' ');
            byte[] bytes = Encoding.Unicode.GetBytes(padded);
            if (bytes.Length != r.CharLength * 2)
                throw new InvalidDataException("Non-BMP characters present; cannot write back in place.");

            string bak = GameSystemPatch.BackupPathFor(dllPath);
            if (!File.Exists(bak)) File.Copy(dllPath, bak);

            Array.Copy(bytes, 0, dll, r.ByteStart, bytes.Length);
            GameSystemPatch.WriteAtomically(dllPath, dll);
        }

        static int IndexOf(byte[] hay, byte[] needle, int from)
        {
            int last = hay.Length - needle.Length;
            for (int i = from; i <= last; i++)
            {
                int j = 0;
                while (j < needle.Length && hay[i + j] == needle[j]) j++;
                if (j == needle.Length) return i;
            }
            return -1;
        }

        public static readonly string[] KeyNames =
        {
            "a","b","c","d","e","f","g","h","i","j","k","l","m",
            "n","o","p","q","r","s","t","u","v","w","x","y","z",
            "digit0","digit1","digit2","digit3","digit4","digit5","digit6","digit7","digit8","digit9",
            "numpad0","numpad1","numpad2","numpad3","numpad4","numpad5","numpad6","numpad7","numpad8","numpad9",
            "numpadEnter","numpadPeriod","numpadPlus","numpadMinus","numpadMultiply","numpadDivide","numpadEquals",
            "leftArrow","rightArrow","upArrow","downArrow",
            "enter","space","escape","tab","backspace",
            "insert","delete","home","end","pageUp","pageDown",
            "leftShift","rightShift","leftCtrl","rightCtrl","leftAlt","rightAlt",
            "f1","f2","f3","f4","f5","f6","f7","f8","f9","f10","f11","f12",
            "backquote","minus","equals","leftBracket","rightBracket","backslash",
            "semicolon","quote","comma","period","slash",
            "capsLock","numLock","scrollLock","pause","printScreen",
        };
    }
}
