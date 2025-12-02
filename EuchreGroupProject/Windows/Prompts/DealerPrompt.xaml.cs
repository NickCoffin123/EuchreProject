using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EuchreGroupProject.Windows.Pages;

namespace EuchreGroupProject.Windows.Prompts
{
    /// <summary>
    /// Interaction logic for DealerPrompt.xaml
    /// </summary>
    public partial class DealerPrompt : Window
    {
        public DealerPrompt(string dealerName)
        {
            InitializeComponent();
            DealerPromptDisplay.Text = $"{dealerName}, it is your turn to deal!";
        }

        #region Event Handler Methods

        private void EndGameButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Navigator.Navigate(new MainMenuPage());
            Close();
        }

        private void DealButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        #endregion
    }
}
