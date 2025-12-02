namespace EuchreGroupProject.Models
{
    /// <summary>
    /// Represents a card in a packaged format for serialization.
    /// </summary>
    public class PackagedCard
    {
        public Card.Suit Suit { get; set; }
        public Card.Rank Rank { get; set; }

        /// <summary>
        /// Creates a PackagedCard from a Card object.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static PackagedCard FromCard(Card card)
        {
            return new PackagedCard
            {
                Suit = card.CurrentSuit,
                Rank = card.CurrentRank
            };
        }

        /// <summary>
        /// Converts the PackagedCard back to a Card object.
        /// </summary>
        /// <returns></returns>
        public Card ToCard()
        {
            return new Card(Suit, Rank);
        }
    }
}
