using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinAppBar.Extensions
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

    }
}
