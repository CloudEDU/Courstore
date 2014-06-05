using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CloudEDU.Common
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class AppbarContent : UserControl
    {
        /// <summary>
        /// The search text block
        /// </summary>
        public static TextBox searchTextBlock = null;
        /// <summary>
        /// The advance search button
        /// </summary>
        public Button advanceSearchButton = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppbarContent"/> class.
        /// </summary>
        public AppbarContent()
        {
            this.InitializeComponent();

            searchTextBlock = searchBox;
            this.advanceSearchButton = AdvanceSearchButton;
        }

        /// <summary>
        /// AppBar Button used to navigate to Courstore Page.
        /// </summary>
        /// <param name="sender">The Courstore button clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void CourstoreButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(CourseStore.Courstore));
        }

        /// <summary>
        /// AppBar Button used to navigate to MyCourses Page.
        /// </summary>
        /// <param name="sender">My Courses button clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void MyCoursesButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(CourseStore.MyCourses));
        }

        /// <summary>
        /// AppBar Button used to navigate to Uploading Page.
        /// </summary>
        /// <param name="sender">The Uploading Course button clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void UploadCourseButton_Click(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(Uploading));
        }

        /// <summary>
        /// Handles the 1 event of the LogoutButton_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void LogoutButton_Click_1(object sender, RoutedEventArgs e)
        {
            ((Frame)Window.Current.Content).Navigate(typeof(Login.LoginSel));
            string courseUplaodUri = "/AddDBLog?opr='Logout'&msg='" + Constants.User.NAME + "'";
            //ctx.UpdateObject(c);
            CloudEDUEntities ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
            try
            {
                TaskFactory<IEnumerable<bool>> tf = new TaskFactory<IEnumerable<bool>>();
                IEnumerable<bool> result = await tf.FromAsync(ctx.BeginExecute<bool>(new Uri(courseUplaodUri, UriKind.Relative), null, null), iar => ctx.EndExecute<bool>(iar));
            }
            catch
            {
            }
        }

        /// <summary>
        /// Handles the Clicked event of the Enter control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
        private void Enter_Clicked(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                string searchText = searchBox.Text;
                if (searchText != null && searchText.Length != 0)
                {
                    ((Frame)Window.Current.Content).Navigate(typeof(CourseStore.SearchResult), searchText);
                }
            }
        }
    }
}
