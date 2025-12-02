
namespace EuchreGroupProject
{
    /// <summary>
    /// AI Player object that inherits from Player.
    /// </summary>
    public class AIPlayer : Player
    {

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="isDealer"></param>
        /// <param name="name"></param>
        public AIPlayer(bool isDealer, string name) : base(isDealer, name) { }


        /// <summary>
        /// choose card based on legality and strength of card
        /// </summary>
        /// <param name="leadCard"></param>
        /// <param name="trumpSuit"></param>
        /// <returns></returns>
        public Card ChooseCard(Card? leadCard, Card.Suit trumpSuit)
        {
            var hand = PlayerHand;
            if (hand.Count < 1) { return new Card(); }

            if (leadCard == null)
            {
                return hand
                    .OrderBy(c => c.IsTrump(trumpSuit))
                    .ThenBy(c => c.GetEuchreValue(trumpSuit))
                    .First();
            }

            var followSuitCards = hand.Where(c => c.CurrentSuit == leadCard.CurrentSuit || (c.IsTrump(trumpSuit) && leadCard.IsTrump(trumpSuit))).ToList();

            if (followSuitCards.Any())
            {
                return followSuitCards.OrderBy(c => c.GetEuchreValue(trumpSuit)).First();
            }

            return hand
                .OrderBy(c => c.IsTrump(trumpSuit))
                .ThenBy(c => c.GetEuchreValue(trumpSuit))
                .First();
        }

        /// <summary>
        /// Make a play based on the lead card and trump suit.
        /// </summary>
        /// <param name="leadCard"></param>
        /// <param name="trumpSuit"></param>
        public void MakePlay(Card? leadCard, Card.Suit trumpSuit)
        {
            var cardToPlay = ChooseCard(leadCard, trumpSuit);
            OnTrickPlayed(cardToPlay);
        }


        /// <summary>
        /// Determine if the AI player should pick trump based on the number of trump cards in hand.
        /// </summary>
        /// <param name="proposedTrump"></param>
        /// <returns></returns>
        public bool ShouldPickTrump(Card.Suit proposedTrump)
        {
            return PlayerHand.Count(c => c.IsTrump(proposedTrump)) >= 2;
        }

        /// <summary>
        /// Discard the first card in hand for trump.
        /// </summary>
        public void DiscardForTrump()
        {
            OnTrickPlayed(Hand.Cards[0]);
        }

    }
}
