using CloudEDU.Common;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MyCourses : GlobalPage
    {
        private static int TopoElementWidth = 140;
        private static int TopoElementHeight = 60;

        private StoreData courseData;
        private List<GroupInfoList<Object>> dataCategory;
        private List<Course> allCourses;

        private CloudEDUEntities ctx = null;
        private DataServiceQuery<COURSE_AVAIL> teachDsq = null;

        enum CourseAvaiStates
        {
            Finished,
            Learning,
            Disable,
        }

        private class Point
        {
            private int _x = 0;
            public int X
            {
                get
                {
                    return _x;
                }
                set
                {
                    if (this._x != value)
                    {
                        this._x = value;
                    }
                }
            }

            private int _y = 0;
            public int Y
            {
                get
                {
                    return _y;
                }
                set
                {
                    if (this._y != value)
                    {
                        this._y = value;
                    }
                }
            }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public Point(Point p)
            {
                X = p.X;
                Y = p.Y;
            }

            public static int PointDistance(Point p1, Point p2)
            {
                int distance = 0;

                if (p1.X == p2.X)
                {
                    distance = Math.Abs(p1.Y - p2.Y);
                }
                else if (p1.Y == p2.Y)
                {
                    distance = Math.Abs(p1.X - p2.X);
                }
                else
                {
                    distance = (int)Math.Floor(Math.Sqrt((p1.Y - p2.Y) * (p1.Y - p2.Y) + (p1.X - p2.X) * (p1.X - p2.X)));
                }
                return distance;
            }
        }

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
            //SetAllCourseElementStyle();
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


                courseData = new StoreData();
                foreach (var c in attends)
                {
                    Course tmpCourse = Constants.CourseAvail2Course(c);
                    tmpCourse.IsBuy = true;
                    tmpCourse.IsTeach = false;
                    courseData.AddCourse(tmpCourse);
                    allCourses.Add(tmpCourse);
                }
                foreach (var c in teaches)
                {
                    Course tmpCourse = Constants.CourseAvail2Course(c);
                    tmpCourse.IsTeach = true;
                    tmpCourse.IsBuy = false;
                    courseData.AddCourse(tmpCourse);
                    allCourses.Add(tmpCourse);
                }

                dataCategory = courseData.GetGroupsByAttendingOrTeaching();
                cvs1.Source = dataCategory;
                UserProfileBt.DataContext = Constants.User;

            }
            catch
            {
                ShowMessageDialog("on navi to");
            }
            
        }

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

                TextBlock textBlock = border.Child as TextBlock;

                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Foreground = new SolidColorBrush(Colors.Black);
                textBlock.FontSize = 13;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Padding = new Thickness(3);
                textBlock.Tapped += CourseTextBlock_Tapped;
            }
        }

       

        private void SetAllCourseElementStyle()
        {
            var borders = from border in topograph.Children.OfType<Border>()
                          select border;

            foreach (Border border in borders)
            {
                border.BorderThickness = new Thickness(3);
                border.CornerRadius = new CornerRadius(30);
                border.Width = TopoElementWidth;
                border.Height = TopoElementHeight;
                border.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top;
                border.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                BorderBrush = new SolidColorBrush(Colors.Black);

                TextBlock textBlock = border.Child as TextBlock;

                textBlock.FontSize = 20;
                textBlock.FontWeight = FontWeights.Bold;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center;
                textBlock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            }
        }

        private void AddCoursesToDefaultPosition()
        {
            AddCourseTopoElement(new Point(100, 100), "dskjdasdjal", CourseAvaiStates.Finished);
        }

        private void AddCourseTopoElement(Point point, string courseName, CourseAvaiStates state)
        {
            SolidColorBrush courseStateColor = new SolidColorBrush(Colors.Gray);
            if (state == CourseAvaiStates.Finished)
            {
                courseStateColor = new SolidColorBrush(Colors.Green);
            }
            else if (state == CourseAvaiStates.Learning)
            {
                courseStateColor = new SolidColorBrush(Colors.Red);
            }
            else if (state == CourseAvaiStates.Disable)
            {
                courseStateColor = new SolidColorBrush(Colors.Gray);
            }
            Border newBorder = new Border
            {
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(30),
                Width = TopoElementWidth,
                Height = TopoElementHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = courseStateColor,
                BorderBrush = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(point.X, point.Y, 0, 0)
            };

            TextBlock newText = new TextBlock
            {
                Text = courseName,
                FontSize = 30,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center
            };
            newBorder.Child = newText;
            topograph.Children.Add(newBorder);
        }

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

        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login.Profile));
        }

        private void CourseTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            TextBlock courseNameBlock = sender as TextBlock;
            System.Diagnostics.Debug.WriteLine("name = " + courseNameBlock.Text);

            foreach(Course c in allCourses){
                if(c.Title.Equals(courseNameBlock.Text)){
                    Frame.Navigate(typeof(CourseOverview), c);
                }
            }

            //List<GroupInfoList<object>> list = courseData.GetSearchResultGroup(courseNameBlock.Text);
            //System.Diagnostics.Debug.WriteLine(list);
            
        }

        private void Close_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CourseTopoPopup.IsOpen = false;
        }

        private void Open_Popup_Click(object sender, RoutedEventArgs e)
        {
            CourseTopoPopup.IsOpen = true;
        }
    }
}
