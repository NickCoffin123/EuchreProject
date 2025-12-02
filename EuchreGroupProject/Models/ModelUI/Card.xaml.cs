/* 
    All general Card logic and Card specific UI logic is defined here.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

#region Namespace Definition

namespace EuchreGroupProject
{
    /// <summary>
    /// A fully functional Card class designed for Euchre gameplay - Inherits Border control as to allow for a simplistic graphical display.
    /// </summary>
    public partial class Card : Border
    {
        #region Custom Delegates + Events

        public delegate void CardPlayedHandler(Card playedCard);
        public event CardPlayedHandler? CardPlayed;

        #endregion

        #region Static Properties and Constants

        public const Suit DefaultSuit = Suit.Spades;
        public const Rank DefaultRank = Rank.Ace;
        public const string SuitUri = "pack://application:,,,/Resources/Suits";
        private static DoubleAnimation HoverAnimation { get; set; } = new DoubleAnimation
        {
            To = -15,
            Duration = TimeSpan.FromSeconds(0.2),
        };
        private static DoubleAnimation HoverStopAnimation { get; set; } = new DoubleAnimation
        {
            To = 0,
            Duration = TimeSpan.FromSeconds(0.2),
        };

        /// <summary>
        /// Maps a given suit enum to it's given Brush color value.
        /// </summary>
        public static Dictionary<Suit, Brush> SuitMappings { get; set; } = new Dictionary<Suit, Brush>()
        {
            { Suit.Spades, Brushes.Black },
            { Suit.Hearts, Brushes.Red },
            { Suit.Diamonds, Brushes.Red },
            { Suit.Clubs, Brushes.Black },

        };

        /// <summary>
        /// Maps a given rank enum to it's respective string display value.
        /// </summary>
        public static Dictionary<Rank, string> RankMappings { get; set; } = new Dictionary<Rank, string>()
        {
            { Rank.Ace, "A" },
            { Rank.King, "K" },
            { Rank.Queen, "Q" },
            { Rank.Jack, "J" },
            { Rank.Ten, "10" },
            { Rank.Nine, "9" }
        };

        #endregion

        #region Enum Definitions

        /// <summary>
        /// A playing card suit of either Spades, Hearts, Diamonds, or Clubs.
        /// </summary>
        public enum Suit
        {
            Spades,
            Hearts,
            Diamonds,
            Clubs
        }

        /// <summary>
        /// A playing card rank specific to a standard Euchre game - Ace, King, Queen, Jack, Ten or Nine.
        /// </summary>
        public enum Rank
        {
            Ace,
            King,
            Queen,
            Jack,
            Ten,
            Nine
        }

        #endregion

        #region Instance Properties

        private Suit _currentSuit;
        private Rank _currentRank;
        private bool _showing = false;

        /// <summary>
        /// The suit currently associated with this card.
        /// </summary>
        public Suit CurrentSuit
        {
            get => _currentSuit;
            set
            {
                _currentSuit = value;
                UpdateSuitUI();

            }
        }

        /// <summary>
        /// The rank currently associated with this card.
        /// </summary>
        public Rank CurrentRank
        {
            get => _currentRank;
            set
            {
                _currentRank = value;
                UpdateRankUI();

            }
        }

        /// <summary>
        /// The current Uri from which this suit's png display should be grabbed.
        /// </summary>
        public BitmapImage CurrentSuitUri { get; set; } = new BitmapImage();

        /// <summary>
        /// Getter returns true if this card is front facing. Setter updates this cards UI to reflect flip.
        /// </summary>
        public bool Showing
        {
            get => _showing;
            set
            {
                _showing = value;
                FlipCard();
            }
        }

        /// <summary>
        /// Set to true if this card has a Hand attached to it.
        /// </summary>
        private bool HasHand { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a Card object and deploys it's UI based on the provided playing card Suit and Rank (See Card.Suit and Card.Rank enums for available choices).
        /// </summary>
        /// <param name="suit">Spades, Hearts, Diamonds or Clubs.</param>
        /// <param name="rank">Ace, King, Queen, Jack, Ten (10), or Nine (9).</param>
        public Card(Suit suit, Rank rank)
        {
            InitializeComponent();
            SetCard(suit, rank);
            Showing = false;

        }

        /// <summary>
        /// Instantiates a Card object and deploys it's UI based on default playing card Suit and Rank (See Card.DefaultSuit and Card.DefaultRank).
        /// </summary>
        public Card()
        {
            InitializeComponent();
            SetCard(DefaultSuit, DefaultRank);
        }
        #endregion

        #region Instance Methods

        /// <summary>
        /// Returns true if this card counts as trump, including left bower logic.
        /// </summary>
        public bool IsTrump(Suit trump)
        {
            if (CurrentRank == Rank.Jack)
            {
                // Left bower (Jack of same color, different suit)
                if ((CurrentSuit == Suit.Spades && trump == Suit.Clubs) ||
                    (CurrentSuit == Suit.Clubs && trump == Suit.Spades) ||
                    (CurrentSuit == Suit.Hearts && trump == Suit.Diamonds) ||
                    (CurrentSuit == Suit.Diamonds && trump == Suit.Hearts))
                {
                    return true;
                }
            }

            return CurrentSuit == trump;
        }

        /// <summary>
        /// Returns an integer value representing Euchre strength of the card.
        /// Higher means stronger.
        /// </summary>
        public int GetEuchreValue(Suit trump)
        {
            if (CurrentRank == Rank.Jack)
            {
                if (CurrentSuit == trump)
                    return 18; // Right Bower

                if ((CurrentSuit == Suit.Spades && trump == Suit.Clubs) ||
                    (CurrentSuit == Suit.Clubs && trump == Suit.Spades) ||
                    (CurrentSuit == Suit.Hearts && trump == Suit.Diamonds) ||
                    (CurrentSuit == Suit.Diamonds && trump == Suit.Hearts))
                    return 17; // Left Bower
            }

            if (CurrentSuit == trump)
            {
                return CurrentRank switch
                {
                    Rank.Ace => 16,
                    Rank.King => 15,
                    Rank.Queen => 14,
                    Rank.Ten => 13,
                    Rank.Nine => 12,
                    _ => 11
                };
            }

            return CurrentRank switch
            {
                Rank.Ace => 10,
                Rank.King => 9,
                Rank.Queen => 8,
                Rank.Jack => 7,
                Rank.Ten => 6,
                Rank.Nine => 5,
                _ => 0
            };
        }

        /// <summary>
        /// Sets up this Card's UI based on provided suit and rank.
        /// </summary>
        /// <param name="suit">The suit to switch to.</param>
        /// <param name="rank">The rank to switch to.</param>
        private void SetCard(Suit suit, Rank rank)
        {
            CurrentSuit = suit;
            CurrentRank = rank;
        }

        /// <summary>
        /// Updates UI to reflect this Card's CurrentSuit.
        /// </summary>
        private void UpdateSuitUI()
        {
            CurrentSuitUri = new BitmapImage(new Uri($"{SuitUri}/{CurrentSuit.ToString()}.png"));
            TopLeftSuitDisplay.Source = CurrentSuitUri;
            MainSuitDisplay.Source = CurrentSuitUri;
            BottomRightSuitDisplay.Source = CurrentSuitUri;
        }

        /// <summary>
        /// Updates UI to reflect this Card's CurrentRank.
        /// </summary>
        private void UpdateRankUI()
        {
            TopLeftRankDisplay.Foreground = SuitMappings[CurrentSuit];
            TopLeftRankDisplay.Text = RankMappings[CurrentRank];
            BottomRightRankDisplay.Text = RankMappings[CurrentRank];
            BottomRightRankDisplay.Foreground = SuitMappings[CurrentSuit];
        }

        /// <summary>
        /// Updates UI such that back of card is showing if needed.
        /// </summary>
        private void FlipCard()
        {
            BackCardDisplay.Visibility = Showing ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// Clears CardPlayed delegates, subscribing the provided method.
        /// </summary>
        /// <param name="cardPlayed">Method this card's CardPlayed event should subscribe to.</param>
        public void AssignHand(CardPlayedHandler cardPlayed)
        {
            CardPlayed = null;
            CardPlayed += cardPlayed;
            HasHand = true;
        }

        /// <summary>
        /// Clears CardPlayed delegates, sets HasHand to false.
        /// </summary>
        public void RevokeHand()
        {
            CardPlayed = null;
            HasHand = false;
            Showing = true;
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Invokes a CardPlayed event.
        /// </summary>
        /// <param name="sender">The card object.</param>
        /// <param name="e">Mouse event args.</param>
        private void Card_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            CardPlayed?.Invoke((Card)sender);
        }

        /// <summary>
        /// Animates card to indicate mouse hover.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!HasHand) { return; }

            // Grab the TranslateTransform object, use it to animate hover
            ((TranslateTransform)RenderTransform).BeginAnimation(TranslateTransform.YProperty, HoverAnimation);
        }

        /// <summary>
        /// Animates card to indicate mouse hover end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!HasHand) { return; }

            // Grab the TranslateTransform object, use it to animate hover end
            ((TranslateTransform)RenderTransform).BeginAnimation(TranslateTransform.YProperty, HoverStopAnimation);
        }

        #endregion

        #region Save/Load

        public dynamic Package() {
            return new {
                _currentSuit,
                _currentRank,
            };
        }

        public void Unpackage(dynamic card) {
            CurrentSuit = card._currentSuit;
            CurrentRank = card._currentRank;
        }
        #endregion

    }
}

#endregion
