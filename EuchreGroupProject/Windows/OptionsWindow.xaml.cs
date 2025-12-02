using System;
using System.Windows;
using System.Windows.Controls;
using EuchreGroupProject.Windows.Pages;
using EuchreGroupProject.Windows;
using EuchreGroupProject.StaticClasses;

namespace EuchreGroupProject
{
    /// <summary>
    /// Interaction logic for OptionsScreen.xaml
    /// </summary>
    public partial class OptionsScreen : Window
    {
        private GameState _currentGame;

        public OptionsScreen(GameState currentGame)
        {
            InitializeComponent();
            _currentGame = currentGame;
        }

        private void ContinueGame_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MainMenu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to return to the main menu?\nYou progress will be automatically saved.",
                "Return to Main Menu",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                _currentGame.GameInProgress = !_currentGame.TrumpPhase;
                GameManager.SaveGameState(_currentGame);
                MessageBox.Show("Game saved!");
                Close();
                Navigator.Navigate(new MainMenuPage());
            }
        }

        private void ExitGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit the game?",
                "Exit Game",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
