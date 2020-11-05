using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SlnLauncher2
{
    public partial class SlnLauncher : Form
    {
        private readonly string _localCacheDirectory;
        private readonly string _localCacheFile;

        private List<string> _slns = new List<string>();

        public SlnLauncher()
        {
            _localCacheDirectory = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "SlnLauncher");
            _localCacheFile = Path.Combine(_localCacheDirectory, "FileCache.txt");

            InitializeComponent();
        }

        #region Events

        private void SlnLauncher_Load(object sender, EventArgs e)
        {
            // load projects
            InitProjects();

            if (File.Exists(_localCacheFile))
            {
                _slns = File.ReadAllLines(_localCacheFile, Encoding.UTF8).ToList();
                InitListBox();
            }

            StartNewThread(() =>
            {
                _slns = Constants.Paths
                    .SelectMany(path => Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories))
                    .OrderBy(s => s)
                    .ToList();

                if (!Directory.Exists(_localCacheDirectory))
                {
                    Directory.CreateDirectory(_localCacheDirectory);
                }

                File.WriteAllLines(_localCacheFile, _slns, Encoding.UTF8);

                InitListBox();
            });
        }

        private void InitProjects()
        {
            lstBranch.Items.Clear();
            lstBranch.Items.AddRange(Constants.Paths.ToArray<object>());
            lstBranch.Items.Add("");
            lstBranch.SelectedIndex = 0;
        }

        private void Form1_OnGotFocus(object sender, EventArgs eventArgs)
        {
            tbxSearch.SelectAll();
            tbxSearch.Focus();
        }

        private void cbxBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitListBox();
            tbxSearch.Focus();
        }

        private void tbxSearch_TextChanged(object sender, EventArgs e)
        {
            InitListBox();
        }

        private void tbxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) && e.Modifiers == Keys.None)
            {
                var direction = e.KeyCode == Keys.Up ? -1 : 1;
                var newIndex = Max(lstSln.SelectedIndex, 0) + direction;
                if (0 <= newIndex && newIndex < lstSln.Items.Count)
                {
                    lstSln.SelectedIndex = newIndex;
                }
            }
            else if (IsOpenKeys(e))
            {
                if (lstSln.Items.Count > 0)
                {
                    var start = lstSln.SelectedIndex >= 0 ? lstSln.SelectedIndex : 0;
                    StartFromIndex(start, (e.Modifiers | e.KeyCode) switch
                    {
                        (Keys.Alt | Keys.T) => OpenWindowsTerminal,
                        (Keys.Alt | Keys.S) => OpenRepositoryWithSourceTree,
                        (Keys.None | Keys.Enter) => OpenSolutionWithRider,
                        (Keys.Shift | Keys.Enter) => OpenContainingFolder,
                        (Keys.Control | Keys.Enter) => OpenVisualStudioCode,
                        (Keys.Alt | Keys.Enter) => OpenSolutionWithVisualStudio,
                        (Keys.Alt | Keys.Shift | Keys.Enter) => OpenRepositoryWithSourceTree,
                        _ => NoOperation,
                    });
                }
                else
                {
                    lstBranch.SelectedIndex = lstBranch.Items.Count - 1;
                    InitListBox();
                }
            }
            else if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
            {
                tbxSearch.SelectAll();
            }
            else if (e.KeyCode == Keys.Back && e.Modifiers == Keys.Control)
            {
                e.SuppressKeyPress = true;
                var selStart = tbxSearch.SelectionStart;
                while (selStart > 0 && tbxSearch.Text.Substring(selStart - 1, 1) == " ")
                {
                    selStart--;
                }

                var prevSpacePos = -1;
                if (selStart != 0)
                {
                    prevSpacePos = tbxSearch.Text.LastIndexOf(' ', selStart - 1);
                }

                tbxSearch.Select(prevSpacePos + 1, tbxSearch.SelectionStart - prevSpacePos - 1);
                tbxSearch.SelectedText = "";
            }
        }

        private static bool IsOpenKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                return e.Modifiers == Keys.None
                       || e.Modifiers == Keys.Shift
                       || e.Modifiers == Keys.Control
                       || e.Modifiers == Keys.Alt
                       || e.Modifiers == (Keys.Alt | Keys.Shift)
                    ;
            }

            if (e.KeyCode == Keys.T)
            {
                return e.Modifiers == Keys.Alt;
            }

            return false;
        }

        void lstSln_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = lstSln.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                StartFromIndex(index, OpenSolutionWithRider);
            }
        }

        #endregion

        private void InitListBox()
        {
            lstSln.InvokeIfRequired(() =>
            {
                var proj = lstBranch.SelectedItem.ToString().Replace("/", "\\");

                var search = tbxSearch.Text;
                var exclude = string.Empty;
                switch (search)
                {
                    case "=t":
                        search = @"\frontwebbusiness\FnacDirect.Technical.Framework.Web.sln";
                        break;
                    case "=f":
                        search = @"\frontwebbusiness\FrontWebBusiness.sln";
                        break;
                    case "=w":
                        search = @"\src\web.sln";
                        exclude = "\\clicetmag\\";
                        break;
                    case "=n":
                        search = @"\fnacdirect.nav\FnacDirect.Nav.sln";
                        break;
                    case "=c":
                        search = @"\fnacdirect.nav.contracts\";
                        break;
                }

                search = search.Replace("/", "\\");

                var selects = _slns
                    .Where(s => s.IndexOf(proj, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    .Where(s => s.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0)
                    .Where(s => string.IsNullOrEmpty(exclude) ||
                                s.IndexOf(exclude, StringComparison.InvariantCultureIgnoreCase) < 0)
                    .Cast<object>()
                    .ToArray();

                lstSln.Items.Clear();
                lstSln.Items.AddRange(selects);
                if (lstSln.Items.Count > 0)
                {
                    lstSln.SelectedIndex = 0;
                }
            });
        }

        private void StartFromIndex(int index, Action<string> action)
        {
            if (index < lstSln.Items.Count)
            {
                var item = lstSln.Items[index].ToString();
                action(item);
            }
        }

        private void NoOperation(string obj)
        {
        }

        private static void OpenRepositoryWithSourceTree(string item)
        {
            var folder = GetDirectoryFullName(item);
            Process.Start(Constants.SourceTreePath, $"-f \"{folder}\"");
        }

        private static string GetDirectoryFullName(string item)
        {
            return new FileInfo(item).Directory?.FullName
                   ?? throw new ApplicationException($"Failed to find directory for {item}");
        }

        private static void OpenVisualStudioCode(string item)
        {
            var fi = new FileInfo(item);
            var path = Constants.VisualStudioCodePath;
            Process.Start(path, fi.Directory.FullName);
        }

        private static void OpenSolutionWithVisualStudio(string item)
        {
            var fi = new FileInfo(item);
            var path = Constants.VisualStudioPath;
            Process.Start(path, fi.FullName);
        }

        private static readonly Lazy<string> _riderPath = new Lazy<string>(GetRiderPath);

        private static string GetRiderPath()
        {
            return Constants
                .RiderPaths
                .SelectMany(p => Directory
                    .GetDirectories(p, "JetBrains Rider *", SearchOption.TopDirectoryOnly)
                    .OrderByDescending(i => i)
                    )
                .Select(p => Path.Combine(p, @"bin\rider64.exe"))
                .Where(File.Exists)
                .FirstOrDefault();
        }

        private static void OpenSolutionWithRider(string item)
        {
            var fi = new FileInfo(item);
            Process.Start(_riderPath.Value, fi.FullName);
        }

        private static void OpenWindowsTerminal(string item)
        {
            var directory = GetDirectoryFullName(item);
            var windowsTerminal = Constants.WindowsTerminalPath;

            Process.Start(new ProcessStartInfo(fileName: windowsTerminal) { WorkingDirectory = directory, });
        }

        private static void OpenContainingFolder(string item)
        {
            var fi = new FileInfo(item);
            Process.Start(fi.Directory.FullName);
        }

        public static Thread StartNewThread(Action action)
        {
            var result = new Thread(() => action());
            result.Start();
            return result;
        }

        private int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitProjects();
        }

        private void lstBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbxSearch.Focus();
            InitListBox();
        }
    }
}
