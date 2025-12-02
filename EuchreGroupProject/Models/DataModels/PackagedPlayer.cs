using System.Collections.ObjectModel;
namespace EuchreGroupProject.Models
{

    /// <summary>
    /// Represents a player in a packaged format for serialization.
    /// </summary>
    public class PackagedPlayer
    {

        #region Properties

        public string Name { get; set; }
        public List<PackagedCard> Cards { get; set; }        // ✅ Changed
        public bool IsDealer { get; set; }
        public bool IsMaker { get; set; }
        public bool IsTurn { get; set; }
        public int TricksTaken { get; set; }
        public PackagedCard? TrickCard { get; set; }         // ✅ Changed

        public int CurrentHandScore { get; set; }
        public int CurrentHandTricksWon { get; set; }
        public int TotalTricksWon { get; set; }
        public int TotalHandsWon { get; set; }
        #endregion

        /// <summary>
        /// Creates a PackagedPlayer from a Player object.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static PackagedPlayer FromPlayer(Player player)
        {
            return new PackagedPlayer
            {
                Name = player.Name,
                Cards = player.Hand.Cards.Select(PackagedCard.FromCard).ToList(),   // ✅ Convert to PackagedCard
                IsDealer = player.IsDealer,
                IsMaker = player.IsMaker,
                IsTurn = player.IsTurn,
                TricksTaken = player.TricksTaken,
                TrickCard = player.TrickCard != null ? PackagedCard.FromCard(player.TrickCard) : null,   // ✅ Convert to PackagedCard
                CurrentHandScore = player.CurrentHandScore,
                CurrentHandTricksWon = player.CurrentHandTricksWon,
                TotalTricksWon = player.TotalTricksWon,
                TotalHandsWon = player.TotalHandsWon
            };
        }

        /// <summary>
        /// Converts the PackagedPlayer back to a Player object.
        /// </summary>
        /// <returns></returns>
        public Player ToPlayer()
        {
            var player = new Player(isDealer: this.IsDealer, name: this.Name)
            {
                IsMaker = this.IsMaker,
                IsTurn = this.IsTurn,
                TricksTaken = this.TricksTaken,
                TrickCard = this.TrickCard?.ToCard(),   // ✅ Convert back
                CurrentHandScore = this.CurrentHandScore,
                CurrentHandTricksWon = this.CurrentHandTricksWon,
                TotalTricksWon = this.TotalTricksWon,
                TotalHandsWon = this.TotalHandsWon
            };

            player.Hand.Cards = new ObservableCollection<Card>(this.Cards.Select(c => c.ToCard()));   // ✅ Convert back
            return player;
        }
    }
}
