namespace EuchreGroupProject.Models
{
    /// <summary>
    /// Represents a trick in a packaged format for serialization.
    /// </summary>
    public class PackagedTrick
    {
        public PackagedCard? TopCard { get; set; }
        public PackagedCard? BottomCard { get; set; }

        /// <summary>
        /// Creates a PackagedTrick from a Trick object.
        /// </summary>
        /// <param name="trick"></param>
        /// <returns></returns>
        public static PackagedTrick FromTrick(Trick trick)
        {
            return new PackagedTrick
            {
                TopCard = trick.TopCard != null ? PackagedCard.FromCard(trick.TopCard) : null,
                BottomCard = trick.BottomCard != null ? PackagedCard.FromCard(trick.BottomCard) : null
            };
        }

        /// <summary>
        /// Converts the PackagedTrick back to a Trick object.
        /// </summary>
        /// <returns></returns>
        public Trick ToTrick()
        {
            return new Trick
            {
                TopCard = this.TopCard?.ToCard(),
                BottomCard = this.BottomCard?.ToCard()
            };
        }
    }
}
