using CloudEDU.Common;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyCourses : GlobalPage
    {
        //private static int TopoElementWidth = 140;
        //private static int TopoElementHeight = 60;

        /// <summary>
        /// The course data
        /// </summary>
        private StoreData courseData;
        /// <summary>
        /// The data category
        /// </summary>
        private List<GroupInfoList<Object>> dataCategory;
        /// <summary>
        /// All courses
        /// </summary>
        private List<Course> allCourses;

        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;
        /// <summary>
        /// The teach DSQ
        /// </summary>
        private DataServiceQuery<COURSE_AVAIL> teachDsq = null;


        /// <summary>
        /// Constructor, initialized the components.
        /// </summary>
        public MyCourses()
        {
            this.InitializeComponent();

            ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetAllTextBlock();
            allCourses = new List<Course>();
            loadingProgressRing.IsActive = true;

            var borders = from b in topograph.Children.OfType<Border>()
                          select b;

            List<string> courseNameList = new List<string>();
            List<CloudEDU.Common.Constants.CourseAvaiStates> courseStatesList = new List<Constants.CourseAvaiStates>();
            foreach (Border border in borders)
            {
                TextBlock textBlock = border.Child as TextBlock;
                string courseShowName = textBlock.Text.Replace("\n", " ");
                courseNameList.Add(textBlock.Text);
                courseStatesList.Add(Constants.DepCourse.GetCourseLearnedState(courseShowName));
            }
            SetAllCoursesStates(courseNameList, courseStatesList);


            SetCourseState("Compute\nArchitecture", Constants.CourseAvaiStates.NotLearnedDisable);

            try
            {
                teachDsq = (DataServiceQuery<COURSE_AVAIL>)(from course in ctx.COURSE_AVAIL
                                                            where course.TEACHER_NAME == Constants.User.NAME
                                                            select course);

                TaskFactory<IEnumerable<COURSE_AVAIL>> tf = new TaskFactory<IEnumerable<COURSE_AVAIL>>();

                IEnumerable<COURSE_AVAIL> attends = await tf.FromAsync(ctx.BeginExecute<COURSE_AVAIL>(
                    new Uri("/GetAllCoursesAttendedByCustomer?customer_id=" + Constants.User.ID, UriKind.Relative), null, null),
                    iar => ctx.EndExecute<COURSE_AVAIL>(iar));
                IEnumerable<COURSE_AVAIL> teaches = await tf.FromAsync(teachDsq.BeginExecute(null, null), iar => teachDsq.EndExecute(iar));

                DataServiceQuery<COURSE_AVAIL> allCoursesDsq = (DataServiceQuery<COURSE_AVAIL>)(from c in ctx.COURSE_AVAIL
                                                                                                select c);
                IEnumerable<COURSE_AVAIL> allCoursesEnum = await tf.FromAsync(allCoursesDsq.BeginExecute(null, null), iar => allCoursesDsq.EndExecute(iar));
                foreach (var c in allCoursesEnum)
                {
                    Course tmpCourse = Constants.CourseAvail2Course(c);
                    allCourses.Add(tmpCourse);
                }

                courseData = new StoreData();
                foreach (var c in attends)
                {
                    Course tmpCourse = Constants.CourseAvail2Course(c);
                    tmpCourse.IsBuy = true;
                    tmpCourse.IsTeach = false;
                    courseData.AddCourse(tmpCourse);
                }
                foreach (var c in teaches)
                {
                    Course tmpCourse = Constants.CourseAvail2Course(c);
                    tmpCourse.IsTeach = true;
                    tmpCourse.IsBuy = false;
                    courseData.AddCourse(tmpCourse);
                }

                dataCategory = courseData.GetGroupsByAttendingOrTeaching();
                cvs1.Source = dataCategory;
                UserProfileBt.DataContext = Constants.User;

            }
            catch
            {
                ShowMessageDialog("on navi to");
            }

            loadingProgressRing.IsActive = false;
        }

        /// <summary>
        /// Sets all text block.
        /// </summary>
        private void SetAllTextBlock()
        {
            var borders = from b in topograph.Children.OfType<Border>()
                          select b;
            foreach (Border border in borders)
            {
                border.Height = 40;
                border.Width = 110;
                border.HorizontalAlignment = HorizontalAlignment.Left;
                border.VerticalAlignment = VerticalAlignment.Top;
                border.BorderThickness = new Thickness(1);
                border.CornerRadius = new CornerRadius(20);
                border.BorderBrush = new SolidColorBrush(Colors.Black);

                TextBlock textBlock = border.Child as TextBlock;

                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
                textBlock.FontSize = 15;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Padding = new Thickness(3);
                textBlock.Tapped += CourseTextBlock_Tapped;
            }
        }

        /// <summary>
        /// Shows the message dialog.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private async void ShowMessageDialog(String msg = "No Network has been found!")
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var messageDialog = new MessageDialog(msg);
                messageDialog.Commands.Add(new UICommand("Try Again", (command) =>
                {
                    Frame.Navigate(typeof(Courstore));
                }));
                messageDialog.Commands.Add(new UICommand("Close"));
                //loadingProgressRing.IsActive = false;
                await messageDialog.ShowAsync();
            });
        }

        /// <summary>
        /// Called when [course seach complete].
        /// </summary>
        /// <param name="ar">The ar.</param>
        private void OnCourseSeachComplete(IAsyncResult ar)
        {
            IEnumerable<COURSE_AVAIL> courses = ctx.EndExecute<COURSE_AVAIL>(ar);
            foreach (COURSE_AVAIL c in courses)
            {
                System.Diagnostics.Debug.WriteLine(c.TITLE);
            }
        }

        /// <summary>
        /// Invoked when back button is clicked and return the last page.
        /// </summary>
        /// <param name="sender">The back button clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(Courstore));
            }
        }

        /// <summary>
        /// Invoked when a course within attending or teaching column is clicked.
        /// </summary>
        /// <param name="sender">The GridView displaying the course clicked.</param>
        /// <param name="e">Event data that describes the course clicked.</param>
        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Course course = (Course)e.ClickedItem;
            List<object> courseInfo = new List<object>();
            courseInfo.Add(course);

            if (course.IsTeach)
            {
                courseInfo.Add("teaching");
            }
            else
            {
                courseInfo.Add("attending");
            }

            Frame.Navigate(typeof(Coursing), courseInfo);
        }

        /// <summary>
        /// Handles the Click event of the UserProfileButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login.Profile));
        }

        /// <summary>
        /// Handles the Tapped event of the CourseTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TappedRoutedEventArgs" /> instance containing the event data.</param>
        private void CourseTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock courseNameBlock = sender as TextBlock;
            System.Diagnostics.Debug.WriteLine("name = " + courseNameBlock.Text);

            foreach (Course c in allCourses)
            {
                if (c.Title.ToLower().Equals(courseNameBlock.Text.Replace("\n", " ").ToLower()))
                {
                    Frame.Navigate(typeof(CourseOverview), c);
                }
            }

            //List<GroupInfoList<object>> list = courseData.GetSearchResultGroup(courseNameBlock.Text);
            //System.Diagnostics.Debug.WriteLine(list);

        }

        /// <summary>
        /// Handles the Tapped event of the Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TappedRoutedEventArgs" /> instance containing the event data.</param>
        private void Close_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CourseTopoPopup.IsOpen = false;
        }

        /// <summary>
        /// Handles the Click event of the Open_Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Open_Popup_Click(object sender, RoutedEventArgs e)
        {
            CourseTopoPopup.IsOpen = true;
        }

        /// <summary>
        /// Sets the state of the course.
        /// </summary>
        /// <param name="courseName">Name of the course.</param>
        /// <param name="state">The state.</param>
        private void SetCourseState(string courseName, CloudEDU.Common.Constants.CourseAvaiStates state)
        {
            var borders = from b in topograph.Children.OfType<Border>()
                          where (b.Child as TextBlock).Text.ToLower().Equals(courseName.ToLower())
                          select b;

            if (borders.Count() == 0) return;
            Border border = borders.FirstOrDefault();

            switch (state)
            {
                case Constants.CourseAvaiStates.Learned:
                    border.Background = new SolidColorBrush(Colors.Blue);
                    break;
                case Constants.CourseAvaiStates.NotLearnedButAvailable:
                    border.Background = new SolidColorBrush(Colors.Red);
                    break;
                case Constants.CourseAvaiStates.NotLearnedDisable:
                    border.Background = new SolidColorBrush(Colors.Gray);
                    break;
            }
        }

        /// <summary>
        /// Sets all courses states.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <param name="states">The states.</param>
        private void SetAllCoursesStates(IEnumerable<string> courses, IEnumerable<CloudEDU.Common.Constants.CourseAvaiStates> states)
        {
            for (int i = 0; i < courses.Count(); ++i)
            {
                string courseName = courses.ElementAtOrDefault(i);
                SetCourseState(courseName, states.ElementAtOrDefault(i));
            }
        }
    }
}
