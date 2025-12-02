/* 
    A simple static helper class for navigating between pages.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
#region Namespace Definition

namespace EuchreGroupProject.Windows
{
    /// <summary>
    /// Provides navigation among pages for a MainWindow object.
    /// </summary>
    static class Navigator
    {
        #region Static Properties

        public static MainWindow? CurrentMainWindow { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// Navigates the CurrentMainWindow to the provided page.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        public static void Navigate(Page page)
        {
            CurrentMainWindow?.PageNavigator.Navigate(page);
        }

        #endregion
    }
}

#endregion
