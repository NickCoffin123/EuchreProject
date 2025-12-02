// A control for the Trump Offer page that will display a potential trump suit and allow the user to accept or decline the offer.
using System.Windows;

namespace EuchreGroupProject.Windows.Pages 
{
    /// <summary>
    /// Allows user to order up or pass a trump card.
    /// </summary>
    public partial class TrumpOfferPrompt : Window 
    {
        #region Constants

        private const string DealerPrompt = "Pick it up!";
        private const string NonDealerPrompt = "Order it up!";

        #endregion

        #region Instance Properties

        private Player Player { get; set; }

        #endregion

        /// <summary>
        /// Displays trump offer in UI.
        /// </summary>
        /// <param name="trumpOffer">The candidate trump.</param>
        /// <param name="player">The player being offered a trump.</param>
        public TrumpOfferPrompt(Card trumpOffer, Player player) 
        {
            InitializeComponent();
            OrderUpButton.Content = player.IsDealer ? DealerPrompt : NonDealerPrompt;
            Card candidateCard = new Card(trumpOffer.CurrentSuit, trumpOffer.CurrentRank);
            candidateCard.Showing = true;
            CardOfferContainer.Children.Add(candidateCard);
            Player = player;
            Header.Text = $"{player.Name}, would you like to choose this trump?";
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

        /// <summary>
        /// Exuts with true dialog result indicating trump offer was accepted.
        /// </summary>
        /// <param name="sender">Order up button.</param>
        /// <param name="e">Args.</param>
        private void OrderUpButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Shows player's hand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowHandButton_Click(object sender, RoutedEventArgs e)
        {
            Player.OnShowCardsChanged();
            ShowHandButton.Content = Player.Hand.Showing ? "Hide Hand" : "Show Hand";
        }

        #endregion

    }
}
