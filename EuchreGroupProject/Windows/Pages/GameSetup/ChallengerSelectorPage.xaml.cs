/*
    Simply prompts user to choose a mode (human vs human or human vs ai), and continues to PlayerSetupPage with the results.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


#region Namespace Definition

namespace EuchreGroupProject.Windows.Pages
{
    /// <summary>
    /// Prompts user to choose a mode (human vs human or human vs ai), and continues to PlayerSetupPage with the results.
    /// </summary>
    public partial class ChallengerSelectorPage : Page
    {
        
        /// <summary>
        /// Instantiates the page.
        /// </summary>
        public ChallengerSelectorPage()
        {
            InitializeComponent();
        }

        #region Event Handler Methods

        /// <summary>
        /// Returns to menu.
        /// </summary>
        /// <param name="sender">ReturnToMenuButton.</param>
        /// <param name="e">Args.</param>
        private void ReturnToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new MainMenuPage());
        }

        /// <summary>
        /// Called on HumanVsAIButton Click.
        /// </summary>
        /// <param name="sender">HumanVsAIButton.</param>
        /// <param name="e">Args.</param>
        private void HumanVsAIButton_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new PlayerSetupPage(true));
        }

        /// <summary>
        /// Called on HumanVsHumanButton Click.
        /// </summary>
        /// <param name="sender">HumanVsHumanButton.</param>
        /// <param name="e">Args.</param>
        private void HumanVsHumanButton_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new PlayerSetupPage(false));
        }

        #endregion
    }
}

#endregion