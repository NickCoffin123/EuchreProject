/*
    A simple Border class for displaying suits.
 */

using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;


namespace EuchreGroupProject.Windows.Components
{
    /// <summary>
    /// Interaction logic for SuitDisplay.xaml
    /// </summary>
    public partial class SuitDisplay : Border
    {
        public SuitDisplay(Card.Suit suit)
        {
            InitializeComponent();
            SetDisplay(suit);
        }

        public SuitDisplay()
        {
            InitializeComponent();
            SetDisplay(null);
        }

        /// <summary>
        /// Updates display image based on provided suit.
        /// </summary>
        /// <param name="suit">The suit to change to.</param>
        public void SetDisplay(Card.Suit? suit)
        {
            if (suit == null)
            {
                NoTrumpDisplay.Visibility = Visibility.Visible;
                SuitImageDisplay.Visibility = Visibility.Hidden;
                return;
            }
            SuitImageDisplay.Source = new BitmapImage(new Uri($"{Card.SuitUri}/{suit.ToString()}.png"));
            NoTrumpDisplay.Visibility = Visibility.Hidden;
            SuitImageDisplay.Visibility = Visibility.Visible;
        }
    }
}
