using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using TopBar.Plugins;
using TopBar.Plugins.Extensions;

namespace TopBar
{
    public partial class MainForm : Form
    {
        #region Application bar methods

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        private enum ABMsg : int
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        private enum ABNotify : int
        {
            ABN_STATECHANGE = 0,
            ABN_POSCHANGED,
            ABN_FULLSCREENAPP,
            ABN_WINDOWARRANGE
        }

        private enum ABEdge : int
        {
            ABE_LEFT = 0,
            ABE_TOP,
            ABE_RIGHT,
            ABE_BOTTOM
        }

        private bool fBarRegistered = false;

        [DllImport("Shell32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("User32.dll", ExactSpelling = true,
            CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool MoveWindow
            (IntPtr hWnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int RegisterWindowMessage(string msg);

        private int uCallBack;

        private Screen _screen;

        public void UnregisterBar()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);
            fBarRegistered = false;
        }

        public void RegisterBar(Screen screen)
        {
            this._screen = screen;
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            if (!fBarRegistered)
            {
                uCallBack = RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = uCallBack;

                uint ret = SHAppBarMessage((int)ABMsg.ABM_NEW, ref abd);
                fBarRegistered = true;

                ABSetPos();
            }
            else
            {
                SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);
                fBarRegistered = false;
            }
        }

        private void ABSetPos()
        {
            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = this.Handle;
            abd.uEdge = (int)ABEdge.ABE_TOP;

            if (abd.uEdge == (int)ABEdge.ABE_LEFT || abd.uEdge == (int)ABEdge.ABE_RIGHT)
            {
                //abd.rc.top = 0;
                abd.rc.top = _screen.Bounds.Y;

                //abd.rc.bottom = SystemInformation.PrimaryMonitorSize.Height;
                abd.rc.bottom = _screen.Bounds.Height;

                if (abd.uEdge == (int)ABEdge.ABE_LEFT)
                {
                    //abd.rc.left = 0;
                    abd.rc.left = _screen.Bounds.X;
                    //abd.rc.right = Size.Width;
                    abd.rc.right = _screen.Bounds.Width;
                }
                else
                {
                    //abd.rc.right = SystemInformation.PrimaryMonitorSize.Width;
                    abd.rc.right = _screen.Bounds.Width;

                    abd.rc.left = abd.rc.right - Size.Width;
                }
            }
            else
            {
                //abd.rc.left = 0;
                //abd.rc.right = SystemInformation.PrimaryMonitorSize.Width;

                abd.rc.left = _screen.Bounds.X;
                abd.rc.right = (_screen.Bounds.X + _screen.Bounds.Width);

                if (abd.uEdge == (int)ABEdge.ABE_TOP)
                {
                    //abd.rc.top = 0;
                    abd.rc.top = _screen.Bounds.Y;

                    abd.rc.bottom = Size.Height;
                }
                else
                {
                    //abd.rc.bottom = SystemInformation.PrimaryMonitorSize.Height;
                    abd.rc.bottom = _screen.Bounds.Height;

                    abd.rc.top = abd.rc.bottom - Size.Height;
                }
            }

            // Query the system for an approved size and position.
            SHAppBarMessage((int)ABMsg.ABM_QUERYPOS, ref abd);

            // Adjust the rectangle, depending on the edge to which the
            // appbar is anchored.
            switch (abd.uEdge)
            {
                case (int)ABEdge.ABE_LEFT:
                    abd.rc.right = abd.rc.left + Size.Width;
                    break;

                case (int)ABEdge.ABE_RIGHT:
                    abd.rc.left = abd.rc.right - Size.Width;
                    break;

                case (int)ABEdge.ABE_TOP:
                    abd.rc.bottom = abd.rc.top + Size.Height;
                    break;

                case (int)ABEdge.ABE_BOTTOM:
                    abd.rc.top = abd.rc.bottom - Size.Height;
                    break;
            }

            // Pass the final bounding rectangle to the system.
            SHAppBarMessage((int)ABMsg.ABM_SETPOS, ref abd);

            // Move and size the appbar so that it conforms to the
            // bounding rectangle passed to the system.
            MoveWindow(abd.hWnd, abd.rc.left, abd.rc.top,
                abd.rc.right - abd.rc.left, abd.rc.bottom - abd.rc.top, true);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == uCallBack)
            {
                switch (m.WParam.ToInt32())
                {
                    case (int)ABNotify.ABN_POSCHANGED:
                        ABSetPos();
                        break;
                }
            }

            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= (~0x00C00000); // WS_CAPTION
                cp.Style &= (~0x00800000); // WS_BORDER
                cp.ExStyle = 0x00000080 | 0x00000008; // WS_EX_TOOLWINDOW | WS_EX_TOPMOST
                return cp;
            }
        }


        #endregion Application bar methods

        readonly IEnumerable<IPlugin> _plugins;
        readonly ColorTheme _colorTheme;
        readonly ContextMenuStrip _contextMenuStripMain;
        readonly Configuration _configuration;

        public MainForm(IEnumerable<IPlugin> plugins, ColorTheme colorTheme, Configuration configuration)
        {
            _contextMenuStripMain = new ContextMenuStrip()
            {
                RenderMode = ToolStripRenderMode.System,
            };

            _configuration = configuration;
            _colorTheme = colorTheme;
            _plugins = plugins;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            this.BackColor = _colorTheme.BackgroudColor;

            var screen = Screen.AllScreens.SingleOrDefault(s => s.DeviceName.Equals(_configuration.ScreenDeviceName));
            if (screen == null)
            {
                screen = Screen.AllScreens.Single(s => s.Primary);
                _configuration.ScreenDeviceName = screen.DeviceName;
            }
            this.RegisterBar(screen);

            this.FormClosing += MainForm_FormClosing;

            this.ContextMenuStrip = _contextMenuStripMain;


            foreach (PluginBase plugin in _plugins.Where(p => p is PluginBase))
            {
                var pluginMenu = new ToolStripMenuItem(plugin.Name);

                if (plugin.MenuItems != null)
                    pluginMenu.DropDownItems.AddRange(plugin.MenuItems.ToArray());

                if (pluginMenu.HasDropDownItems)
                    _contextMenuStripMain.Items.Add(pluginMenu);

                this.Controls.Add(plugin);

                plugin.ContextMenuStrip = _contextMenuStripMain;
            }

            _contextMenuStripMain.Items.Add("-");

            var screensMenu = new ToolStripMenuItem("Screen", null, null, "Screens");

            screensMenu.DropDownItems.AddRange(
                Screen.AllScreens.OrderBy(s => s.Bounds.X)
                    .Select((screen, index) => new ToolStripMenuItem($"Screen {index + 1}{(screen.Primary ? " (primary)" : String.Empty)}", null,
                        (sender, e) =>
                            {
                                this.UnregisterBar();
                                this.RegisterBar(screen);
                                this._configuration.ScreenDeviceName = screen.DeviceName;
                                var item = sender as ToolStripMenuItem;
                                if (item != null)
                                {
                                    foreach (var screenItem in screensMenu.DropDownItems.Cast<ToolStripMenuItem>())
                                        screenItem.Checked = false;

                                    item.Checked = true;
                                }

                            }, "Screen")
                    { Tag = screen.DeviceName, Checked = screen.DeviceName.Equals(this._screen.DeviceName) })
                .ToArray());

            _contextMenuStripMain.Items.AddRange(new ToolStripMenuItem[]{
            screensMenu,
            new ToolStripMenuItem("Restart bar", null,
                (sender, e) =>
                {
                    ExitApplication(true);
                }, "Restart"),
            new ToolStripMenuItem("Close bar", null,
                (sender, e) =>
                {
                    ExitApplication(false);
                }, "Exit")
            });

            this.ContextMenuStrip = _contextMenuStripMain;
        }

        private async void ExitApplication(bool restart)
        {
            foreach (var plugin in _plugins)
                await plugin.SaveConfiguration();

            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{this.GetType().Namespace}.json");
            await File.WriteAllTextAsync(configPath, JsonSerializer.Serialize<Configuration>(_configuration));

            UnregisterBar();

            if (restart)
                Application.Restart();

            Process.GetCurrentProcess().Kill();
        }

        private async void ExitApplication()
        {
            foreach (var plugin in _plugins)
                await plugin.SaveConfiguration();

            UnregisterBar();

            Process.GetCurrentProcess().Kill();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            ExitApplication(false);
        }
    }
}