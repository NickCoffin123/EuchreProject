/* 
    The page where the 2 hand Euchre game will take place.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using EuchreGroupProject.Windows.Prompts;
using EuchreGroupProject.Windows.Components;
using EuchreGroupProject.StaticClasses;
using System.Numerics;

#region Namespace Definition

namespace EuchreGroupProject.Windows.Pages
{
    /// <summary>
    /// Interaction logic for PlayScreenPage.xaml.
    /// </summary>
    partial class PlayScreenPage : Page
    {

        #region Instance Properties

        /// <summary>
        /// The gamestate associated with this instance of PlayScreenPage.
        /// </summary>
        private GameState BoundGameState { get; set; }

        #endregion

        /// <summary>
        /// Sets up play area with a provided
        /// </summary>
        public PlayScreenPage(GameState gameState)
        {
            BoundGameState = gameState;
            InitializeComponent();
            SetupWindow();
        }

        #region Setup

        /// <summary>
        /// Sets up parent window as needed.
        /// </summary>
        private void SetupWindow()
        {
            if (Navigator.CurrentMainWindow == null) { return; }
            Navigator.CurrentMainWindow.WindowState = WindowState.Maximized;
            Loaded += PlayScreenPage_Loaded;
        }

        /// <summary>
        /// Adds deck, stats, and dialogs to screen then starts next hand.
        /// </summary>
        private void SetupGameArea()
        {
            // Setup both hands and trick container
            BottomHandStatCardContainer.Children.Add(BoundGameState.Players[0].BoundStatCard);
            BottomHandDialogContainer.Children.Add(BoundGameState.Players[0].BoundDialogContainer);
            TopHandStatCardContainer.Children.Add(BoundGameState.Players[1].BoundStatCard);
            TopHandDialogContainer.Children.Add(BoundGameState.Players[1].BoundDialogContainer);

            BottomHandContainer.Children.Add(BoundGameState.Players[0].Hand);
            TopHandContainer.Children.Add(BoundGameState.Players[1].Hand);

            // Add deck and start first hand
            DeckAreaContainer.Children.Add(BoundGameState.Deck);

            // Add event listeners to gamestate for UI updates
            BoundGameState.TrickEnded += NextTrick;
            BoundGameState.HandEnded += EndHand;
            BoundGameState.TrumpPhaseEnded += SetTrumpUI;
        }

        #endregion

        #region Gameplay

        /// <summary>
        /// Clears trick container, moves deck to center, allows dealer to deal, then runs through trump phase.
        /// </summary>
        private async void NextHand()
        {
            BoundGameState.TrumpPhase = true;
            NextTrick();
            await CenterDeck();
            if (PromptDeal()) { PromptTrumpOffer(); };
        }

        /// <summary>
        /// Clears current trick UI, adding a new one.
        /// </summary>
        private void NextTrick()
        {
            TrickContainer.Children.Clear();
            TrickContainer.Children.Add(BoundGameState.CreateNewTrick());
        }

        /// <summary>
        /// Prompts the current turn player to deal.
        /// </summary>
        /// <returns>False if player chooses to exit.</returns>
        private bool PromptDeal()
        {
            if (BoundGameState.GetDealer() is AIPlayer)
            {
                BeginDeal();
                return true;
            }

            else if ((new DealerPrompt(BoundGameState.GetDealerName())).ShowDialog() == true)
            {
                BeginDeal();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deals 5 cards to each player from deck.
        /// </summary>
        private void BeginDeal()
        {
            BoundGameState.Deal();
        }

        /// <summary>
        /// Prompts non dealer to order up trump first; if they pass the dealer is offered to pick up.
        /// If both players pass, they are prompted to choose any suit.
        /// </summary>
        private void PromptTrumpOffer()
        {
            // Flip top card as to show trump
            BoundGameState.Deck.FlipTopCard();

            // If dealer or non dealer accepts canditate offer, confirm accept.
            foreach (Player player in BoundGameState.Players)
            {
                if (player is AIPlayer)
                {
                    BoundGameState.AcceptCandidate(player);
                    return;
                }
                
                else if ((new TrumpOfferPrompt(BoundGameState.CandidateCard, player)).ShowDialog() == true)
                {
                    BoundGameState.AcceptCandidate(player);
                    return;
                }
            }

            // Otherwise, offer each player to choose any suit
            foreach (Player player in BoundGameState.Players)
            {
                AnyTrumpOfferPrompt trumpPrompt = new(player);

                if (trumpPrompt.ShowDialog() == true)
                {
                    BoundGameState.AcceptCandidate(player, trumpPrompt.ChosenTrump);
                    return;
                }
            }

            // If no player chooses a suit, reshuffle and retry
            BoundGameState.Deck.ReShuffle();
            PromptTrumpOffer();
        }

        /// <summary>
        /// Displays hand win dialog for the provided player, then moves to next hand
        /// </summary>
        private void EndHand(Player winner)
        {
            if (MessageBox.Show($"{winner.Name} won that hand! would you like to play again?", "Another hand?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                BoundGameState.NextHand();
                NextHand();
                return;
            }
            Navigator.Navigate(new MainMenuPage());
        }
        
        /// <summary>
        /// Updates trump display to reflect current trump.
        /// </summary>
        private void SetTrumpUI()
        {
            TrumpDisplay.SetDisplay(BoundGameState.TrumpSuit);
        }

        #endregion

        #region Animation

        /// <summary>
        /// Moves deck to center of screen.
        /// </summary> 
        private async Task CenterDeck()
        {
            await BoundGameState.Deck.Center(GetCenter());
        }

        /// <summary>
        /// Determines the x and y coordinates of the center of TrickContainer.
        /// </summary>
        /// <returns>X and Y coordinates of the center of TrickContainer</returns>
        private Point GetCenter()
        {
            return TrickContainer.TranslatePoint(new Point(0, 0), PlayArea);
        }

        #endregion

        #region Event Handler Methods

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            OptionsScreen options = new OptionsScreen(BoundGameState);
            options.Owner = Window.GetWindow(this);
            options.ShowDialog();
        }

        /// <summary>
        /// Continues to game area setup once page is loaded.
        /// </summary>
        /// <param name="sender">The page.</param>
        /// <param name="e">Args.</param>
        private void PlayScreenPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetupGameArea();
            BoundGameState.StartGame();
            NextHand();
        }

        #endregion
    }
}

#endregion
