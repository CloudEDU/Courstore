using CloudEDU.Common;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Category : GlobalPage
    {
        /// <summary>
        /// The category courses
        /// </summary>
        private StoreData categoryCourses;
        /// <summary>
        /// The data category
        /// </summary>
        private List<GroupInfoList<Object>> dataCategory;
        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;
        /// <summary>
        /// The course DSQ
        /// </summary>
        private DataServiceQuery<COURSE_AVAIL> courseDsq = null;
        /// <summary>
        /// The record DSQ
        /// </summary>
        private DataServiceQuery<COURSE_RECO_AVAIL> recDsq = null;

        /// <summary>
        /// The category name
        /// </summary>
        string categoryName = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Category"/> class.
        /// </summary>
        public Category()
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
            categoryName = e.Parameter as string;
            Title.Text = Constants.UpperInitialChar(categoryName);
            loadingProgressRing.IsActive = true;

            /****** 这里是所有推荐课程的搜索 ***/
            if (categoryName.Equals("Recommendation"))
            {
                TaskFactory<IEnumerable<COURSE_AVAIL>> tfRec = new TaskFactory<IEnumerable<COURSE_AVAIL>>();

                IEnumerable<COURSE_AVAIL> recCourses = await tfRec.FromAsync(ctx.BeginExecute<COURSE_AVAIL>(
                    new Uri("/GetRecommendedCourses?customer_id=" + Constants.User.ID, UriKind.Relative), null, null),
                    iar => ctx.EndExecute<COURSE_AVAIL>(iar));


                categoryCourses = new StoreData();
                foreach (var c in recCourses)
                {
                    categoryCourses.AddCourse(Constants.CourseRec2Course(c));
                }
                dataCategory = categoryCourses.GetSingleGroupByCategoryTitle(categoryName);
                cvs1.Source = dataCategory;
                loadingProgressRing.IsActive = false;


                //courseDsq = (DataServiceQuery<COURSE_AVAIL>)(from course in ctx.COURSE_AVAIL
                //                                             where course.ID > 5
                //                                             select course);
                //courseDsq.BeginExecute(OnRecommendationMannualCoursesComplete, null);
            }
            else
            {

                if (Constants.CategoryNameList.Contains(categoryName))
                {
                    courseDsq = (DataServiceQuery<COURSE_AVAIL>)(from course_avail in ctx.COURSE_AVAIL
                                                                 where course_avail.CATE_NAME == categoryName
                                                                 select course_avail);
                    courseDsq.BeginExecute(OnCategoryCoursesComplete, null);
                }
                else
                {
                    recDsq = (DataServiceQuery<COURSE_RECO_AVAIL>)(from re in ctx.COURSE_RECO_AVAIL
                                                                   where re.RECO_TITLE == categoryName
                                                                   select re);
                    recDsq.BeginExecute(OnRecommendationCoursesComplete, null);
                }
            }
            UserProfileBt.DataContext = Constants.User;
        }

        /// <summary>
        /// Called when [recommendation courses complete].
        /// </summary>
        /// <param name="result">The result.</param>
        private async void OnRecommendationCoursesComplete(IAsyncResult result)
        {
            categoryCourses = new StoreData();
            try
            {
                IEnumerable<COURSE_RECO_AVAIL> courses = recDsq.EndExecute(result);
                foreach (var c in courses)
                {
                    categoryCourses.AddCourse(Constants.CourseRecAvail2Course(c));
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    dataCategory = categoryCourses.GetSingleGroupByCategoryTitle(categoryName);
                    cvs1.Source = dataCategory;
                    loadingProgressRing.IsActive = false;
                });
            }
            catch
            {
                ShowMessageDialog();
            }
        }

        /// <summary>
        /// DataServiceQuery callback method to refresh the UI.
        /// </summary>
        /// <param name="result">Async operation result.</param>
        private async void OnCategoryCoursesComplete(IAsyncResult result)
        {
            categoryCourses = new StoreData();
            try
            {
                IEnumerable<COURSE_AVAIL> courses = courseDsq.EndExecute(result);
                foreach (var c in courses)
                {
                    categoryCourses.AddCourse(Constants.CourseAvail2Course(c));
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        dataCategory = categoryCourses.GetSingleGroupByCategoryTitle(categoryName);
                        cvs1.Source = dataCategory;
                        loadingProgressRing.IsActive = false;
                    });
            }
            catch
            {
                ShowMessageDialog();
            }
        }

        /// <summary>
        /// Network Connection error MessageDialog.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private async void ShowMessageDialog(String msg = "No network has been fooooooound ")
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var messageDialog = new MessageDialog("No Network has been found!");
                    messageDialog.Commands.Add(new UICommand("Try Again", (command) =>
                    {
                        Frame.Navigate(typeof(Courstore));
                    }));
                    messageDialog.Commands.Add(new UICommand("Close"));
                    loadingProgressRing.IsActive = false;
                    await messageDialog.ShowAsync();
                }
                catch
                {
                    ShowMessageDialog();
                }
            });
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
        /// Invoked when a course within a category is clicked.
        /// </summary>
        /// <param name="sender">The GridView displaying the course clicked.</param>
        /// <param name="e">Event data that describes the course clicked.</param>
        private void Course_ItemClick(object sender, ItemClickEventArgs e)
        {
            Course course = (Course)e.ClickedItem;

            Frame.Navigate(typeof(CourseOverview), course);
        }

        /// <summary>
        /// Invoked when a category is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a category for the selected category.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            if (categoryName == "newest")
            {
                Frame.Navigate(typeof(CategoryForNewest));
            }
        }
        /// <summary>
        /// Handles the Click event of the UserProfileButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login.Profile));
        }

    }
}
