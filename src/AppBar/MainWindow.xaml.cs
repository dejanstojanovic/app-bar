using System.Windows;
using WpfAppBar;

namespace AppBar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AppBarFunctions.SetAppBar(this, ABEdge.Top);
            InitializeComponent();
        }
    }
}