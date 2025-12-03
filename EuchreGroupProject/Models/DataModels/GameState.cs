/* 
   All game logic is encapsulated here, utilizing Deck, Hand, Card, and Player/Dealer classes to tie together a functional Euchre game.
*/

using EuchreGroupProject;
using EuchreGroupProject.StaticClasses;
using System.Text.Json.Serialization;
using System.Windows;

#region Namespace Definition

namespace EuchreGroupProject
{

    /// <summary>
    /// Euchre game logic, and basic I/O (For loading/saving games) interfacing is defined within a given instance of this class.
    /// </summary>
    public class GameState
    {
        #region Events and Delegate defs

        public delegate void NextPhaseEndHandler();
        public delegate void HandEndHandler(Player player);
        public event NextPhaseEndHandler? TrumpPhaseEnded;
        public event NextPhaseEndHandler? TrickEnded;
        public event HandEndHandler? HandEnded;

        #endregion

        #region Instance Properties

        private bool _trumpPhase;
        private bool _trickOver;

        /// <summary>
        /// Readonly; returns top card from deck as candidate trump.
        /// </summary>
        public Card CandidateCard
        {
            get => Deck.TopCard;
        } 

        /// <summary>
        /// List of each player.
        /// </summary>
        public List<Player> Players { get; set; }

        /// <summary>
        /// The current round for this hand, to be incremented at the end of each Trick.
        /// </summary>
        public int CurrentRound { get; set; }

        /// <summary>
        /// The trump suit for this hand.
        /// </summary>
        public Card.Suit? TrumpSuit { get; set; } = null;

        /// <summary>
        /// True if a player has yet to choose a trump.
        /// </summary>
        public bool TrumpPhase
        {
            get => _trumpPhase;
            set
            {
                _trumpPhase = value;
                if (!value) { TrumpPhaseEnded?.Invoke(); }
            }
        }

        /// <summary>
        /// The deck for this game.
        /// </summary>
        public Deck Deck { get; set; }
        
        /// <summary>
        /// The current trick (collection of all played cards.)
        /// </summary>
        public Trick Trick { get; set; }

        /// <summary>
        /// Returns the player whos is currently taking their turn.
        /// </summary>
        public Player CurrentTurnPlayer
        {
            get => FindCurrentTurnPlayer(); 
        }
        
        /// <summary>
        /// Set to true when a trick ends.
        /// </summary>
        public bool TrickOver
        {
            get => _trickOver;
            set
            {
                _trickOver = value;
                if (value) { EndTrick(); }
            }
        }

        /// <summary>
        /// Returns true if a game is currently in progress.
        /// </summary>
        public bool GameInProgress { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates the GameState.
        /// </summary>
        /// <param name="players">Players array.</param>
        public GameState(List<Player> players)
        {
            Players = players;
            Deck = new Deck();
            Trick = new Trick();
            CurrentRound = 0;
        }

        public GameState()
        {
            Players = new List<Player>();
            Deck = new Deck();
            Trick = new Trick();
            CurrentRound = 0;
        }

        public GameState(int id) {
            Players = new List<Player>();
            Deck = new Deck();
            Trick = new Trick();
            CurrentRound = 0;
        }
        #endregion

        #region Game Logic

        #region Trump

        /// <summary>
        /// Sets trump suit to candidate suit.
        /// </summary>
        /// <param name="maker">Player who chose the trump.</param>
        public void AcceptCandidate(Player maker)
        {
            maker.IsMaker = true;
            TrumpSuit = CandidateCard.CurrentSuit;
            Player dealer = GetDealer();
            dealer.TrickPlayed += HandleDealerTradeOffer;
            DialogChange("DealerTrade", dealer);

            if (dealer is AIPlayer)
            {
                ((AIPlayer)dealer).DiscardForTrump();
            }
        }

        /// <summary>
        /// Sets trump suit to a specified suit.
        /// </summary>
        /// <param name="maker">Player who chose the trump.</param>
        /// <param name="chosenSuit">The trump suit chosen by the player.</param>
        public void AcceptCandidate(Player maker, Card.Suit chosenSuit)
        {
            maker.IsMaker = true;
            TrumpSuit = chosenSuit;
            Player dealer = GetDealer();
            dealer.TrickPlayed += HandleDealerTradeOffer;
            DialogChange("DealerTrade", dealer);

            if (dealer is AIPlayer)
            {
                ((AIPlayer)dealer).DiscardForTrump();
            }
        }

        /// <summary>
        /// Called when a dealer has chosen a card to trade.
        /// If the card is valid to trade (isn't trump), then it is removed from the dealer's deck and the trump is added.
        /// </summary>
        /// <param name="cardOffered">The card offered in exchange for the candidate trump.</param>
        /// <param name="dealer">The dealer.</param>
        private void HandleDealerTradeOffer(Card cardOffered, Player dealer)
        {
            if (cardOffered == CandidateCard) { return; }
            dealer.Hand.Remove(cardOffered);
            dealer.Hand.Add(Deck);
            dealer.TrickPlayed -= HandleDealerTradeOffer;
            DialogChange(null, dealer);
            Deck.Return();
            TrumpPhase = false;
        }

        #endregion

        #region Turns

        /// <summary>
        /// Swaps dealers.
        /// </summary>
        private void SwapDealer()
        {
            if (Players.Count != 2) { return; }
            Players[0].IsDealer = !Players[0].IsDealer;
            Players[1].IsDealer = !Players[1].IsDealer;
        }

        /// <summary>
        /// Swaps player turns.
        /// </summary>
        private void SwapTurns()
        {
            if (Players.Count != 2 || TrickOver) { return; }
            Players[0].IsTurn = !Players[0].IsTurn;
            Players[1].IsTurn = !Players[1].IsTurn;
            NextTurn();
        }

        /// <summary>
        /// Searches for the player who is currently taking a turn. If none is found, it sets player 1's IsTurn to true and returns.
        /// </summary>
        /// <returns></returns>
        private Player FindCurrentTurnPlayer()
        {
            foreach (Player player in Players)
            {
                if (player.IsTurn) { return player; }
            }
            Players[0].IsTurn = true;
            return Players[0];
        }

        #endregion

        #region Core Gameplay

        /// <summary>
        /// Subscribes to TrickPlayed event for all players.
        /// </summary>
        /// <returns>A trump card offer.</returns>
        public void StartGame()
        {
            foreach (Player player in Players) { player.TrickPlayed += PlayTrick; }
            TrumpPhaseEnded += NextTurn;
        }

        /// <summary>
        /// Shows next turn dialog, gets next ai turn if needed.
        /// </summary>
        private async void NextTurn()
        {
            if (TrumpPhase) { return; }

            DialogChange("PlayerTurn", CurrentTurnPlayer);
            if (CurrentTurnPlayer is AIPlayer && TrumpSuit != null)
            {
                await Task.Delay(800);
                ((AIPlayer)CurrentTurnPlayer).MakePlay(Trick.LeadCard, (Card.Suit)TrumpSuit);
            }
        }

        /// <summary>
        /// Swaps dealers for next hand, resets hand level stats.
        /// </summary>
        public void NextHand()
        {
            foreach (Player player in Players) 
            { 
                player.CurrentHandTricksWon = 0;
                player.CurrentHandScore = 0;
                player.IsMaker = false;
            }
            SwapDealer();
        }

        /// <summary>
        /// Determines winner and clears trick.
        /// </summary>
        private async void EndTrick()
        {
            // Detrmine winner
            Player winner = DetermineTrickWinner(Trick.LeadCard, Players[0], Players[1]);

            // Informs player of win, waits a few moments
            winner.CurrentHandTricksWon++;
            DialogChange("PlayerWin", winner);
            await Task.Delay(800);

            // Animates trick collection and invokes event to inform ui.
            await Trick.GiveToWinner(winner);
            TrickEnded?.Invoke();

            // Checks for hand win
            if (Players[0].Hand.Cards.Count == 0 && Players[1].Hand.Cards.Count == 0)
            {
                DialogChange(null, FindCurrentTurnPlayer());
                HandEnded?.Invoke(GetHandWinner());
            }

            // Setup next trick
            TrickOver = false;

            //if (!winner.IsTurn) SwapTurns();
            SwapTurns();

            winner.IsTurn = true;
            foreach (Player player in Players)
            {
                if (player != winner) { player.IsTurn = false; }
            }
            NextTurn();

        }

        /// <summary>
        /// Updates UI to reflect new trick.
        /// </summary>
        /// <param name="trickCard">The trick played.</param>
        /// <param name="player">The player who played the trick.</param>
        private void PlayTrick(Card trickCard, Player player)
        {
            if (TrumpPhase || !player.IsTurn || TrickOver) { return; }
            if (!EnsureLegalMove(trickCard, player))
            {
                DialogChange("InvalidCard", player);
                return;
            }

            player.TricksTaken++;
            player.Hand.Remove(trickCard);

            // Add trick card to bottom if player 1, else top.
            if (Players.IndexOf(player) == 0) { Trick.BottomCard = trickCard; }
            else { Trick.TopCard = trickCard; }

            // Determine if trick is over and swap turns.
            TrickOver = (Trick.BottomCard != null && Trick.TopCard != null);
            SwapTurns();
        }

        /// <summary>
        /// Deals cards to all players.
        /// </summary>
        public void Deal()
        {
            foreach (Player player in Players) { player.SetHand(Deck); }
        }

        /// <summary>
        /// Checks to see which player won the trick.
        /// </summary>
        /// <param name="leadCard"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private Player DetermineTrickWinner(Card? leadCard, Player p1, Player p2)
        {
            if (leadCard == null) { return Players[0]; }

            var card1 = p1.TrickCard!;
            var card2 = p2.TrickCard!;

            var v1 = card1.GetEuchreValue(TrumpSuit!.Value);
            var v2 = card2.GetEuchreValue(TrumpSuit!.Value);

            if (card1.IsTrump(TrumpSuit.Value) && !card2.IsTrump(TrumpSuit.Value)) return p1;
            if (!card1.IsTrump(TrumpSuit.Value) && card2.IsTrump(TrumpSuit.Value)) return p2;

            if (card1.CurrentSuit == leadCard.CurrentSuit && card2.CurrentSuit != leadCard.CurrentSuit) return p1;
            if (card2.CurrentSuit == leadCard.CurrentSuit && card1.CurrentSuit != leadCard.CurrentSuit) return p2;

            return (v1 >= v2) ? p1 : p2;
        }

        /// <summary>
        /// Checks legality of a move.
        /// </summary>
        /// <param name="trickCard">The card to check.</param>
        /// <param name="player">The player to check.</param>
        /// <returns>True if move was legal.</returns>
        private bool EnsureLegalMove(Card trickCard, Player player)
        {
            if (Trick.LeadCard == null || trickCard.CurrentSuit == Trick.LeadCard.CurrentSuit) { return true; }
            foreach (Card card in player.Hand.Cards)
            {
                if (card.CurrentSuit == Trick.LeadCard.CurrentSuit)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the winner of the current hand or the first player.
        /// </summary>
        /// <returns>The winner of the hand.</returns>
        private Player GetHandWinner()
        {
            Player winner = Players[0];
            foreach (Player player in Players)
            {
                player.UpdateScore();
                if (player.CurrentHandScore > winner.CurrentHandScore) { winner = player; }
            }
            winner.TotalHandsWon++;
            return winner;
        }

        /// <summary>
        /// Returns the name of the current dealer.
        /// </summary>
        /// <returns>The name of the current dealer.</returns>
        public string GetDealerName()
        {
            return GetDealer().Name;
        }

        /// <summary>
        /// Returns the current dealer.
        /// </summary>
        /// <returns>The current dealer.</returns>
        public Player GetDealer()
        {
            foreach (Player player in Players) { if (player.IsDealer) { return player; } }
            return Players[0];
        }

        /// <summary>
        /// Instantiates a fresh trick and returns it.
        /// </summary>
        /// <returns>The new trick.</returns>
        public Trick CreateNewTrick()
        {
            Trick = new();
            return Trick;
        }

        #endregion

        #region Dialog

        /// <summary>
        /// Updates player dialog based on key.
        /// </summary>
        /// <param name="dialogKey">Key for dialog found in Player.DialogMapping</param>
        /// <param name="addDialogPlayer">The player to add dialog to.</param>
        private void DialogChange(string? dialogKey, Player addDialogPlayer)
        {
            // Remove existing dialog from all players
            foreach (Player player in Players) { player.Dialog = null; }
            addDialogPlayer.Dialog = dialogKey;
        }

        #endregion

        #endregion

    }
}

#endregion