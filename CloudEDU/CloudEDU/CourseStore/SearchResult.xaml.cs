﻿using CloudEDU.Common;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchResult : GlobalPage
    {
        /// <summary>
        /// The store search result
        /// </summary>
        private StoreData storeSearchResult;
        /// <summary>
        /// The search results
        /// </summary>
        private List<GroupInfoList<Object>> searchResults;
        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;
        /// <summary>
        /// The course DSQ
        /// </summary>
        private DataServiceQuery<COURSE_AVAIL> courseDsq = null;

        /// <summary>
        /// The search title key
        /// </summary>
        private string searchTitleKey;
        /// <summary>
        /// The search author key
        /// </summary>
        private string searchAuthorKey;
        /// <summary>
        /// The search description key
        /// </summary>
        private string searchDescriptionKey;
        /// <summary>
        /// The search category key
        /// </summary>
        private string searchCategoryKey;

        /// <summary>
        /// The search options
        /// </summary>
        List<string> searchOptions = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult"/> class.
        /// </summary>
        public SearchResult()
        {
            this.InitializeComponent();

            ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            searchOptions = e.Parameter as List<string>;
            loadingProgressRing.IsActive = true;

            searchTitleKey = searchOptions[0].Trim();
            if (searchOptions.Count == 4)
            {
                searchAuthorKey = searchOptions[1].Trim();
                searchDescriptionKey = searchOptions[2].Trim();
                searchCategoryKey = searchOptions[3];
            }
            else
            {
                searchAuthorKey = searchDescriptionKey = searchCategoryKey = "";
            }

            courseDsq = (DataServiceQuery<COURSE_AVAIL>)(from course_avail in ctx.COURSE_AVAIL
                                                         select course_avail);
            if (!searchCategoryKey.Equals("Any Categories") && !searchCategoryKey.Equals(""))
            {
                courseDsq = (DataServiceQuery<COURSE_AVAIL>)courseDsq.Where(c => c.CATE_NAME.Equals(searchCategoryKey));
            }
            if (!searchTitleKey.Equals(""))
            {
                courseDsq = (DataServiceQuery<COURSE_AVAIL>)courseDsq.Where(c => c.TITLE.Contains(searchTitleKey));
            }
            if (!searchAuthorKey.Equals(""))
            {
                courseDsq = (DataServiceQuery<COURSE_AVAIL>)courseDsq.Where(c => c.TEACHER_NAME.Contains(searchAuthorKey));
            }
            if (!searchDescriptionKey.Equals(""))
            {
                courseDsq = (DataServiceQuery<COURSE_AVAIL>)courseDsq.Where(c => c.INTRO.Contains(searchDescriptionKey));
            }

            courseDsq.BeginExecute(OnSearchResultComplete, null);

            UserProfileBt.DataContext = Constants.User;
        }

        /// <summary>
        /// Called when [search result complete].
        /// </summary>
        /// <param name="result">The result.</param>
        private async void OnSearchResultComplete(IAsyncResult result)
        {
            storeSearchResult = new StoreData();
            try
            {
                IEnumerable<COURSE_AVAIL> courses = courseDsq.EndExecute(result);

                foreach (var c in courses)
                {
                    storeSearchResult.AddCourse(Constants.CourseAvail2Course(c));
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        searchResults = storeSearchResult.GetSearchResultGroup(storeSearchResult.Count().ToString());
                        cvs1.Source = searchResults;
                        loadingProgressRing.IsActive = false;
                    });
            }
            catch
            {
                ShowMessageDialog();
            }
        }

        /// <summary>
        /// Handles the Click event of the BackButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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
        /// Invoked when a course within this category is clicked.
        /// </summary>
        /// <param name="sender">The GridView displaying the course clicked.</param>
        /// <param name="e">Event data that describes the course clicked.</param>
        private void Course_ItemClick(object sender, ItemClickEventArgs e)
        {
            var course = (Course)e.ClickedItem;

            Frame.Navigate(typeof(CourseOverview), course);
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
    }
}
