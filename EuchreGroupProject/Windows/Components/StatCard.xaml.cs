using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;

namespace EuchreGroupProject.Windows.Components
{
    /// <summary>
    /// Interaction logic for StatCard.xaml
    /// </summary>
    public partial class StatCard : Border
    {
        #region Events + Delegate defs

        public delegate void ShowCardsEventHandler();
        public event ShowCardsEventHandler? ShowCards;

        #endregion

        #region Constants - Statistic Labels

        private const string CurrentHandScoreText = "Current hand score:";
        private const string CurrentHandTricksWonText = "Current hand tricks won:";
        private const string TotalTricksWonText = "Total tricks won:";
        private const string TotalHandsWonText = "Total hands won:";
        private const string WinRateText = "Win rate:";
        private const string ShowCardsButtonText = "Show Cards";
        private const string HideCardsButtonText = "Hide Cards";

        #endregion

        #region Instance Properties

        private Card? _trickCard;
        private bool _isDealer;
        private int _currentHandScore;
        private int _currentHandTricksWon;
        private int _totalTricksWon;
        private int _totalHandsWon;

        /// <summary>
        /// True if cards are showing.
        /// </summary>
        private bool CardsShowing { get; set; }

        /// <summary>
        /// The hand associated with this player.
        /// </summary>
        public Hand Hand { get; set; } = new Hand();

        // All scores associated with this player
        public int CurrentHandScore
        {
            get => _currentHandScore;
            set
            {
                _currentHandScore = value;
                CurrentHandScoreDisplay.Text = $"{CurrentHandScoreText} {value}";
            }
        }
        public int CurrentHandTricksWon
        {
            get => _currentHandTricksWon;
            set
            {
                _currentHandTricksWon = value;
                CurrentHandTricksWonDisplay.Text = $"{CurrentHandTricksWonText} {value}";
            }
        }
        public int TotalTricksWon
        {
            get => _totalTricksWon;
            set
            {
                _totalTricksWon = value;
                TotalTricksWonDisplay.Text = $"{TotalTricksWonText} {value}";
            }
        }
        public int TotalHandsWon
        {
            get => _totalHandsWon;
            set
            {
                _totalHandsWon = value;
                TotalHandsWonDisplay.Text = $"{TotalHandsWonText} {value}";
            }
        }

        #endregion

        public StatCard()
        {
            InitializeComponent();
            ShowCards += ChangeShowCards;
        }

        #region Event handlers

        /// <summary>
        /// Invokes ShowCards event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowCardsButton_Click(object sender, RoutedEventArgs e)
        {
            ShowCards?.Invoke();
        }

        #endregion

        #region Instance Methods

        public void OnShowCardsChanged()
        {
            ShowCards?.Invoke();
        }

        public void ChangeShowCards()
        {
            CardsShowing = !CardsShowing;
            ShowCardsButton.Content = CardsShowing ? HideCardsButtonText : ShowCardsButtonText;
        }

        #endregion
    }
}
