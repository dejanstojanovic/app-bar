namespace WinAppBar
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool canCreateInstance = false;
            var mutex = new Mutex(true, "D9A46512-C517-40A2-8860-7D440705CC4C", out canCreateInstance);

            if (canCreateInstance)
            {
                ApplicationConfiguration.Initialize();
                Application.EnableVisualStyles();
                Application.Run(new MainForm());

                mutex.ReleaseMutex();
            }
            else
                return;
        }
    }
}