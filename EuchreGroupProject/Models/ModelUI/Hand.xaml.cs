/* 
    All general Hand logic and Hand specific UI logic is defined here.
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using EuchreGroupProject.StaticClasses;

#region Namespace Definition

namespace EuchreGroupProject
{
    /// <summary>
    /// A fully functional Hand class designed for Euchre gameplay - Inherits Border control as to allow for a simplistic graphical display.
    /// </summary>
    public partial class Hand : Border
    {
        #region Custom Delegates and Events

        public event Card.CardPlayedHandler? CardPlayed;

        #endregion

        #region Static Properties and Constants

        // This simply determines how spaced apart each card in the hand should be.
        private const int StartingCardMargin = 50;

        private const int CardsToGrab = Settings.TricksPerHand;

        #endregion

        #region Instance Properties

        private bool _showing;

        /// <summary>
        /// A collection of the Cards within this hand.
        /// Note an ObservableCollection is used such that it's CollectionChanged event can be subscribed to.
        /// This way UI is automatically updated whenever a card is added/removed from the hand.
        /// </summary>
        public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();  

        /// <summary>
        /// True if this Deck's cards are showing.
        /// </summary>
        public bool Showing
        {
            get => _showing;
            set
            {
                _showing = value;
                ShowCards();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor - Generates a random hand of Cards.
        /// </summary>
        public Hand()
        {
            InitializeComponent();
            Cards.CollectionChanged += UpdateHandUI;
        }

        /// <summary>
        /// Instantiates a Hand by taking 5 cards at random from the provided deck. Note that this modifies the Deck object, 
        /// removing 5 cards from its Stack of Cards.
        /// </summary>
        /// <param name="deck">The deck to pull cards from.</param>
        public Hand(Deck deck)
        {
            InitializeComponent();
            Cards.CollectionChanged += UpdateHandUI;
        }
        #endregion

        #region Instance Methods

        /// <summary>
        /// Subscribes to collection changed event of Cards and fills hand using a provided deck.
        /// </summary>
        /// <param name="deck">Deck to fill hand from.</param>
        public void SetHand(Deck deck)
        {
            FillHand(deck);
        }

        /// <summary>
        /// Updates UI to reflect current Cards collection.
        /// </summary>
        private void UpdateHandUI(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // Just clears prev display of cards and re adds. Margin is added so each are alligned in a styled way
            HandDisplay.Children.Clear();
            int currentCardMargin = StartingCardMargin;
            foreach (Card card in Cards)
            {
                card.HorizontalAlignment = HorizontalAlignment.Left;
                card.Margin = new Thickness(currentCardMargin, 0,0,0);
                HandDisplay.Children.Add(card);
                currentCardMargin += StartingCardMargin;
            }
        }

        /// <summary>
        /// Pops 5 cards from a stack of cards and adds to this hand.
        /// </summary>
        /// <param name="deck">Deck to pop cards from.</param>
        private void FillHand(Deck deck)
        {
            for (int i = 0; i < CardsToGrab; i++)
            {
                // Pop from deck's UI and Stack, and clear any prev delegates before subscribing new card played event.
                Add(deck);
            }
        }

        /// <summary>
        /// Shows or hides all cards in this hand.
        /// </summary>
        private void ShowCards()
        {
            foreach(Card card in Cards)
            {
                card.Showing = Showing;
            }
        }

        /// <summary>
        /// Removes the specified card from list of cards, as well as all of it's interactive elements.
        /// </summary>
        /// <param name="card">The card to remove</param>
        public void Remove(Card? card)
        {
            if (card == null) { return; }
            card.Margin = new Thickness(0,0,0,0);
            card.RevokeHand();
            Cards.Remove(card);
        }

        /// <summary>
        /// Adds top card of a given deck to this hand.
        /// </summary>
        /// <param name="deck">Deck to pull from.</param>
        public void Add(Deck deck)
        {
            Card cardToAdd = deck.UIPop();
            cardToAdd.AssignHand(CardPlayedInHand);
            cardToAdd.Showing = Showing;
            Cards.Add(cardToAdd);
        }
        #endregion

        #region Event Handler Methods/Delegates

        /// <summary>
        /// Invokes CardPlayed event on this hand, passing in the Card as an arg.
        /// All cards within the hand are subscribed to this event.
        /// </summary>
        /// <param name="cardPlayed">Card that was played.</param>
        /// <returns>Card that was played</returns>
        public void CardPlayedInHand(Card cardPlayed)
        {
            CardPlayed?.Invoke(cardPlayed);
        }

        #endregion

        #region Save/Load
        
        public dynamic Package() {
            List<dynamic> cards = new List<dynamic>();
            foreach (Card card in Cards) cards.Add(card.Package());
            return new {
                cards,
                Showing
            };
        }

        public void Unpackage(dynamic data) {
            Cards.Clear();
            foreach (dynamic card in data.cards) {
                Card newCard = new Card();
                newCard.Unpackage(card);
                Cards.Add(newCard);
            }
            Showing = data.Showing;
        }
        #endregion
    }
}

#endregion
