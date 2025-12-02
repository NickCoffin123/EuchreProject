/* 
    A Trick is simply a Border containing two cards; The dealer card (bottom) and the non dealer card (top).    
*/

using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;


#region Namespace Definition

namespace EuchreGroupProject
{
    /// <summary>
    /// A Border to be filled with two cards.
    /// </summary>
    public partial class Trick : Border {
        #region Animation Properties

        private const double GiveToWinnerAnimationSpan = 0.2;

        #endregion

        #region Instance Properties

        private Card? _topCard;
        private Card? _bottomCard;

        public Card? TopCard {
            get => _topCard;
            set {
                _topCard = value;
                AddTop(value);
                if (LeadCard == null) { LeadCard = value; }
            }
        }

        public Card? BottomCard {
            get => _bottomCard;
            set {
                _bottomCard = value;
                AddBottom(value);
                if (LeadCard == null) { LeadCard = value; }
            }
        }

        /// <summary>
        /// Returns the first card that was added to the trick.
        /// </summary>
        public Card? LeadCard { get; set; }

        #endregion

        #region Constructors

        public Trick() {
            InitializeComponent();
        }
        #endregion

        #region Instance Methods

        /// <summary>
        /// Adds a card to the top hand for this trick.
        /// </summary>
        private void AddTop(Card? card) {
            if (card == null) { return; }
            TopCardContainer.Children.Clear();
            TopCardContainer.Children.Add(card);
        }

        /// <summary>
        /// Adds a card to the bottom hand for this trick.
        /// </summary>
        private void AddBottom(Card? card) {
            if (card == null) { return; }
            BottomCardContainer.Children.Clear();
            BottomCardContainer.Children.Add(card);
        }

        /// <summary>
        /// Clears all cards from this trick.
        /// </summary>
        public void Clear() {
            TopCardContainer.Children.Clear();
            BottomCardContainer.Children.Clear();
        }

        #endregion

        #region Instance Methods - Animation

        /// <summary>
        /// Shrinks this trick and moves it to the specified winner.
        /// </summary>
        /// <param name="winner">The winner of this trick.</param>
        public async Task GiveToWinner(Player winner) {
            // Animation setup
            DoubleAnimation yAnimate = new DoubleAnimation() {
                Duration = TimeSpan.FromSeconds(GiveToWinnerAnimationSpan),
                To = (PointFromScreen(new Point(0, 0)).Y > winner.Hand.PointFromScreen(new Point(0, 0)).Y) ? Height : -Height
            };

            // Peform animation, delay until it finishes
            ((TranslateTransform)RenderTransform).BeginAnimation(TranslateTransform.YProperty, yAnimate);
            await Task.Delay((int)(GiveToWinnerAnimationSpan * 1000));
        }

        #endregion

        #region Save/Load

        public dynamic Package() {
            return new {
                TopCard = TopCard?.Package(),
                BottomCard = BottomCard?.Package(),
                LeadCard = LeadCard?.Package()
            };
        }

        public void Unpackage(dynamic data) {
            //// Clear the current stack
            //TopCardContainer.Children.Clear();
            //BottomCardContainer.Children.Clear();


            // Unpackage the cards
            if (data.TopCard != null) {
                TopCard = new Card();
                TopCard.Unpackage(data.TopCard);
            }
            if (data.BottomCard != null) {
                BottomCard = new Card();
                BottomCard.Unpackage(data.BottomCard);
            }
            if (data.LeadCard != null) {
                LeadCard = new Card();
                LeadCard.Unpackage(data.LeadCard);
            }
        }
        #endregion
    }
}

#endregion