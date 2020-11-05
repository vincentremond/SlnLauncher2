using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlnLauncher2.DTO;

namespace SlnLauncher2
{
    public partial class SlnLauncher : Form
    {
        private readonly string _cacheFile;

        private string[] _projects = new string[0];

        public SlnLauncher()
        {
            _cacheFile = Cache.Init();

            InitializeComponent();
        }

        #region Events

        private void SlnLauncher_Load(object sender, EventArgs e)
        {
            UI_InitBaseFolders();
            _projects = TryInitProjectsListFromCache();
            Helpers.StartNewThread(
                () =>
                {
                    var projectsList = UpdateProjectsListFromDisk();
                    SaveProjectsListToLocalCache(projectsList);
                    UI_UpdateProjectList(projectsList);
                    _projects = projectsList;
                }
            );
        }

        private string[] UpdateProjectsListFromDisk()
        {
            var all = LauncherConfiguration.Current
                .BasePaths
                .SelectMany(path => GetFiles(path).SelectMany(s => s))
                .ToArray();
            var cleaned = new DuplicateCleaner().Clean(all);
            var result = cleaned
                .Select(c => Path.Combine(c.Path, c.Name))
                .Sort()
                .ToArray();
            return result;
        }

        private static IEnumerable<IEnumerable<ItemDescriptor>> GetFiles(string path)
        {
            yield return (
                from file in Directory.GetFiles(path, "*.sln", SearchOption.AllDirectories)
                let fi = new FileInfo(file)
                select new FileDescriptor(fi.Directory.FullName, fi.Name)
            );

            yield return (
                from file in Directory.GetDirectories(path, ".git", SearchOption.AllDirectories)
                let fi = new DirectoryInfo(file)
                select new DirectoryDescriptor(fi.Parent.FullName, fi.Name)
            );
        }

        private string[] TryInitProjectsListFromCache()
        {
            if (!File.Exists(_cacheFile))
            {
                return new string[0];
            }

            var projects = File.ReadAllLines(_cacheFile, Encoding.UTF8);
            UI_UpdateProjectList(projects);
            return projects;
        }

        private void SaveProjectsListToLocalCache(IEnumerable<string> projects)
        {
            File.WriteAllLines(_cacheFile, projects, Encoding.UTF8);
        }

        private void UI_InitBaseFolders()
        {
            lstBaseFolders.Items.Clear();
            lstBaseFolders.Items.AddRange(LauncherConfiguration.Current.BasePaths.ToArray<object>());
            lstBaseFolders.Items.Add("");
            lstBaseFolders.SelectedIndex = 0;
        }

        private void Form1_OnGotFocus(object sender, EventArgs eventArgs)
        {
            tbxSearch.SelectAll();
            tbxSearch.Focus();
        }

        private void tbxSearch_TextChanged(object sender, EventArgs e)
        {
            UI_UpdateProjectList(_projects);
        }

        private void tbxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            var eventKey = e.Modifiers | e.KeyCode;

            switch (eventKey)
            {
                case Keys.None | Keys.Up:
                case Keys.None | Keys.Down:
                    MoveUpOrDown();
                    break;
                case Keys.Control | Keys.A:
                    tbxSearch.SelectAll();
                    break;
                case Keys.Control | Keys.Back:
                    WordDelete();
                    break;
                default:
                    if (Opener.TryGetFromKeys(eventKey, out var action))
                    {
                        TryOpen(action);
                    }

                    break;
            }

            void WordDelete()
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

            void TryOpen(Action<string> action)
            {
                if (lstSln.Items.Count > 0)
                {
                    var start = lstSln.SelectedIndex >= 0 ? lstSln.SelectedIndex : 0;
                    StartFromIndex(start, action);
                }
                else
                {
                    lstBaseFolders.SelectedIndex = lstBaseFolders.Items.Count - 1;
                    UI_UpdateProjectList(_projects);
                }
            }

            void MoveUpOrDown()
            {
                var direction = e.KeyCode switch
                {
                    Keys.Up => -1,
                    Keys.Down => 1,
                    _ => throw new NotImplementedException(),
                };
                var newIndex = Math.Max(lstSln.SelectedIndex, 0) + direction;
                if (newIndex.IsInRangeIncl(0, lstSln.Items.Count - 1))
                {
                    lstSln.SelectedIndex = newIndex;
                }
            }
        }

        private void lstSln_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = lstSln.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                StartFromIndex(index, Opener.DefaultAction);
            }
        }

        #endregion

        private void UI_UpdateProjectList(string[] projects)
        {
            lstSln.InvokeIfRequired(
                () =>
                {
                    var proj = lstBaseFolders.SelectedItem?.ToString()?.Replace("/", "\\") ?? string.Empty;

                    var search = tbxSearch.Text.Replace("/", "\\");

                    var selects = projects
                        .Where(s => s.IndexOf(proj, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        .Where(s => s.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        .Cast<object>()
                        .ToArray();

                    lstSln.Items.Clear();
                    lstSln.Items.AddRange(selects);
                    if (lstSln.Items.Count > 0)
                    {
                        lstSln.SelectedIndex = 0;
                    }
                }
            );
        }

        private void StartFromIndex(int index, Action<string> action)
        {
            if (index < lstSln.Items.Count)
            {
                var item = lstSln.Items[index].ToString();
                action(item);
            }
        }

        private void lstBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbxSearch.Focus();
            UI_UpdateProjectList(_projects);
        }
    }
}
