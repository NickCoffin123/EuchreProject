// A control for the Trump Offer page that will display all trumps to the player to choose from.
using System.Windows;

namespace EuchreGroupProject.Windows.Pages 
{
    /// <summary>
    /// Allows user to order up or pass a trump card.
    /// </summary>
    public partial class AnyTrumpOfferPrompt : Window 
    {
        #region Instance Properties

        public Card.Suit ChosenTrump;
        public Player Maker { get; set; } = new Player();

        #endregion

        /// <summary>
        /// Displays trump offer in UI.
        /// </summary>
        public AnyTrumpOfferPrompt(Player player) 
        {
            InitializeComponent();
            Maker = player;
            PromptText.Text = $"{player.Name}, hoose from any of the trumps below...";
        }

        #region Event Handler Methods

        /// <summary>
        /// Exits with false dialog result indicating trump was not accepted.
        /// </summary>
        /// <param name="sender">Pass button.</param>
        /// <param name="e">Args.</param>
        private void PassButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }



        #endregion

        #region Event Handlers

        private void ClubsContainer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChosenTrump = Card.Suit.Clubs;
            DialogResult = true;
            Close();
        }

        private void DiamondsContainer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChosenTrump = Card.Suit.Diamonds;
            DialogResult = true;
            Close();
        }

        private void SpadesContainer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChosenTrump = Card.Suit.Spades;
            DialogResult = true;
            Close();
        }

        private void HeartsContainer_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChosenTrump = Card.Suit.Hearts;
            DialogResult = true;
            Close();
        }


        private void ShowCardsButton_Click(object sender, RoutedEventArgs e)
        {
            Maker.OnShowCardsChanged();
        }

        #endregion

    }
}
