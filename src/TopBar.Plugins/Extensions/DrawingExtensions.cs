using System.Runtime.InteropServices;

namespace TopBar.Plugins.Extensions
{
    public static class DrawingExtensions
    {
        [DllImport("User32.dll")]
        private static extern long LockWindowUpdate(IntPtr Handle);

        public static void LockWindowUpdate(this Form form)
        {
            LockWindowUpdate(form.Handle);
        }

        public static void UnlockWindowUpdate(this Form form)
        {
            LockWindowUpdate(IntPtr.Zero);
        }

        public static int MeasureDisplayStringWidth(this Label label)
        {
            return MeasureDisplayStringWidth(label.CreateGraphics(), label.Text, label.Font);
        }

        public static int MeasureDisplayStringWidth(this Graphics graphics, string text, Font font)
        {
            StringFormat format = new StringFormat();
            RectangleF rect = new System.Drawing.RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = {
                new CharacterRange(0, text.Length)
            };
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return (int)(rect.Right + 1.0f);
        }

    }
}
