/* 
    A functional, instantiatable player class for a standard Euchre game.    
*/
using System.Collections.ObjectModel;
using EuchreGroupProject.Windows.Components;

#region Namespace Definition

namespace EuchreGroupProject
{
    /// <summary>
    /// Player object with the added privilege of holding the Deck and Dealing.
    /// </summary>
    public class Player
    {  

        #region Static Properties and Constants

        private const string DefaultName = "Guest";

        #endregion

        #region Dialog Mapping

        /// <summary>
        /// Maps some key string to a dialog string. Note every dialog should always be prepended by the player name.
        /// </summary>
        private static Dictionary<string, string> DialogMapping = new Dictionary<string, string>()
        {
            { "DealerTrade", ", please select a card from your hand to discard in place of the trump." },
            { "PlayerTurn", ", it is your turn to play a card!" },
            { "PlayerWin", ", you won this trick!" },
            { "InvalidCard", ", you cannot play that card right now!" }
        };

        #endregion

        #region Custom Delegates and Events

        public delegate void TrickPlayedHandler(Card trickCard, Player player);
        public event TrickPlayedHandler? TrickPlayed;

        #endregion

        #region Instance Properties

        private Card? _trickCard;
        private string _name = "none";
        private bool _isDealer;
        private int _currentHandScore;
        private int _currentHandTricksWon;
        private int _totalTricksWon;
        private int _totalHandsWon;
        
        /// <summary>
        /// The hand associated with this player.
        /// </summary>
        public Hand Hand { get; set; } = new Hand();

        protected ObservableCollection<Card> PlayerHand => Hand.Cards;


        // All scores associated with this player
        public int CurrentHandScore
        {
            get => _currentHandScore;
            set
            {
                _currentHandScore = value;
                BoundStatCard.CurrentHandScore = value;
            }
        }
        public int CurrentHandTricksWon
        {
            get => _currentHandTricksWon;
            set
            {
                _currentHandTricksWon = value;
                TotalTricksWon++;
                BoundStatCard.CurrentHandTricksWon = value;
            }
        }
        public int TotalTricksWon
        {
            get => _totalTricksWon;
            set
            {
                _totalTricksWon = value;
                BoundStatCard.TotalTricksWon = value;
            }
        }
        public int TotalHandsWon
        {
            get => _totalHandsWon;
            set
            {
                _totalHandsWon = value;
                BoundStatCard.TotalHandsWon = value;
            }
        }

        /// <summary>
        /// The number of tricks taken by this player for a given game.
        /// </summary>
        public int TricksTaken { get; set; } = 0;

        /// <summary>
        /// True if player chooses trump for a given trick.
        /// </summary>
        public bool IsMaker { get; set; }

        /// <summary>
        /// True if it's this players turn.
        /// </summary>
        public bool IsTurn { get; set; }

        /// <summary>
        /// Name associated with this player.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                BoundStatCard.NameDisplay.Text = value;
            }
        }

        /// <summary>
        /// The card which the player has played for the current trick.
        /// Setter invokes TrickPlayed event only if it's this players turn, and the value isn't null
        /// </summary>
        public Card? TrickCard
        {
            get => _trickCard;
            set
            {
                if (value == null) { return; }
                _trickCard = value;
                TrickPlayed?.Invoke(value, this);
            }
        }

        /// <summary>
        /// True if this player is a dealer.
        /// Also swaps turns (Non dealer should go first).
        /// </summary>
        public bool IsDealer
        {
            get => _isDealer;
            set
            {
                IsTurn = !value;
                _isDealer = value;
            }
        }

        /// <summary>
        /// The auto updated stat card associated with this player.
        /// </summary>
        public StatCard BoundStatCard { get; set; } = new StatCard();

        /// <summary>
        /// The text for this player's current dialog.
        /// </summary>
        public string? Dialog
        {
            get { return BoundDialogContainer.Text; }
            set
            {
                if (value == null)
                {
                    BoundDialogContainer.Clear();
                    return;
                }
                BoundDialogContainer.Text = $"{Name}{DialogMapping[value]}";
            }
        }

        /// <summary>
        /// A dialog container for displaying messages related to this user.
        /// </summary>
        public Dialog BoundDialogContainer { get; set; } = new Dialog();

        #endregion

        #region Constructors

        public Player() { }

        /// <summary>
        /// Instantiates with an existing name.
        /// </summary>
        public Player(bool isDealer, string name)
        {
            Name = name;
            IsDealer = isDealer;
            BoundStatCard.ShowCards += ShowCards;
            if (this is not AIPlayer) { Hand.CardPlayed += (Card cardPlayed) => { TrickCard = cardPlayed; }; }
        }

        /// <summary>
        /// Instantiates with default name.
        /// </summary>
        public Player(bool isDealer)
        {
            Name = DefaultName;
            IsDealer = isDealer;
            BoundStatCard.ShowCards += ShowCards;
            if (this is not AIPlayer) { Hand.CardPlayed += (Card cardPlayed) => { TrickCard = cardPlayed; }; }
        }
        #endregion

        #region Instance Methods

        /// <summary>
        /// Sets hand for this player.
        /// </summary>
        /// <param name="deck">The deck to pull cards from.</param>
        public void SetHand(Deck deck)
        {
            Hand.SetHand(deck);
        }

        /// <summary>
        /// Adds card to player's hand. This will auto update UI.
        /// </summary>
        /// <param name="cardToAdd">The card to add.</param>
        public void PickupCard(Card cardToAdd)
        {
            Hand.Cards.Add(cardToAdd);
        }

        /// <summary>
        /// To be called on game end - updates score based on TricksTaken and IsMaker properties.
        /// </summary>
        public void UpdateScore()
        {
            if (IsMaker)
            {
                UpdateMakerScore();
                return;
            }
            UpdateDefenderScore();
        }

        /// <summary>
        /// Scores 1 point for taking 3 tricks and 2 points for taking all 5 tricks.
        /// </summary>
        public void UpdateMakerScore()
        {
            if (CurrentHandTricksWon >= 5) { CurrentHandScore += 2; }
            else if (CurrentHandTricksWon >= 3) { CurrentHandScore += 1; }
        }

        /// <summary>
        /// Defender always scores 2 points if they take more than 2 tricks (maker is Euchred).
        /// </summary>
        public void UpdateDefenderScore()
        {
            if (CurrentHandTricksWon >= 3) { CurrentHandScore += 2; }
        }

        /// <summary>
        /// Shows all player cards.
        /// </summary>
        private void ShowCards()
        {
            Hand.Showing = !Hand.Showing;
        }

        /// <summary>
        /// To be called when show card method needs to be invoked in stat card.
        /// </summary>
        public void OnShowCardsChanged()
        {
            BoundStatCard.OnShowCardsChanged();
        }

        /// <summary>
        /// Invokes trick played.
        /// </summary>
        /// <param name="trickPlayed">The trick to be played.</param>
        protected void OnTrickPlayed(Card trickPlayed)
        {
            TrickCard = trickPlayed;
        }

        #endregion

        #region Save/Load

        public dynamic Package() {
            List<dynamic> Hand = new List<dynamic>();
            foreach (Card card in PlayerHand) Hand.Add(card.Package());
            return new {
                Name, 
                Hand = Hand.Count > 0 ? Hand : new List<dynamic> { }, // might need to make this a List<Card> not sure how this will react
                IsDealer,
                IsMaker,
                IsTurn,
                TricksTaken,
                TrickCard = TrickCard != null ? TrickCard!.Package() : new Card().Package(),
                CurrentHandScore,
                CurrentHandTricksWon,
                TotalTricksWon,
                TotalHandsWon
            };
        }

        public void Unpackage(dynamic data) {
            Name = data.Name;
            IsDealer = data.IsDealer;
            IsMaker = data.IsMaker;
            IsTurn = data.IsTurn;
            TricksTaken = data.TricksTaken;
            CurrentHandScore = data.CurrentHandScore;
            CurrentHandTricksWon = data.CurrentHandTricksWon;
            TotalTricksWon = data.TotalTricksWon;
            TotalHandsWon = data.TotalHandsWon;
            foreach (dynamic card in data.Hand) {
                Card newCard = new Card();
                newCard.Unpackage(card);
                Hand.Cards.Add(newCard);
            }
        }
        #endregion
    }
}

#endregion
