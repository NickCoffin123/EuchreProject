/* 
    All general Deck logic and Deck specific UI logic is defined here.
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
using System.Windows.Media.Animation;
using EuchreGroupProject.StaticClasses;

namespace EuchreGroupProject
{

    /// <summary>
    /// A fully functional Deck class designed for Euchre gameplay - Inherits Border control as to allow for a simplistic graphical display.
    /// </summary>
    public partial class Deck : Border
    {
        #region Static Properties and Constants

        private const int DeckSize = 24;
        private const double CardMargin = 1;
        private static Random RandomNumGen = new Random();

        #endregion

        #region Animation Definitions

        private static DoubleAnimation CenterAnimationX { get; set; } = new DoubleAnimation { Duration = TimeSpan.FromSeconds(0.5) };
        private static DoubleAnimation CenterAnimationY { get; set; } = new DoubleAnimation { Duration = TimeSpan.FromSeconds(0.5) };
        private static DoubleAnimation ReturnAnimation { get; set; } = new DoubleAnimation
        {
            To = 0,
            Duration = TimeSpan.FromSeconds(0.5),
        };

        #endregion

        #region Instance Properties

        /// <summary>
        /// The collection of Card objects associated with this Deck.
        /// </summary>
        public Stack<Card> Cards { get; set; } = new Stack<Card>();

        /// <summary>
        /// Read only; returns top card from deck without removing.
        /// </summary>
        public Card TopCard
        {
            get => Cards.Peek();
        }
        /// <summary>
        /// CardMargin should be added to this property each time a new card is added to UI.
        /// </summary>
        private double CurrentCardMargin { get; set; } = CardMargin;

        /// <summary>
        /// 1 should be added to this property each time a new card is added to UI.
        /// </summary>
        private int CurrentCardZIndex { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new Deck object with a shuffles Stack of 24 Cards within the rankings allowed in a standard game of Euchre.
        /// </summary>
        public Deck()
        {
            InitializeComponent();
            Shuffle();
        }
        #endregion

        #region Instance Methods - Cards Stack

        /// <summary>
        /// Shuffles stack of Cards, and populates it if its empty.
        /// </summary>
        public void Shuffle()
        {
            // Populate if needed, then shuffle
            if (Cards.Count < 1) { PopulateStack(); }

            // Generate random index for each card - ensure none are overwritten
            int[] cardIndices = Enumerable.Repeat(DeckSize+1, DeckSize).ToArray();
            List<Card> newCards = Cards.ToList();
            for (int i = 0; i < DeckSize; i++)
            {
                int cardIndex = RandomNumGen.Next(0, DeckSize);
                while (cardIndices.Contains(cardIndex))
                {
                    cardIndex = RandomNumGen.Next(0, DeckSize);
                }
                cardIndices[i] = cardIndex;
                newCards[cardIndex] = Cards.Pop();
            }

            // Update stack
            Cards = new Stack<Card>(newCards);
        }
        
        /// <summary>
        /// Fills stack with 24 euchre ranking cards (Ranks J - 9).
        /// </summary>
        private void PopulateStack()
        {
            // Just return if already populated
            if (Cards.Count >= DeckSize) { return; }

            // Iterate through each rank, generating a card for each suit
            foreach (KeyValuePair<Card.Rank, string> ranking in Card.RankMappings)
            {
                foreach (KeyValuePair<Card.Suit, Brush> suit in Card.SuitMappings)
                {
                    // Add the card so long as its exact rank and suit does not already exist in cards
                    if (!CheckCard(ranking.Key, suit.Key))
                    {
                        Card currentCard = new Card(suit.Key, ranking.Key);
                        Cards.Push(currentCard);
                        AddToUI(currentCard);
                    }
                }

                // Break if limit is prematurely reached
                if (Cards.Count >= DeckSize) { break; }
            }
        }

        /// <summary>
        /// Updates UI to display this Deck object's top card.
        /// </summary>
        public void FlipTopCard()
        {
            // Ensure the Card Stack is not at 0 before flipping
            if (Cards.Count < 1) { throw new Exception("A Deck Flip was attempted on Card Stack with a count less than 1."); }

            // Ensure top stack card is at top of UI , then show
            Card card = Cards.Peek();
            DeckContainer.Children.Remove(card);
            DeckContainer.Children.Add(card);
            card.Showing = true;   
        }
 
        /// <summary>
        /// Simply adds the provided card to the deck UI.
        /// </summary>
        /// <param name="cardToAdd"></param>
        private void AddToUI(Card cardToAdd)
        {
            cardToAdd.Margin = new Thickness(CurrentCardMargin, CurrentCardMargin, 0, 0);
            DeckContainer.Children.Add(cardToAdd);
            CurrentCardMargin += CardMargin;
        }

        /// <summary>
        /// Pops the top card from both the UI and the stack.
        /// NOTE: It is crucial that the card is removed from this Decks UI before it is used in another UI area (e.g. in a Hand); an error will be thrown otherwise.
        /// </summary>
        /// <returns>The top card of this deck.</returns>
        public Card UIPop()
        {
            // Re populate and shuffle deck automatically
            if (Cards.Count < 1)
            {
                Cards.Clear();
                Shuffle();
            }

            Card topCard = Cards.Pop();
            DeckContainer.Children.Remove(topCard);
            return topCard;
        }

        /// <summary>
        /// Clears UI and reshuffles stack.
        /// </summary>
        public void ReShuffle()
        {
            CurrentCardMargin = CardMargin;
            Cards.Clear();
            DeckContainer.Children.Clear();
            Shuffle();
        }

        #endregion

        #region Instance Methods - Search

        /// <summary>
        /// Checks if a card already exists in the stack of cards by a given rank and suit.
        /// </summary>
        /// <param name="rank">The rank of the card to search for.</param>
        /// <param name="suit">The suit of the card to search for.</param>
        /// <returns></returns>
        private bool CheckCard(Card.Rank rank, Card.Suit suit)
        {
            foreach (Card card in Cards) { if (card.CurrentRank == rank && card.CurrentSuit == suit) return true; }
            return false;
        }

        #endregion

        #region Instance Methods - Animation

        /// <summary>
        /// Animates deck move to provided coordinates.
        /// </summary>
        /// <param name="center">X and Y coordinates to move to.</param>
        public Task Center(Point center)
        {
            CenterAnimationX.To = center.X;
            CenterAnimationY.To = center.Y;
            ((TranslateTransform)RenderTransform).BeginAnimation(TranslateTransform.XProperty, CenterAnimationX);
            ((TranslateTransform)RenderTransform).BeginAnimation(TranslateTransform.YProperty, CenterAnimationY);
            return Task.Delay(500);
        }

        /// <summary>
        /// Animates return to initial position.
        /// </summary>
        public void Return()
        {
            ((TranslateTransform)RenderTransform).BeginAnimation(TranslateTransform.XProperty, ReturnAnimation);
            ((TranslateTransform)RenderTransform).BeginAnimation(TranslateTransform.YProperty, ReturnAnimation);
        }

        #endregion

        #region Save/Load

        public dynamic Package() {
            List<dynamic> cards = new List<dynamic>();
            foreach (Card card in Cards) cards.Add(card.Package());
            return new {
                cards
            };
        }

        public void Unpackage(dynamic data) {
            // Clear the current stack
            Cards.Clear();

            //DeckContainer.Children.Clear();
            //CurrentCardMargin = CardMargin;


            // Iterate through each card in the data and add it to the stack
            foreach (dynamic cardData in data.cards) {
                if (cardData == null) continue; 

                Card card = new Card();
                card.Unpackage(cardData);
                Cards.Push(card);
                //AddToUI(card);
            }
            // Update the UI to reflect the new stack
            PopulateStack();
        }
        #endregion

    }
}
