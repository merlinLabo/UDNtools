using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UDNConfig
{
    /// <summary>
    /// udntools.ini
    /// key=value format, UTF-8 without BOM, preserving comments and order
    /// </summary>
    
    public class IniFile
    {
        readonly List<string> _lines = new List<string>();
        readonly Dictionary<string, int> _index =
            new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public static IniFile Load(string path)
        {
            var f = new IniFile();
            if (File.Exists(path))
                foreach (var line in File.ReadAllLines(path, new UTF8Encoding(false)))
                    f.AddLine(line);
            return f;
        }

        void AddLine(string line)
        {
            _lines.Add(line);
            string t = line.Trim();
            if (t.Length == 0 || t[0] == '#' || t[0] == ';' || t[0] == '[') return;
            int eq = t.IndexOf('=');
            if (eq < 0) return;
            _index[t.Substring(0, eq).Trim()] = _lines.Count - 1;
        }

        public string Get(string key, string def)
        {
            int i;
            if (!_index.TryGetValue(key, out i)) return def;
            string t = _lines[i];
            int eq = t.IndexOf('=');
            string v = t.Substring(eq + 1).Trim();
            return v.Length == 0 ? def : v;
        }

        public bool GetBool(string key, bool def)
        {
            string v = Get(key, def ? "true" : "false");
            return v.Equals("true", StringComparison.OrdinalIgnoreCase) || v == "1"
                || v.Equals("yes", StringComparison.OrdinalIgnoreCase);
        }

        public void Set(string key, string value)
        {
            int i;
            if (_index.TryGetValue(key, out i))
            {
                string original = _lines[i];
                int eq = original.IndexOf('=');
                string prefix = original.Substring(0, eq + 1);
                _lines[i] = prefix + " " + value;
            }
            else
            {
                _lines.Add(key + " = " + value);
                _index[key] = _lines.Count - 1;
            }
        }

        public void SetBool(string key, bool value) { Set(key, value ? "true" : "false"); }

        public void Save(string path)
        {
            File.WriteAllLines(path, _lines, new UTF8Encoding(false));
        }
    }
}
