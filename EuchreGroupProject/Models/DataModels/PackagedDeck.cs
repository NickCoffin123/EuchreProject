
namespace EuchreGroupProject.Models
{

    /// <summary>
    /// Represents a deck of cards in a packaged format for serialization.
    /// </summary>
    public class PackagedDeck
    {
        // a list representing the cards in the deck as packaged cards
        public List<PackagedCard> Cards { get; set; }

        /// <summary>
        /// Creates a PackagedDeck from a Deck object.
        /// </summary>
        /// <param name="deck"></param>
        /// <returns></returns>
        public static PackagedDeck FromDeck(Deck deck)
        {
            return new PackagedDeck
            {
                Cards = deck.Cards.Select(PackagedCard.FromCard).ToList()
            };
        }

        /// <summary>
        /// Converts the PackagedDeck back to a Deck object.
        /// </summary>
        /// <returns></returns>
        public Deck ToDeck()
        {
            var deck = new Deck();

            // Clear UI and re-init deck manually to avoid duplicating default setup
            deck.Cards.Clear();
            deck.DeckContainer.Children.Clear(); // Reset UI side

            foreach (var packagedCard in Cards)
            {
                var card = packagedCard.ToCard();
                deck.Cards.Push(card);
                // Not calling AddToUI since it depends on WPF visuals; avoid or stub if needed
            }

            return deck;
        }
    }
}
