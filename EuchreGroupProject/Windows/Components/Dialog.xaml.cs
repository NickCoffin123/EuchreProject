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

namespace EuchreGroupProject.Windows.Components
{
    /// <summary>
    /// Interaction logic for Dialog.xaml
    /// </summary>
    public partial class Dialog : Border
    {
        #region Instance Properties

        /// <summary>
        /// Dialog display text.
        /// </summary>
        public string Text
        {
            get => DialogDisplay.Text;
            set
            {
                DialogDisplay.Text = value;
                Visibility = Visibility.Visible;
            }
        }

        #endregion

        public Dialog()
        {
            InitializeComponent();
        }

        #region Instance Methods

        /// <summary>
        /// Hides dialog.
        /// </summary>
        public void Clear()
        {
            Visibility = Visibility.Hidden;
        }

        #endregion
    }
}
