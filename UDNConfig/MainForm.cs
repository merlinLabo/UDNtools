using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace UDNConfig
{
    public partial class MainForm : Form
    {
        string _root;
        string _iniPath, _appSettingPath, _gsDllPath;
        IniFile _ini;

        readonly Dictionary<string, CheckBox> _patchChecks = new Dictionary<string, CheckBox>();
        readonly Dictionary<string, CheckBox> _envChecks = new Dictionary<string, CheckBox>();

        List<InputActionsEditor.Binding> _bindings;
        readonly Dictionary<string, string> _keyChanges = new Dictionary<string, string>();

        static readonly Tuple<string, string>[] EnvOptions = new[]
        {
            Tuple.Create("DUMMY_BI2A",   "Skip BI2A Main I/O Board"),
            Tuple.Create("DUMMY_ICCA",   "Skip ICCA Card Reader"),
            Tuple.Create("DUMMY_CAMERA", "Skip Intel RealSense"),
            Tuple.Create("STANDALONE_ENABLE",    "Standalone Mode (Requires DLL Patch)"),
            Tuple.Create("DISABLE_WATCHDOG",     "BI2A I/O–specific variable. Does not affect runtime behavior"),
            Tuple.Create("ALL_LEVEL_PLAYABLE",   "Bypass Level Limitation in Light Mode (≤6). Not Equivalent to Unlocking Songs"),
            Tuple.Create("AGING_MODE",           "AGING_MODE"),
            Tuple.Create("GOTO_QCMODE",          "QC_MODE"),
            Tuple.Create("REBOOT_TEST",          "REBOOT_TEST"),
        };

        public MainForm()
        {
            InitializeComponent();
        }
        // dynamic controls: environment variable checkboxes, DLL patch checkboxes
        void PopulateDynamicControls()
        {
            envCheckPanel.SuspendLayout();
            foreach (var opt in EnvOptions)
            {
                var cb = new CheckBox
                {
                    Text = opt.Item1,
                    AutoSize = false,
                    Width = 144,
                    Height = 20,
                    Margin = new Padding(1, 2, 1, 2),
                    Font = new Font(Font.FontFamily, 8f),
                    AutoEllipsis = true,
                };
                _envChecks[opt.Item1] = cb;
                envCheckPanel.Controls.Add(cb);
                toolTip.SetToolTip(cb, opt.Item2);
            }
            envCheckPanel.ResumeLayout(true);
            envCheckPanel.PerformLayout();
            // PreferredSize
            envCheckPanel.Height = envCheckPanel.GetPreferredSize(new Size(envCheckPanel.Width, 0)).Height;
            envGroupBox.Height = envCheckPanel.Bottom + 12;

            patchGroupBox.Top = envGroupBox.Bottom + 8;

            var groups = new List<GameSystemPatch.Site>();
            foreach (var site in GameSystemPatch.Sites)
                if (!groups.Exists(x => x.Key == site.Key)) groups.Add(site);

            patchCheckPanel.SuspendLayout();
            foreach (var site in groups)
            {
                string text = site.Name;
                var cb = new CheckBox
                {
                    Text = text,
                    AutoSize = false,
                    Width = 310,
                    Height = 20,
                    Font = new Font(Font.FontFamily, 8f),
                    AutoEllipsis = true,
                };
                _patchChecks[site.Key] = cb;
                patchCheckPanel.Controls.Add(cb);
                toolTip.SetToolTip(cb, text);
            }
            patchCheckPanel.ResumeLayout(true);
            patchCheckPanel.PerformLayout();

            patchGroupBox.Height = patchCheckPanel.Bottom + 12;
        }
        // events
        void browseButton_Click(object sender, EventArgs e)
        {
            using (var d = new FolderBrowserDialog())
                if (d.ShowDialog() == DialogResult.OK) SetRoot(d.SelectedPath);
        }

        void applyButton_Click(object sender, EventArgs e)
        {
            ApplyAll();
        }

        void launchButton_Click(object sender, EventArgs e)
        {
            Launch();
        }

        void restoreButton_Click(object sender, EventArgs e)
        {
            var r = MessageBox.Show(this,
                "This restores every managed DLL to its original (pre-patch) bytes from *.udnbak. Continue?",
                "Restore DLL", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (r != DialogResult.Yes) return;
            try
            {
                GameSystemPatch.RevertAll(_root);
                RefreshPatchCheckStates();
                Status("DLLs restored to original.");
            }
            catch (Exception ex) { Status("Restore failed: " + ex.Message); }
        }

        void presetButton_Click(object sender, EventArgs e)
        {
            _w.Value = 590; _h.Value = 1000; _modeCombo.SelectedIndex = 3;
        }

        void preset2Button_Click(object sender, EventArgs e)
        {
            _w.Value = 1920; _h.Value = 1080; _modeCombo.SelectedIndex = 1;
        }

        void reloadButton_Click(object sender, EventArgs e)
        {
            LoadBindings();
        }

        void _mapCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillGrid();
        }

        void _grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 1) return;
            var row = _grid.Rows[e.RowIndex];
            string id = (string)row.Cells[2].Value;
            string key = row.Cells[1].Value as string;
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(key))
                _keyChanges[id] = "<Keyboard>/" + key;
        }

        void _grid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (_grid.IsCurrentCellDirty) _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // loading

        static string GuessRoot()
        {
            // prefer exe directory
            var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 4 && dir != null; i++)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "game")) &&
                    Directory.Exists(Path.Combine(dir.FullName, "prop")))
                    return dir.FullName;
                dir = dir.Parent;
            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        void SetRoot(string root)
        {
            _root = root.TrimEnd('\\');
            _rootBox.Text = _root;
            _iniPath = Path.Combine(_root, "udntools.ini");
            _appSettingPath = Path.Combine(_root, "prop", "UDNAppSetting.json");
            _gsDllPath = GameSystemPatch.PathFor(_root, GameSystemPatch.Target.GameSystem);

            _ini = IniFile.Load(_iniPath);
            foreach (var kv in _envChecks)
            {
                bool def = kv.Key == "CONNECT_DUMMY_BI2A" || kv.Key == "CONNECT_DUMMY_ICCA"
                        || kv.Key == "CONNECT_DUMMY_CAMERA" || kv.Key == "STANDALONE_ENABLE"
                        || kv.Key == "DISABLE_WATCHDOG";
                kv.Value.Checked = _ini.GetBool(kv.Key, def);
            }
            _frontVol.Text = _ini.Get("SET_FRONT_VOLUME", "");
            _wooferVol.Text = _ini.Get("SET_WOOFER_VOLUME", "");
            _extraArgs.Text = _ini.Get("LAUNCH_SCREEN_ARGS", "");

            LoadScreen();
            RefreshPatchCheckStates();
            LoadBindings();
        }

        void LoadScreen()
        {
            try
            {
                var ws = WindowSettings.Load(_appSettingPath);
                _modeCombo.SelectedIndex = Math.Max(0, Math.Min(3, ws.FullScreenMode));
                _w.Value = Math.Max(_w.Minimum, Math.Min(_w.Maximum, ws.ScreenWidth));
                _h.Value = Math.Max(_h.Minimum, Math.Min(_h.Maximum, ws.ScreenHeight));
                _refresh.Value = Math.Max(0, Math.Min(480, ws.PreferredRefreshRate));
                _vsync.SelectedIndex = Math.Max(0, Math.Min(2, ws.VSyncCount));
                _fps.Value = Math.Max(-1, Math.Min(480, ws.TargetFrameRate));
                _virtualScreen.Checked = ws.VirtualScreenEnable;
            }
            catch (Exception e)
            {
                _modeCombo.SelectedIndex = 3;
                Status("Failed to read UDNAppSetting.json: " + e.Message);
            }
        }

        void LoadBindings()
        {
            _keyChanges.Clear();
            _bindings = null;
            _mapCombo.Items.Clear();
            _grid.Rows.Clear();
            try
            {
                byte[] dll = File.ReadAllBytes(_gsDllPath);
                var region = InputActionsEditor.Locate(dll);
                _bindings = InputActionsEditor.Parse(region.Json);
                foreach (var name in _bindings.Select(b => b.MapName).Distinct())
                    _mapCombo.Items.Add(name);
                if (_mapCombo.Items.Count > 0) _mapCombo.SelectedIndex = 0;
            }
            catch (Exception e)
            {
                Status("Failed to read key bindings: " + e.Message);
            }
        }

        void FillGrid()
        {
            _grid.Rows.Clear();
            if (_bindings == null || _mapCombo.SelectedItem == null) return;
            string map = (string)_mapCombo.SelectedItem;

            var keyCol = (DataGridViewComboBoxColumn)_grid.Columns[1];
            var choices = new List<string>(InputActionsEditor.KeyNames);
            foreach (var b in _bindings.Where(x => x.MapName == map && x.IsKeyboard))
                if (!choices.Contains(b.KeyName)) choices.Add(b.KeyName);
            keyCol.Items.Clear();
            keyCol.Items.AddRange(choices.Cast<object>().ToArray());

            foreach (var b in _bindings.Where(x => x.MapName == map && x.IsKeyboard))
            {
                string key = _keyChanges.ContainsKey(b.Id)
                    ? _keyChanges[b.Id].Substring("<Keyboard>/".Length) : b.KeyName;
                _grid.Rows.Add(b.ActionName, key, b.Id);
            }
        }

        // action
        void RefreshPatchCheckStates()
        {
            var state = new Dictionary<string, GameSystemPatch.State>();
            foreach (var site in GameSystemPatch.Sites)
            {
                string d;
                var st = GameSystemPatch.Check(_root, site, out d);
                if (!state.ContainsKey(site.Key)) { state[site.Key] = st; continue; }
                if (state[site.Key] != st)
                    state[site.Key] = (st == GameSystemPatch.State.Unknown || state[site.Key] == GameSystemPatch.State.Unknown)
                        ? GameSystemPatch.State.Unknown : GameSystemPatch.State.Original;
            }

            foreach (var kv in state)
            {
                var cb = _patchChecks[kv.Key];
                bool req = GameSystemPatch.IsRequired(kv.Key);
                switch (kv.Value)
                {
                    case GameSystemPatch.State.Patched:
                        cb.Checked = true; cb.Enabled = true; break;
                    case GameSystemPatch.State.Original:
                        cb.Checked = req || GameSystemPatch.IsRecommended(kv.Key);
                        cb.Enabled = true;
                        break;
                    default:
                        cb.Checked = false; cb.Enabled = false; break;
                }
            }
        }

        bool ApplyAll()
        {
            try
            {
                foreach (var kv in _envChecks) _ini.SetBool(kv.Key, kv.Value.Checked);
                _ini.Set("SET_FRONT_VOLUME", _frontVol.Text.Trim());
                _ini.Set("SET_WOOFER_VOLUME", _wooferVol.Text.Trim());
                _ini.Set("LAUNCH_SCREEN_ARGS", _extraArgs.Text.Trim());
                _ini.Set("UNITY_LOG_FILE", "true");
                _ini.Save(_iniPath);

                var ws = new WindowSettings
                {
                    FullScreenMode = _modeCombo.SelectedIndex,
                    ScreenWidth = (int)_w.Value,
                    ScreenHeight = (int)_h.Value,
                    PreferredRefreshRate = (int)_refresh.Value,
                    VSyncCount = _vsync.SelectedIndex,
                    TargetFrameRate = (int)_fps.Value,
                    VirtualScreenEnable = _virtualScreen.Checked,
                };
                ws.Save(_appSettingPath);

                var desired = new Dictionary<string, bool>();
                foreach (var kv in _patchChecks)
                    if (kv.Value.Enabled) desired[kv.Key] = kv.Value.Checked;
                GameSystemPatch.ApplySelection(_root, desired);

                if (_keyChanges.Count > 0)
                {
                    byte[] dll = File.ReadAllBytes(_gsDllPath);
                    var region = InputActionsEditor.Locate(dll);
                    string updated = InputActionsEditor.ApplyChanges(region.Json, _keyChanges);
                    InputActionsEditor.Save(_gsDllPath, updated);
                }


                RefreshPatchCheckStates();
                LoadBindings();

                Status("Settings applied.");
                return true;
            }
            catch (Exception e)
            {
                Status("Apply failed: " + e.Message);
                return false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            PopulateDynamicControls();
            SetRoot(GuessRoot());
        }

        private void patchGroupBox_Enter(object sender, EventArgs e)
        {

        }

        private void keyInfoLabel_Click(object sender, EventArgs e)
        {

        }

        private void keyTab_Click(object sender, EventArgs e)
        {

        }

        private void _grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolTip_Popup(object sender, PopupEventArgs e)
        {

        }

        void Launch()
        {
            string launcher = Path.Combine(_root, "launcher.exe");
            if (!File.Exists(launcher)) { Status("Launcher not found"); return; }
            try { Process.Start(new ProcessStartInfo(launcher) { WorkingDirectory = _root }); }
            catch (Exception e) { Status("Launch failed: " + e.Message); }
        }

        void Status(string msg)
        {
            MessageBox.Show(this, msg, "Alert", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
