using Microsoft.Win32;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace TopBar.Plugins
{
    public class ColorTheme
    {
        #region Methods
        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(int msg, ref APPBARDATA data);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        private struct RECT
        {
            public int left, top, right, bottom;
        }

        private const int ABM_GETTASKBARPOS = 5;

        private static Rectangle GetTaskbarPosition()
        {
            APPBARDATA data = new APPBARDATA();
            data.cbSize = Marshal.SizeOf(data);

            IntPtr retval = SHAppBarMessage(ABM_GETTASKBARPOS, ref data);
            if (retval == IntPtr.Zero)
            {
                throw new Win32Exception("Please re-install Windows");
            }

            return new Rectangle(data.rc.left, data.rc.top, data.rc.right - data.rc.left, data.rc.bottom - data.rc.top);
        }

        private static Color GetColourAt(Point location)
        {
            using (Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }

                return screenPixel.GetPixel(0, 0);
            }
        }

        public static Color GetBackgroundColor()
        {
            return GetColourAt(GetTaskbarPosition().Location);
        }

        public static Color GetTextColor()
        {
            return ContrastColor(GetBackgroundColor());
        }

        public static Color GetHoverBackgroundColor()
        {
            const String DWM_KEY = @"Software\Microsoft\Windows\DWM";
            using (RegistryKey dwmKey = Registry.CurrentUser.OpenSubKey(DWM_KEY, RegistryKeyPermissionCheck.ReadSubTree))
            {
                const String KEY_EX_MSG = "The \"HKCU\\" + DWM_KEY + "\" registry key does not exist.";
                if (dwmKey is null) throw new InvalidOperationException(KEY_EX_MSG);

                Object accentColorObj = dwmKey.GetValue("AccentColor");
                if (accentColorObj is Int32 accentColorDword)
                {
                    var parsed = ParseDWordColor(accentColorDword);
                    return Color.FromArgb(parsed.a, parsed.r, parsed.g, parsed.b);
                }
                else
                {
                    const String VALUE_EX_MSG = "The \"HKCU\\" + DWM_KEY + "\\AccentColor\" registry key value could not be parsed as an ABGR color.";
                    throw new InvalidOperationException(VALUE_EX_MSG);
                }
            }

            static (Byte r, Byte g, Byte b, Byte a) ParseDWordColor(Int32 color)
            {
                Byte
                    a = (Byte)((color >> 24) & 0xFF),
                    b = (Byte)((color >> 16) & 0xFF),
                    g = (Byte)((color >> 8) & 0xFF),
                    r = (Byte)((color >> 0) & 0xFF);

                return (r, g, b, a);
            }
        }

        public static Color GetHoverTextColor()
        {
            return ContrastColor(GetHoverBackgroundColor());
        }

        private static Color ContrastColor(Color color)
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (luminance > 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb(d, d, d);
        }
        #endregion


        public Color BackgroudColor { get; set; }
        public Color TextColor { get; set; }
        public Color HoverBackgroundColor { get; set; }
        public Color HoverTextColor { get; set; }

        public ColorTheme()
        {
            this.BackgroudColor = GetBackgroundColor();
            this.TextColor = GetTextColor();
            this.HoverTextColor = GetHoverTextColor();
            this.HoverBackgroundColor = GetHoverBackgroundColor();

        }
    }
}
