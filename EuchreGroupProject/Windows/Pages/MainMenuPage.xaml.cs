/* 
    Interaction logic for main menu interface.
*/

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using EuchreGroupProject.StaticClasses;

#region Namespace Definition

namespace EuchreGroupProject.Windows.Pages
{
    /// <summary>
    /// Interaction logic for MainMenuPage.
    /// </summary>
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
            InitializeApp();
        }


        /// <summary>
        /// Application setup, at a bare minimum, load all saved game states.
        /// </summary>
        private static void InitializeApp()
        {
            GameManager.LoadGameState();
        }


        #region Event Handler Methods

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new ChallengerSelectorPage());

        }

        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        { 
            if (GameManager.CurrentGameState == null)
            {
                MessageBox.Show("No perviously played games exist yet.");
                return;
            }

            if (MessageBox.Show("Click yes to continue the last played game.", "Continue last game?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Navigator.Navigate(new PlayScreenPage(GameManager.CurrentGameState));
            }
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GuideButton_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new GuidePage());
        }

        #endregion

    }
}

#endregion
