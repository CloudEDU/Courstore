using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CloudEDU.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class GlobalPage : Page
    {
        /// <summary>
        /// The global application bar
        /// </summary>
        public static AppBar globalAppBar = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalPage"/> class.
        /// </summary>
        public GlobalPage()
        {
            // Create the global appbar
            globalAppBar = new AppBar();
            globalAppBar.Content = new AppbarContent();
            globalAppBar.Background = Application.Current.Resources["AddBarBackgroundBrush"] as SolidColorBrush;
            globalAppBar.Opened += globalAppBar_Opened;

            this.BottomAppBar = globalAppBar;
        }

        /// <summary>
        /// Handles the Loaded event of the globalAppBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        void globalAppBar_Loaded(object sender, RoutedEventArgs e)
        {
            globalAppBar.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        /// <summary>
        /// Globals the application bar_ opened.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void globalAppBar_Opened(object sender, object e)
        {
            globalAppBar.Focus(Windows.UI.Xaml.FocusState.Programmatic);
        }

        /// <summary>
        /// Invoked as an event handler to navigate backward in the page's associated
        /// <see cref="Frame" /> until it reaches the top of the navigation stack.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        protected virtual void GoHome(object sender, RoutedEventArgs e)
        {
            // Use the navigation frame to return to the topmost page
            if (this.Frame != null)
            {
                while (this.Frame.CanGoBack) this.Frame.GoBack();
            }
        }

        /// <summary>
        /// Invoked as an event handler to navigate backward in the navigation stack
        /// associated with this page's <see cref="Frame" />.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the
        /// event.</param>
        protected virtual void GoBack(object sender, RoutedEventArgs e)
        {
            // Use the navigation frame to return to the previous page
            if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
        }

        /// <summary>
        /// Invoked as an event handler to navigate forward in the navigation stack
        /// associated with this page's <see cref="Frame" />.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the
        /// event.</param>
        protected virtual void GoForward(object sender, RoutedEventArgs e)
        {
            // Use the navigation frame to move to the next page
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }
    }
}
