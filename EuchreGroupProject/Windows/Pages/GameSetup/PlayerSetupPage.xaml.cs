/*
    Allows for players to specify names and choose dealers.
*/


using System.Windows.Controls;
using System.Windows;
using System;

namespace EuchreGroupProject.Windows.Pages
{
    /// <summary>
    /// Interaction logic for PlayerSetupPage.xaml
    /// </summary>
    public partial class PlayerSetupPage : Page
    {

        #region Properties

        /// <summary>
        /// True if player is going against AI.
        /// </summary>
        private bool AgainstAI { get; set; }

        private const string AIPlayerName = "AI Player";
        private const string AIHeaderText = "Player 2 (AI Player)";
        private const string AIRadioButtonText = "AI will be the dealer first!";
        private static Random RNG { get; set; } = new();

        #endregion

        /// <summary>
        /// Instantiates player setup page based on provided param.
        /// </summary>
        /// <param name="againstAI">True if player is going against AI.</param>
        public PlayerSetupPage(bool againstAI)
        {
            InitializeComponent();
            AgainstAI = againstAI;
            Loaded += PlayerSetupPage_Loaded;
        }

        #region Instance Methods

        /// <summary>
        /// Sets up the page baed on AgainstAI property.
        /// </summary>
        private void SetupPage()
        {
            Player1Name.TextChanged += NameChangedHandler;
            Player1IsDealerButton.Click += IsDealerHandler;
            Player2Name.TextChanged += NameChangedHandler;
            Player2IsDealerButton.Click += IsDealerHandler;
            if (AgainstAI) { SetupHumanVsAIPage(); }
        }

        /// <summary>
        /// Locks all player 2 properties down and sets some generic names.
        /// </summary>
        private void SetupHumanVsAIPage()
        {
            Player2Label.Text = AIHeaderText;
            Player2Name.Text = AIPlayerName;
            Player2IsDealerButton.Content = AIRadioButtonText;
            Player2Name.Enabled = false;
        }

        /// <summary>
        /// Enables/disables continue button
        /// </summary>
        private void ValidateInputs()
        {
            if (Player1Name.Text.Length > 0 && Player2Name.Text.Length > 0 && (Player1IsDealerButton.IsChecked == true || Player2IsDealerButton.IsChecked == true))
            {
                ContinueButton.IsEnabled = true;
                return;
            }
            ContinueButton.IsEnabled = false;
        }

        /// <summary>
        /// Continues to next page.
        /// </summary>
        private void Continue()
        {
            // Instantiate players
            Player player1 = new Player(Player1IsDealerButton.IsChecked == true, Player1Name.Text);
            Player player2 = AgainstAI ? new AIPlayer(Player2IsDealerButton.IsChecked == true, Player2Name.Text) : new Player(Player2IsDealerButton.IsChecked == true, Player2Name.Text);

            List<Player> players = new List<Player>() { player1, player2 };
            GameState gameState = new GameState(players);
            Navigator.Navigate(new PlayScreenPage(gameState));
        }

        /// <summary>
        /// Randomizes currently selected dealer
        /// </summary>
        private void RandomizeDealer()
        {
            int dealerNum = RNG.Next(2);
            Player1IsDealerButton.IsChecked = dealerNum == 0;
            Player2IsDealerButton.IsChecked = dealerNum == 1;
        }

        #endregion

        #region Event Handler Methods
        
        /// <summary>
        /// Just calls method to setup page.
        /// </summary>
        /// <param name="sender">This.</param>
        /// <param name="e">Args.</param>
        private void PlayerSetupPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetupPage();
        }

        /// <summary>
        /// Navigates back to challenger select.
        /// </summary>
        /// <param name="sender">Cancel button.</param>
        /// <param name="e">Args.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Navigator.Navigate(new ChallengerSelectorPage());
        }

        /// <summary>
        /// Validates inputs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameChangedHandler(object sender, TextChangedEventArgs e)
        {
            ValidateInputs();
        }

        /// <summary>
        /// Validates inputs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsDealerHandler(object sender, RoutedEventArgs e)
        {
            ValidateInputs();
        }

        /// <summary>
        /// Called on continue button press.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            Continue();
        }

        /// <summary>
        /// Continues with random dealer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RandomizeButton_Click(object sender, RoutedEventArgs e)
        {
            RandomizeDealer();
        }

        #endregion
    }
}
