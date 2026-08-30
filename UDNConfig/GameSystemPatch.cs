using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace UDNConfig
{
    /// <summary>
    /// length-preserving IL patches
    /// 
    /// game\DANCEaROUND_Data\Managed\Konami.GameSystem.dll
    /// game\DANCEaROUND_Data\Managed\Assembly-CSharp.dll
    /// </summary>

    public static class GameSystemPatch
    {
        public enum Target { GameSystem, AssemblyCSharp, TestMode4Unity }

        public class Site
        {
            public Target File;
            public string Key; // internal marks
            public long Offset;
            public byte[] Original;
            public byte[] Patched;
            public long AnchorOffset;
            public byte[] Anchor;
            public bool Required; // true = components required for the game to run on PC
            public bool Recommended; // true = generally recommended to be enabled on PC
            public string Name;
            public string Note;
        }

        // UDN-2022062801 original data files
        public const string GameSystemSha256 =
            "02853150cbf9f0d136e13c8022ac718eede02181656c87cf13ff2fb6b9c95e3d";
        public const long GameSystemSize = 1330176;
        public const string AssemblyCSharpSha256 =
            "ec34cd39db9807a6887e5d5bedb5e2aade7f85aecd6ef136601852aa7ac45323";
        public const long AssemblyCSharpSize = 1371136;
        public const string TestMode4UnitySha256 =
            "0ddb1fc5fa29299e5caede5c966dfdf24d443f0a860ca6382436d1f5b81d3dd9";
        public const long TestMode4UnitySize = 124928;

        static byte[] Hex(string s)
        {
            var b = new byte[s.Length / 2];
            for (int i = 0; i < b.Length; i++)
                b[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            return b;
        }

        static byte[] Nops(int n) { return new byte[n]; }

        // prefix + nop padding to a total length of total bytes
        static byte[] Lead(string hexPrefix, int total)
        {
            var b = new byte[total];
            var p = Hex(hexPrefix);
            Array.Copy(p, b, p.Length);
            return b;
        }

        // orig IL IL_001B..IL_038C in PlayerCalibrator.<StartCalibrationAsync>d__5::MoveNext
        const string CalibOriginalB64 =
              "AnyeDQAEKIoKAAoHFn1GBgAEBygECgAGJS0DJisLB3tGBgAEKMILAAoCIwAAAAAAIIxAAnygDQAEe0MGAARsW2kXKCACAAp9oQ0A" +
              "BAIoQgAACn2iDQAEAhZ9ow0ABDhOAQAAAnyeDQAEKIoKAAoo5gkABm/VCQAGOZAAAAAo5gkABm/TCQAGOYEAAAAo5gkABm/RCQAG" +
              "DQICfKANAAR7QAYABAkowwsACiIAAIA/AnygDQAEe0AGAARZAnuiDQAEKMMLAAooMgUACn2iDQAEBwd7RgYABAJ7oQ0ABFh9RgYA" +
              "BAcoBAoABiUtAyYrCwd7RgYABCjCCwAKAnujDQAEF1gTBQIRBX2jDQAEKzACKEIAAAp9og0ABAcWfUYGAAQHKAQKAAYlLQMmKwsH" +
              "e0YGAAQowgsACgIWfaMNAAQCfKANAAR7QgYABBYeAnueDQAEKH0KAAoTBxIHKHoKAAoTBhIGKHsKAAotQQIWJQp9nA0ABAIRBn2k" +
              "DQAEAnydDQAEEgYCKDUCACvdTgIAAAJ7pA0ABBMGAnykDQAE/hVOAAABAhUlCn2cDQAEEgYofAoACgJ7ow0ABAJ8oA0ABHtDBgAE" +
              "P5z+//8CfKANAAR7RAYABBY+mgEAAAIjAAAAAAAAWUACfKANAAR7RAYABGxbaRcoIAIACn2hDQAEAhZ9ow0ABDhVAQAAAnyeDQAE" +
              "KIoKAAoo5gkABm/VCQAGOccAAAAo5gkABm/TCQAGObgAAAACe6INAAQTCCjmCQAGb9EJAAYNAgJ8oA0ABHtABgAECSjDCwAKIgAA" +
              "gD8CfKANAAR7QAYABFkCe6INAAQowwsACigyBQAKfaINAAQHB3tGBgAEAnuhDQAEWBYg6AMAACg5AAAKfUYGAAQHKAQKAAYlLQMm" +
              "KwsHe0YGAAQowgsACgJ7ow0ABBMFAhEFF1h9ow0ABAJ7og0ABBEIKFwBAAoTCRIJKDUFAAoCfKANAAR7QQYABD6KAAAAAnygDQAE" +
              "e0IGAAQWHgJ7ng0ABCh9CgAKEwcSByh6CgAKEwYSBih7CgAKLUECFyUKfZwNAAQCEQZ9pA0ABAJ8nQ0ABBIGAig1AgAr3aMAAAAC" +
              "e6QNAAQTBgJ8pA0ABP4VTgAAAQIVJQp9nA0ABBIGKHwKAAoCe6MNAAQCfKANAAR7RAYABD+V/v//";

        // orig IL IL_0000..IL_0231 in Testmode4Unity.VirtualCoinSetting::SaveEaCoinXml
        const string SaveEaCoinXmlOriginalB64 =
              "AiiUAAAGb5EAAAZvoQAABigDAQAGDCiUAAAGb5MAAAZvFAEABg0CFggJKAYBAAYTBAIXCAkoBgEABhMFAhgICSgGAQAGEwYX" +
              "jRcAAAElFnJbEABwKKgAAApyYxAAcCioAAAKGY0XAAABJRZycRAAcCioAAAKGI0XAAABJRZyfxAAcCioAAAKco0QAHBzqQAA" +
              "CqIlFxeMVQAAAaJzqgAACqIlF3IBEABwKKgAAAoZjRcAAAElFnKTEABwKKgAAAoYjRcAAAElFnJ/EABwKKgAAApynxAAcHOp" +
              "AAAKoiUXEQSMVQAAAaJzqgAACqIlF3KnEABwKKgAAAoYjRcAAAElFnJ/EABwKKgAAApynxAAcHOpAAAKoiUXEQWMVQAAAaJz" +
              "qgAACqIlGHK5EABwKKgAAAoYjRcAAAElFnJ/EABwKKgAAApynxAAcHOpAAAKoiUXEQaMVQAAAaJzqgAACqJzqgAACqIlGHLL" +
              "EABwKKgAAApy1xAAcCioAAAKGI0XAAABJRZyfxAAcCioAAAKco0QAHBzqQAACqIlFxeMVQAAAaJzqgAACnOrAAAKonOqAAAK" +
              "c6sAAAqic6wAAApzrQAACnNdAgAGEwcRB2+uAAAKKK8AAAoRB2+wAAAKb7EAAAoKcuUQAHAosgAACgsHKLMAAAotDHIREQBw" +
              "KLQAAAoWKii1AAAKBwYGjmlvtgAACii3AAAKB2+4AAAKJhYvDHI/EQBwKLQAAAoWKhcouQAACiYXKg==";

        public static readonly Site[] Sites = new[]
        {
            // ----required for launch----

            new Site {
                File = Target.GameSystem, Key = "standalone",
                Offset = 0x0002E6CB,
                AnchorOffset = -1, Anchor = Hex("0A"),
                Original = Hex("162A"), Patched = Hex("172A"),
                Name = "EA3 Standalone Mode",
            },
            new Site {
                File = Target.GameSystem, Key = "dummyseckey",
                Offset = 0x0002E70A,
                AnchorOffset = -1, Anchor = Hex("0A"),
                Original = Hex("162A"), Patched = Hex("172A"),
                Name = "Skip Seckey Check",
            },
            new Site {
                File = Target.GameSystem, Key = "onlineupdate",
                Offset = 0x0000BDB8,
                AnchorOffset = -12, Anchor = Hex("1330040033000000C0000011"),
                Original = Hex("281F03000A1200120112026F2003000A06163118072D081728A10400062B061828A10400060828A30400062A1628A10400062A"),
                Patched  = ConcatBytes(Nops(44), Hex("1628A10400062A")),
                Name = "Skip Update Check",
            },
            new Site {
                File = Target.GameSystem, Key = "regionjapan",
                Offset = 0x0001EF74,
                AnchorOffset = -12, Anchor = Hex("133002001C00000096010011"),
                Original = Hex("28B60A00061200281F0100062C0C067EEC000004281C0100062A162A"),
                Patched  = ConcatBytes(Nops(26), Hex("172A")),
                Name = "Force Destination Region (Japan)",
            },
            new Site {
                File = Target.GameSystem, Key = "ea3local",
                Offset = 0x0002D655,
                AnchorOffset = -9, Anchor = Hex("8B0F00066F6209000A"),
                Original = Hex("1733"), Patched = Hex("1934"),
                Name = "Force EA3 Local Fallback",
            },
            new Site {
                File = Target.GameSystem, Key = "gateopen",
                Offset = 0x00024C3B,
                AnchorOffset = -1, Anchor = Hex("4E"),
                Original = Hex("287B0F00066F890F00066F8607000A19FE012A"),
                Patched  = ConcatBytes(Nops(17), Hex("172A")),
                Name = "Force Offline Entry",
            },
            new Site {
                File = Target.GameSystem, Key = "keyboard", Recommended = true,
                Offset = 0x000204F9,
                AnchorOffset = -1, Anchor = Hex("2E"),
                Original = Hex("282306000A282A06000A2A"),
                Patched  = ConcatBytes(Nops(10), Hex("2A")),
                Name = "Allow Keyboard Initialize",
            },
            new Site {
                File = Target.AssemblyCSharp, Key = "eacoinxml",
                Offset = 0x0008B784,
                AnchorOffset = -0x138, Anchor = Hex("1B300700311500001B050011"),
                Original = Hex("283604000A6FAA0B000A6FA912000A26"),
                Patched  = Nops(16),
                Name = "Remove SaveEaCoinXml Call",
            },

            // ----optional----

            new Site {
                File = Target.AssemblyCSharp, Key = "fastsave", Required = false, Recommended = true,
                Offset = 0x00059A51,
                AnchorOffset = -12, Anchor = Hex("0A6FEA0C000A027BC3120004"),
                Original = Hex("19220000A041"), Patched = Hex("162200004040"),
                Name = "Force Guest Ending",
            },
            new Site {
                File = Target.AssemblyCSharp, Key = "fastsave", Required = false, Recommended = true,
                Offset = 0x00053A54,
                AnchorOffset = -12, Anchor = Hex("0A6F8D0B000A027BA9110004"),
                Original = Hex("19220000A041"), Patched = Hex("162200004040"),
                Name = "Force Guest Ending",
                Note = "GameTotalResultSceneController: same retry/timeout fix as the SavePlayDataService site above",
            },
            new Site {
                File = Target.GameSystem, Key = "logger",
                Offset = 0x00001900,
                AnchorOffset = -1, Anchor = Hex("1A"),
                Original = Hex("283A00000A2A"),
                Patched  = Hex("00000000172A"),
                Name = "Enable Unity Logger",
            },
            new Site {
                File = Target.GameSystem, Key = "skipcalib2", Required = false,
                Offset = 0x00041D97,
                AnchorOffset = -0x27, Anchor = Hex("1B300400F7030000ED020011"),
                Original = Convert.FromBase64String(CalibOriginalB64),
                Patched  = Lead("386D030000", 0x372),
                Name = "Skip Player Calibrator (Buffer)",
            },
            new Site {
                File = Target.AssemblyCSharp, Key = "skipcalib", Required = false,
                Offset = 0x00069FED,
                AnchorOffset = -0x1D, Anchor = Hex("1B300400B500000078040011"),
                Original = Hex("077B4E070004027B38160004027B39160004027B3A1600046F6F0A00061304120428DE0F000A0D120328DF0F000A2D3C0216250A7D3516000402097D3B160004027C3616000412030228DF04002BDE53027B3B1600040D027C3B160004FE15DF02001B0215250A7D35160004120328E00F000A0C"),
                Patched  = ConcatBytes(Hex("1202177D8110000A"), Nops(116 - 8)),
                Name = "Skip Player Calibrator (Direct)",
            },
            new Site {
                File = Target.AssemblyCSharp, Key = "skipcalib", Required = false,
                Offset = 0x000869DA,
                AnchorOffset = -12, Anchor = Hex("077BDC0C0004176F7800000A"),
                Original = Hex("077BE40C000428B101002B7BB707000A11117BE10F000A28A500000A7DB807000A"),
                Patched  = Nops(33),
                Name = "Skip Player Calibrator (Direct)",
                Note = "GameEntry_Calibration.DoTaskAsync: companion edit, removes the m_fkApply null-ref " +
                       "that follows since FullBodyFK4 is never Initialize()'d without a camera",
            },

            new Site {
                File = Target.GameSystem, Key = "unlockall", Required = false,
                Offset = 0x00015FBC,
                AnchorOffset = -0xC, Anchor = Hex("133002002E00000029010011"),
                Original = Hex("021759450400000002000000060000000A0000000E0000002B10160A2B0E180A2B0A170A2B06190A2B02160A062A"),
                Patched  = ConcatBytes(Nops(44), Hex("172A")),
                Name = "Unlock All Songs",
            },
            new Site {
                File = Target.AssemblyCSharp, Key = "autoplay", Required = false,
                Offset = 0x0002BA1A,
                AnchorOffset = -1, Anchor = Hex("1E"),
                Original = Hex("027B400800042A"),
                Patched  = Hex("0000000000172A"),
                Name = "Auto Play",
            },

            new Site {
                File = Target.TestMode4Unity, Key = "testmodesave", Required = false,
                Offset = 0x00003F28,
                AnchorOffset = -12, Anchor = Hex("133012003202000020000011"),
                Original = Convert.FromBase64String(SaveEaCoinXmlOriginalB64),
                Patched  = ConcatBytes(Nops(560), Hex("162A")),
                Name = "Testmode Patch",
            },
            new Site {
                File = Target.TestMode4Unity, Key = "freeplay", Required = false,
                Offset = 0x00002F8F,
                AnchorOffset = -12, Anchor = Hex("8D000004037D510200042A32"),
                Original = Hex("027B8D0000047B510200042A"),
                Patched  = ConcatBytes(Nops(10), Hex("172A")),
                Name = "Free Play",
            },
        };

        static byte[] ConcatBytes(byte[] a, byte[] b)
        {
            var r = new byte[a.Length + b.Length];
            Array.Copy(a, 0, r, 0, a.Length);
            Array.Copy(b, 0, r, a.Length, b.Length);
            return r;
        }

        public enum State { NotFound, Unknown, Original, Patched }

        // Required items
        public static bool IsRequired(string key)
        {
            foreach (var s in Sites)
                if (s.Key == key && s.Required) return true;
            return false;
        }

        // Recommended items. Don't affect launching, but are basically always wanted without AVS
        public static bool IsRecommended(string key)
        {
            foreach (var s in Sites)
                if (s.Key == key && s.Recommended) return true;
            return false;
        }

        public static string PathFor(string root, Target t)
        {
            string dir = Path.Combine(root, "game", "DANCEaROUND_Data", "Managed");
            string name;
            switch (t)
            {
                case Target.GameSystem: name = "Konami.GameSystem.dll"; break;
                case Target.TestMode4Unity: name = "testmode4unity.dll"; break;
                default: name = "Assembly-CSharp.dll"; break;
            }
            return Path.Combine(dir, name);
        }

        public static string Sha256Of(string path)
        {
            using (var s = File.OpenRead(path))
            using (var h = SHA256.Create())
                return BitConverter.ToString(h.ComputeHash(s)).Replace("-", "").ToLowerInvariant();
        }

        // Reads the current state of one patch site
        public static State Check(string root, Site s, out string detail)
        {
            detail = "";
            string path = PathFor(root, s.File);
            if (!File.Exists(path)) { detail = "Not found: " + Path.GetFileName(path); return State.NotFound; }

            byte[] data;
            try { data = File.ReadAllBytes(path); }
            catch (Exception e) { detail = e.Message; return State.Unknown; }

            if (s.Offset + s.Original.Length > data.Length) { detail = "File is shorter than expected"; return State.Unknown; }
            if (s.Anchor != null && !Match(data, s.Offset + s.AnchorOffset, s.Anchor))
            {
                detail = "Anchor check failed, DLL version may differ";
                return State.Unknown;
            }
            if (Match(data, s.Offset, s.Patched)) return State.Patched;
            if (Match(data, s.Offset, s.Original)) return State.Original;
            detail = "Unexpected byte sequence at this location";
            return State.Unknown;
        }

        public static string BackupPathFor(string dllPath) { return dllPath + ".udnbak"; }

        // Applies or reverts each site individually
        public static void ApplySelection(string root, IDictionary<string, bool> desired)
        {
            foreach (Target t in new[] { Target.GameSystem, Target.AssemblyCSharp, Target.TestMode4Unity })
            {
                var mine = new List<Site>();
                foreach (var s in Sites)
                    if (s.File == t && desired.ContainsKey(s.Key)) mine.Add(s);
                if (mine.Count == 0) continue;

                string path = PathFor(root, t);
                if (!File.Exists(path))
                    throw new FileNotFoundException("Not found: " + path);

                string bak = BackupPathFor(path);
                if (!File.Exists(bak)) File.Copy(path, bak);

                byte[] data = File.ReadAllBytes(path);
                bool dirty = false;
                foreach (var s in mine)
                {
                    if (s.Anchor != null && !Match(data, s.Offset + s.AnchorOffset, s.Anchor))
                        throw new InvalidDataException(s.Name + ": anchor check failed, aborted, file not modified.");
                    bool isPatched = Match(data, s.Offset, s.Patched);
                    bool isOriginal = Match(data, s.Offset, s.Original);
                    if (!isPatched && !isOriginal)
                        throw new InvalidDataException(s.Name + ": unexpected bytes at this location, aborted, file not modified.");

                    byte[] want = desired[s.Key] ? s.Patched : s.Original;
                    if (!Match(data, s.Offset, want))
                    {
                        Array.Copy(want, 0, data, s.Offset, want.Length);
                        dirty = true;
                    }
                }
                if (dirty) WriteAtomically(path, data);
            }
        }

        // >Restores the whole file from *.udnbak
        public static void RevertAll(string root)
        {
            foreach (Target t in new[] { Target.GameSystem, Target.AssemblyCSharp, Target.TestMode4Unity })
            {
                string path = PathFor(root, t);
                string bak = BackupPathFor(path);
                if (File.Exists(bak) && File.Exists(path)) File.Copy(bak, path, true);
            }
        }

        static bool Match(byte[] data, long at, byte[] pat)
        {
            if (at < 0 || at + pat.Length > data.Length) return false;
            for (int i = 0; i < pat.Length; i++)
                if (data[at + i] != pat[i]) return false;
            return true;
        }

        internal static void WriteAtomically(string path, byte[] data)
        {
            string tmp = path + ".udntmp";
            File.WriteAllBytes(tmp, data);
            File.Copy(tmp, path, true);
            File.Delete(tmp);
        }
    }
}
