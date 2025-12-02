/* 
    Inherits border; serves as a TextBox with the addition of a placeholder property with border properties.
 */

using System.Windows.Controls;
using System.Windows;



namespace EuchreGroupProject.Windows.Components
{

    /// <summary>
    /// A TextBox with the addition of a PlaceHolder property.
    /// </summary>
    public partial class BetterTextBox : Border
    {
        #region Instance Properties

        /// <summary>
        /// Sets the text placeholder for this BetterTextBox.
        /// </summary>
        public string Placeholder
        {
            get => TextPlaceholder.Text;
            set => TextPlaceholder.Text = value;
        }

        /// <summary>
        /// Sets the text for this BetterTextBox.
        /// </summary>
        public string Text
        {
            get => TextInput.Text;
            set => TextInput.Text = value;
        }

        /// <summary>
        /// Gets and sets fontsize.
        /// </summary>
        public double FontSize
        {
            get => TextInput.FontSize;
            set
            {
                TextInput.FontSize = value;
                TextPlaceholder.FontSize = value;

            }
        }

        /// <summary>
        /// Gets and sets actual text style.
        /// </summary>
        public Style TextStyle
        {
            get => TextInput.Style;
            set => TextInput.Style = value;
        }

        /// <summary>
        /// Gets and sets placeholder text style.
        /// </summary>
        public Style PlaceholderStyle
        {
            get => TextInput.Style;
            set => TextPlaceholder.Style = value; 
        }

        /// <summary>
        /// Enables/disables the actual text property.
        /// </summary>
        public bool Enabled
        {
            get => TextInput.IsEnabled;
            set => TextInput.IsEnabled = value;
        }

        /// <summary>
        /// Returns text changed event of actual text property.
        /// </summary>
        public event TextChangedEventHandler TextChanged
        {
            add
            {
                TextInput.TextChanged += value;
            }
            remove
            {
                TextInput.TextChanged -= value;
            }
        }
        #endregion

        public BetterTextBox()
        {
            InitializeComponent();
        }

        #region Visibility

        /// <summary>
        /// Hides or shows placeholder depending on length of actual text string.
        /// </summary>
        private void ChangePlaceholderVisibility()
        {
            if (Text.Length > 0) { TextPlaceholder.Visibility = Visibility.Hidden; }
            else { TextPlaceholder.Visibility = Visibility.Visible; }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when actual text input changes.
        /// </summary>
        /// <param name="sender">TextInput.</param>
        /// <param name="e">Args.</param>
        private void TextInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangePlaceholderVisibility();
        }

        #endregion
    }
}
