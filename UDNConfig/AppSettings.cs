using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UDNConfig
{
    /// <summary>
    /// UDNAppSetting.json windowSettings section
    /// prop\UDNAppSetting.json
    ///
    /// Konami.GameSystem.ApplicationConfig.StartUp() reads this file, then GameScreen.ApplySettings() feeds it straight into:
    ///   QualitySettings.vSyncCount
    ///   Application.targetFrameRate
    ///   Screen.SetResolution(screenWidth, screenHeight, fullScreenMode, preferredRefreshRate)
    ///
    /// fullScreenMode is UnityEngine.FullScreenMode:
    ///   0 = ExclusiveFullScreen
    ///   1 = FullScreenWindow (borderless)
    ///   2 = MaximizedWindow
    ///   3 = Windowed
    ///
    /// The original arcade setting is 0 + 1920x3260, portrait tri-panel, which has to be changed for a normal monitor.
    /// </summary>

    public class WindowSettings
    {
        public bool VirtualScreenEnable;
        public int VSyncCount;
        public int TargetFrameRate;
        public int FullScreenMode;
        public int ScreenWidth;
        public int ScreenHeight;
        public int PreferredRefreshRate;

        static string Block(string json)
        {
            var m = Regex.Match(json, "\"windowSettings\"\\s*:\\s*\\{(?<body>[^{}]*)\\}",
                                RegexOptions.Singleline);
            if (!m.Success) throw new InvalidDataException(
                "windowSettings section not found in UDNAppSetting.json");
            return m.Groups["body"].Value;
        }

        static int ReadInt(string body, string key, int def)
        {
            var m = Regex.Match(body, "\"" + key + "\"\\s*:\\s*(-?\\d+)");
            return m.Success ? int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture) : def;
        }

        static bool ReadBool(string body, string key, bool def)
        {
            var m = Regex.Match(body, "\"" + key + "\"\\s*:\\s*(true|false)");
            return m.Success ? m.Groups[1].Value == "true" : def;
        }

        public static WindowSettings Load(string path)
        {
            string json = File.ReadAllText(path, new UTF8Encoding(false));
            string body = Block(json);
            return new WindowSettings
            {
                VirtualScreenEnable  = ReadBool(body, "virtualScreenEnable", false),
                VSyncCount           = ReadInt(body, "vSyncCount", 1),
                TargetFrameRate      = ReadInt(body, "targetFrameRate", 60),
                FullScreenMode       = ReadInt(body, "fullScreenMode", 0),
                ScreenWidth          = ReadInt(body, "screenWidth", 1920),
                ScreenHeight         = ReadInt(body, "screenHeight", 3260),
                PreferredRefreshRate = ReadInt(body, "preferredRefreshRate", 60),
            };
        }

        public void Save(string path)
        {
            string json = File.ReadAllText(path, new UTF8Encoding(false));

            string bak = path + ".udnbak";
            if (!File.Exists(bak)) File.Copy(path, bak);

            var m = Regex.Match(json, "\"windowSettings\"\\s*:\\s*\\{(?<body>[^{}]*)\\}",
                                RegexOptions.Singleline);
            if (!m.Success) throw new InvalidDataException("windowSettings section not found in UDNAppSetting.json");

            string body = m.Groups["body"].Value;
            body = SetInt(body, "vSyncCount", VSyncCount);
            body = SetInt(body, "targetFrameRate", TargetFrameRate);
            body = SetInt(body, "fullScreenMode", FullScreenMode);
            body = SetInt(body, "screenWidth", ScreenWidth);
            body = SetInt(body, "screenHeight", ScreenHeight);
            body = SetInt(body, "preferredRefreshRate", PreferredRefreshRate);
            body = SetBool(body, "virtualScreenEnable", VirtualScreenEnable);

            int bodyStart = m.Groups["body"].Index;
            int bodyLen = m.Groups["body"].Length;
            string result = json.Substring(0, bodyStart) + body + json.Substring(bodyStart + bodyLen);

            File.WriteAllText(path, result, new UTF8Encoding(false));
        }

        static string SetInt(string body, string key, int value)
        {
            string pat = "(\"" + key + "\"\\s*:\\s*)(-?\\d+)";
            if (!Regex.IsMatch(body, pat))
                throw new InvalidDataException("Missing " + key + " field in windowSettings");
            return Regex.Replace(body, pat, "${1}" + value.ToString(CultureInfo.InvariantCulture));
        }

        static string SetBool(string body, string key, bool value)
        {
            string pat = "(\"" + key + "\"\\s*:\\s*)(true|false)";
            if (!Regex.IsMatch(body, pat)) return body;
            return Regex.Replace(body, pat, "${1}" + (value ? "true" : "false"));
        }
    }
}
