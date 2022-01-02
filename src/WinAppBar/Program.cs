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
            var mutex = new Mutex(true, "330ECF4F-5DB6-4A70-B227-D03EB2AC9E6F", out canCreateInstance);

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