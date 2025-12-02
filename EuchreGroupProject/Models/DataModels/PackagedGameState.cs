using EuchreGroupProject.Models;

namespace EuchreGroupProject
{
    /// <summary>
    /// Represents the state of a game, including players, deck, and trick.
    /// </summary>
    public class PackagedGameState
    {
        #region Properties

        public int GameID { get; set; }
        public int CurrentRound { get; set; }
        public bool GameInProgress { get; set; }
        public Card.Suit? TrumpSuit { get; set; }
        public bool TrumpPhase { get; set; }

        public List<PackagedPlayer> Players { get; set; }
        public PackagedDeck Deck { get; set; }
        public PackagedTrick Trick { get; set; }
        #endregion

        /// <summary>
        /// Creates a PackagedGameState from a GameState object.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static PackagedGameState FromGameState(GameState game)
        {
            return new PackagedGameState
            {
                CurrentRound = game.CurrentRound,
                TrumpSuit = game.TrumpSuit,
                TrumpPhase = game.TrumpPhase,
                Players = game.Players.Select(PackagedPlayer.FromPlayer).ToList(),
                Deck = PackagedDeck.FromDeck(game.Deck),
                Trick = PackagedTrick.FromTrick(game.Trick),
                GameInProgress = game.GameInProgress
            };
        }

        /// <summary>
        /// Converts the PackagedGameState back to a GameState object.
        /// </summary>
        /// <returns></returns>
        public GameState ToGameState()
        {
            return new GameState
            {
                CurrentRound = this.CurrentRound,
                TrumpSuit = this.TrumpSuit,
                TrumpPhase = this.TrumpPhase,
                Players = this.Players.Select(p => p.ToPlayer()).ToList(),
                Deck = this.Deck.ToDeck(),
                Trick = this.Trick.ToTrick(),
                GameInProgress = this.GameInProgress
            };
        }
    }
}
