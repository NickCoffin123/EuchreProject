/* 
    Game Manager a static class used for handling GameState instances.
    All I/O related to a given GameState comes through here.
*/

#region Namespace Definition

using System.IO;
using System.Text.Json;

namespace EuchreGroupProject.StaticClasses
{
    static class GameManager
    {

        #region Constants

        private const string GameStateDirUri = "GameStates";
        private const string GameStateUri = $"{GameStateDirUri}/gamestate.json";

        #endregion

        /// <summary>
        /// The current gamestate loaded in memeory.
        /// </summary>
        public static GameState? CurrentGameState { get; set; }

        #region I/O

        /// <summary>
        /// Saves the current gamestate to memory and storage.
        /// <param name="game"></param>
        public static void SaveGameState(GameState game)
        {
            // Store in memory
            CurrentGameState = game;

            // Write to the dir
            EnsureDirExists();
            var packaged = PackagedGameState.FromGameState(game);
            string json = JsonSerializer.Serialize(packaged, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GameStateUri, json);
        }

        /// <summary>
        /// Loads the previous gamestate from storage.
        /// </summary>
        /// <returns>The last played gamestate.</returns>
        public static GameState? LoadGameState()
        {
            EnsureDirExists();
            
            // Ensure file exists
            if (!File.Exists(GameStateUri)) { return null; }

            string json = File.ReadAllText(GameStateUri);
            var packaged = JsonSerializer.Deserialize<PackagedGameState>(json);

            if (packaged == null)
            {
                CurrentGameState = null;
                return CurrentGameState;
            }

            CurrentGameState = packaged.ToGameState();
            return CurrentGameState;
        }

        /// <summary>
        /// Ensures the gamestate dir exists.
        /// </summary>
        private static void EnsureDirExists()
        {   
            // Ensure the dir exists
            if (!Directory.Exists(GameStateDirUri))
            {
                Directory.CreateDirectory(GameStateDirUri);
            }
        }

        #endregion


    }
}

#endregion
