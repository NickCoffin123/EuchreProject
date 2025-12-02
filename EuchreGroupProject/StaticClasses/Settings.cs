// A static class to hold all the settings for the game

using System.Drawing;

namespace EuchreGroupProject.StaticClasses {
    internal static class Settings {

        #region Color Palette

        public static Color LAVENDAR  = Color.FromArgb(217, 219, 241);
        public static Color DARK_GREY = Color.FromArgb(208, 205, 215);
        public static Color TEAL = Color.FromArgb(0, 128, 128);
        public static Color LIGHT_GREY = Color.FromArgb(172, 176, 189);
        public static Color CORAL = Color.FromArgb(237, 106, 90);


        #endregion

        #region Gameplay

        public const int TricksPerHand = 5;
        public const int PlayersAllowed = 2;

        /// <summary>
        /// Number of milliseconds to pause in between animations
        /// </summary>
        public const int AnimationBuffer = 200;

        #endregion
    }
}
