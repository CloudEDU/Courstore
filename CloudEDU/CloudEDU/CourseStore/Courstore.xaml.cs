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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Courstore : GlobalPage
    {
        /// <summary>
        /// The courses data
        /// </summary>
        private StoreData coursesData;
        /// <summary>
        /// The data category
        /// </summary>
        private List<GroupInfoList<object>> dataCategory;
        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;
        /// <summary>
        /// The course DSQ
        /// </summary>
        private DataServiceQuery<COURSE_AVAIL> courseDsq = null;
        /// <summary>
        /// The category DSQ
        /// </summary>
        private DataServiceQuery<CATEGORY> categoryDsq = null;

        /// <summary>
        /// Constructor, initialize the components
        /// </summary>
        public Courstore()
        {
            this.InitializeComponent();
            ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));

            var appBarContent = globalAppBar.Content as AppbarContent;
            appBarContent.advanceSearchButton.Visibility = Visibility;
            appBarContent.advanceSearchButton.Click += AdvanceSearchButton_Click;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            InitializePopupStyle();


            //Constants.DepCourse.GetAllLearned();

            //await Task.Delay(TimeSpan.FromSeconds(10));

            //if (!Constants.DepCourse.IfLearned("Organic Chemistry"))
            //{
            //    System.Diagnostics.Debug.WriteLine("NONONONONONO");
            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine("YESYESYES");
            //}
            loadingProgressRing.IsActive = true;

            try
            {
                if (Constants.RecUriDic.Count == 0 && Constants.CategoryNameList.Count == 0)
                {



                    try
                    {
                        DataServiceQuery<CATEGORY> cateDsq = (DataServiceQuery<CATEGORY>)(from cate in ctx.CATEGORY
                                                                                          select cate);
                        TaskFactory<IEnumerable<CATEGORY>> tfc = new TaskFactory<IEnumerable<CATEGORY>>();
                        IEnumerable<CATEGORY> categories = await tfc.FromAsync(cateDsq.BeginExecute(null, null), iar => cateDsq.EndExecute(iar));
                        foreach (var c in categories)
                        {
                            Constants.CategoryNameList.Add(c.CATE_NAME);
                        }
                    }
                    catch
                    {
                        ShowMessageDialog("categories!");
                    }
                    try
                    {
                        DataServiceQuery<RECOMMENDATION> craDsq = (DataServiceQuery<RECOMMENDATION>)(from re in ctx.RECOMMENDATION
                                                                                                     select re);
                        TaskFactory<IEnumerable<RECOMMENDATION>> tf = new TaskFactory<IEnumerable<RECOMMENDATION>>();
                        IEnumerable<RECOMMENDATION> recommendation = await tf.FromAsync(craDsq.BeginExecute(null, null), iar => craDsq.EndExecute(iar));
                        foreach (var r in recommendation)
                        {
                            Constants.RecUriDic.Add(r.TITLE, r.ICON_URL);
                        }

                        Constants.RecUriDic.Add("Recommendation", "ms-appx:///Images/Category/Recommendation.jpg");
                    }
                    catch
                    {
                        ShowMessageDialog("recommedations!");
                    }






                }
            }
            catch
            {
                ShowMessageDialog("On nav to");
            }

            courseDsq = (DataServiceQuery<COURSE_AVAIL>)(from course_avail in ctx.COURSE_AVAIL select course_avail);
            courseDsq.BeginExecute(OnCourseAvailComplete, null);
            UserProfileBt.DataContext = Constants.User;
            //UserProfileBt.IsEnabled = false;


            Constants.DepCourse.GetAllLearned();
        }

        /// <summary>
        /// Initializes the popup style.
        /// </summary>
        private void InitializePopupStyle()
        {
            // 样式设置
            var grids = from g in AdvanceSearchStackPanel.Children.OfType<Grid>()
                        where !g.Name.Equals(searchButtonsGrid.Name)
                        select g;

            foreach (var grid in grids)
            {
                grid.Margin = new Thickness(10);
                grid.Height = 70;

                var filterTitles = from c in grid.Children.OfType<TextBlock>()
                                   select c;
                filterTitles.FirstOrDefault().FontSize = 50;

                var filterContent = from c in grid.Children.OfType<TextBox>()
                                    select c;
                if (filterContent != null && filterContent.Count() != 0)
                {
                    filterContent.FirstOrDefault().FontSize = 40;
                    filterContent.FirstOrDefault().Text = "";
                }
            }

            // Category内容设置
            categoryDsq = (DataServiceQuery<CATEGORY>)(from category in ctx.CATEGORY select category);
            categoryDsq.BeginExecute(OnCategoryComplete, null);

            SearchPopTitleText.KeyDown += AdvanceSearch_KeyDown;
            SearchPopAuthorText.KeyDown += AdvanceSearch_KeyDown;
            SearchPopDescriptionText.KeyDown += AdvanceSearch_KeyDown;
        }

        /// <summary>
        /// Called when [category complete].
        /// </summary>
        /// <param name="result">The result.</param>
        private async void OnCategoryComplete(IAsyncResult result)
        {
            try
            {
                IEnumerable<CATEGORY> cts = categoryDsq.EndExecute(result);
                List<CATEGORY> categories = new List<CATEGORY>();
                CATEGORY defaultCategory = new CATEGORY();
                defaultCategory.CATE_NAME = "Any Categories";
                categories.Add(defaultCategory);
                categories.AddRange(cts);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    categorySearchComboBox.ItemsSource = categories;
                    categorySearchComboBox.SelectedIndex = 0;
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
        private async void OnCourseAvailComplete(IAsyncResult result)
        {
            coursesData = new StoreData();

            try
            {
                /************************ 搜索推荐课程 *********************/
                TaskFactory<IEnumerable<COURSE_AVAIL>> tfRec = new TaskFactory<IEnumerable<COURSE_AVAIL>>();

                IEnumerable<COURSE_AVAIL> recCourses = await tfRec.FromAsync(ctx.BeginExecute<COURSE_AVAIL>(
                    new Uri("/GetRecommendedCourses?customer_id=" + Constants.User.ID, UriKind.Relative), null, null),
                    iar => ctx.EndExecute<COURSE_AVAIL>(iar));
                //DataServiceQuery<COURSE_AVAIL> recCourseDsq = (DataServiceQuery<COURSE_AVAIL>)(from rc in ctx.COURSE_AVAIL
                //                                                                               where rc.ID > 5
                //                                                                               select rc);
                //TaskFactory<IEnumerable<COURSE_AVAIL>> recTF = new TaskFactory<IEnumerable<COURSE_AVAIL>>();
                //IEnumerable<COURSE_AVAIL> recCourses = await recTF.FromAsync(recCourseDsq.BeginExecute(null, null), iar => recCourseDsq.EndExecute(iar));

                foreach (var c in recCourses)
                {
                    coursesData.AddCourse(Constants.CourseRec2Course(c));
                }
                /************************ 搜索推荐课程 *********************/
            }
            catch
            {
                coursesData = new StoreData();
            }


            try
            {
                DataServiceQuery<COURSE_RECO_AVAIL> craDsq = (DataServiceQuery<COURSE_RECO_AVAIL>)(from re in ctx.COURSE_RECO_AVAIL
                                                                                                   select re);
                TaskFactory<IEnumerable<COURSE_RECO_AVAIL>> tf = new TaskFactory<IEnumerable<COURSE_RECO_AVAIL>>();
                IEnumerable<COURSE_RECO_AVAIL> recommendation = await tf.FromAsync(craDsq.BeginExecute(null, null), iar => craDsq.EndExecute(iar));
                foreach (var re in recommendation)
                {
                    coursesData.AddCourse(Constants.CourseRecAvail2Course(re));
                }
                IEnumerable<COURSE_AVAIL> courses = courseDsq.EndExecute(result);
                foreach (var c in courses)
                {
                    coursesData.AddCourse(Constants.CourseAvail2Course(c));
                }

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    dataCategory = coursesData.GetGroupsByCategory();
                    cvs1.Source = dataCategory;
                    (SemanticZoom.ZoomedOutView as ListViewBase).ItemsSource = cvs1.View.CollectionGroups;
                    loadingProgressRing.IsActive = false;
                });
            }
            catch
            {
                ShowMessageDialog("on course avail complete");
                // Network Connection error.
            }
        }

        /// <summary>
        /// Network Connection error MessageDialog.
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
                    loadingProgressRing.IsActive = false;
                    await messageDialog.ShowAsync();
                });
        }

        //public void OnComplete(IAsyncResult result)
        //{
        //    var courses = ctx.EndExecute<COURSE_OK>(result);
        //    foreach (var c in courses)
        //    {
        //        System.Diagnostics.Debug.WriteLine(c.TITLE);
        //    }
        //}

        /// <summary>
        /// Invoked when a category is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a category for the selected category.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            var category = (sender as FrameworkElement).DataContext;
            string categoryName = ((GroupInfoList<object>)category).Key.ToString();

            Frame.Navigate(typeof(Category), categoryName);
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
        /// Handles the Click event of the UserProfileButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void UserProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login.Profile));
        }

        /// <summary>
        /// Handles the KeyUp event of the SearchKey control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs" /> instance containing the event data.</param>
        private void SearchKey_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
                courstoreSearchBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                courstoreSearchBox.Text = "";
            }
            else if (e.Key == Windows.System.VirtualKey.Enter && courstoreSearchBox.Text.Length != 0)
            {
                string searchKeyText = courstoreSearchBox.Text;
                List<string> searchOptions = new List<string>
                {
                    searchKeyText
                };
                Frame.Navigate(typeof(SearchResult), searchOptions);
            }
            else
            {
                courstoreSearchBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
                courstoreSearchBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }

        /// <summary>
        /// Handles the LostFocus event of the SearchBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            courstoreSearchBox.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            courstoreSearchBox.Text = "";
        }

        /// <summary>
        /// Handles the GotFocus event of the SearchBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            courstoreSearchBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        /// <summary>
        /// Handles the KeyUp event of the CourstoreGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs" /> instance containing the event data.</param>
        private void CourstoreGrid_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != Windows.System.VirtualKey.Enter)
            {
                courstoreSearchBox.Visibility = Windows.UI.Xaml.Visibility.Visible;
                courstoreSearchBox.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }
        }

        /// <summary>
        /// Handles the Click event of the AdvanceSearchButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AdvanceSearchButton_Click(object sender, RoutedEventArgs e)
        {
            AdvanceSearchPopup.IsOpen = true;
        }

        /// <summary>
        /// Handles the Click event of the SearchCancelButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void SearchCancelButton_Click(object sender, RoutedEventArgs e)
        {
            AdvanceSearchPopup.IsOpen = false;
        }

        /// <summary>
        /// Handles the KeyDown event of the AdvanceSearch control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs" /> instance containing the event data.</param>
        private void AdvanceSearch_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AdvanceSearchPopup.IsOpen = false;
                AdvanceSearchSubmitButton_Click(null, null);
            }
        }

        /// <summary>
        /// Handles the Click event of the AdvanceSearchSubmitButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void AdvanceSearchSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> SearchOption = new List<string>
            {
                SearchPopTitleText.Text,
                SearchPopAuthorText.Text,
                SearchPopDescriptionText.Text,
                (categorySearchComboBox.SelectedValue as CATEGORY).CATE_NAME
            };
            Frame.Navigate(typeof(SearchResult), SearchOption);
        }
    }
}
