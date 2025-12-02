using System.Text;
using System.Windows;
using EuchreGroupProject.Windows.Pages;
using EuchreGroupProject.Windows;

namespace EuchreGroupProject.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Navigator.CurrentMainWindow = this;
            Navigator.Navigate(new MainMenuPage());
        }
    }
}