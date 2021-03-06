﻿using CloudEDU.Common;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace CloudEDU.CourseStore.CoursingDetail
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Home : Page
    {
        /// <summary>
        /// The course
        /// </summary>
        Course course;

        /// <summary>
        /// The categories
        /// </summary>
        private List<CATEGORY> categories;
        /// <summary>
        /// The PGS
        /// </summary>
        private List<PARENT_GUIDE> pgs;

        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;
        /// <summary>
        /// The category DSQ
        /// </summary>
        private DataServiceQuery<CATEGORY> categoryDsq = null;
        /// <summary>
        /// The pg DSQ
        /// </summary>
        private DataServiceQuery<PARENT_GUIDE> pgDsq = null;

        /// <summary>
        /// The edit course
        /// </summary>
        COURSE editCourse = null;

        /// <summary>
        /// Constructor, initilize the components.
        /// </summary>
        public Home()
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
            course = e.Parameter as Course;
            List<object> courseInfo = e.Parameter as List<object>;
            course = courseInfo[0] as Course;
            DataContext = course;

            if ((courseInfo[1] as string) == "attending")
            {
                beginButton.Content = "Start Learning!";
                getLearnedPercentage();
            }
            else if ((courseInfo[1] as string) == "teaching")
            {
                beginButton.Content = "Edit Course";
            }

            SetStarsStackPanel(course.Rate ?? 0);

            categoryDsq = (DataServiceQuery<CATEGORY>)(from category in ctx.CATEGORY select category);
            categoryDsq.BeginExecute(OnCategoryComplete, null);

            pgDsq = (DataServiceQuery<PARENT_GUIDE>)(from pg in ctx.PARENT_GUIDE select pg);
            pgDsq.BeginExecute(OnPGComplete, null);


            // getLearnedPercentage();

        }





        /// <summary>
        /// Gets the learned percentage.
        /// </summary>
        private async void getLearnedPercentage()
        {
            double percent;

            string uri2 = "/CoursePercentByCustomer?customer_id=" + Constants.User.ID + "&course_id=" + course.ID;
            TaskFactory<IEnumerable<double>> tf = new TaskFactory<IEnumerable<double>>();
            IEnumerable<double> percentages = await tf.FromAsync(ctx.BeginExecute<double>(new Uri(uri2, UriKind.Relative), null, null), iar => ctx.EndExecute<double>(iar));
            percent = percentages.FirstOrDefault();

            FinishPercentage.Text = "Finished " + percent * 100 + "%";
        }

        /// <summary>
        /// Sets the stars stack panel.
        /// </summary>
        /// <param name="rate">The rate.</param>
        private void SetStarsStackPanel(double rate)
        {
            int fillInt = (int)rate;
            int blankInt = 5 - fillInt - 1;
            double percentFill = rate - (double)fillInt;

            for (int i = 0; i < fillInt; ++i)
            {
                TextBlock fillStarTextBlock = new TextBlock
                {
                    Style = Application.Current.Resources["SubheaderTextStyle"] as Style,
                    Foreground = new SolidColorBrush(Colors.White),
                    Text = Constants.FillStar
                };
                rateStarsPanel.Children.Add(fillStarTextBlock);
            }
            if (rate == 5) return;
            double width = Constants.StarWidth * percentFill;
            TextBlock halfFillStarTextBlock = new TextBlock
            {
                Style = Application.Current.Resources["SubheaderTextStyle"] as Style,
                Foreground = new SolidColorBrush(Colors.White),
                Text = Constants.FillStar,
                Width = width
            };
            TextBlock halfBlankStarTextBlock = new TextBlock
            {
                Style = Application.Current.Resources["SubheaderTextStyle"] as Style,
                Foreground = new SolidColorBrush(Colors.White),
                Text = Constants.BlankStar,
                Margin = new Thickness(-width, 0, 0, 0)
            };
            rateStarsPanel.Children.Add(halfFillStarTextBlock);
            rateStarsPanel.Children.Add(halfBlankStarTextBlock);
            for (int i = 0; i < blankInt; ++i)
            {
                TextBlock blankStarTextBlock = new TextBlock
                {
                    Style = Application.Current.Resources["SubheaderTextStyle"] as Style,
                    Foreground = new SolidColorBrush(Colors.White),
                    Text = Constants.BlankStar,
                };
                rateStarsPanel.Children.Add(blankStarTextBlock);
            }
        }

        /// <summary>
        /// Handles the 1 event of the beginButton_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void beginButton_Click_1(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt.Content.ToString() == "Start Learning!")
            {
                Constants.coursing.NavigateToLecture();
            }
            else if (bt.Content.ToString() == "Edit Course")
            {
                try
                {
                    DataServiceQuery<COURSE> dsq = (DataServiceQuery<COURSE>)(from c in ctx.COURSE
                                                                              where c.ID == course.ID
                                                                              select c);
                    TaskFactory<IEnumerable<COURSE>> tf = new TaskFactory<IEnumerable<COURSE>>();
                    editCourse = (await tf.FromAsync(dsq.BeginExecute(null, null), iar => dsq.EndExecute(iar))).FirstOrDefault();
                }
                catch
                {
                    ShowMessageDialog("Network connection error.31");
                    return;
                }

                courseTitle.Text = editCourse.TITLE;
                price.Text = editCourse.PRICE.ToString();
                courseContent.Text = editCourse.INTRO;

                EditCoursePopup.IsOpen = true;
            }
        }

        /// <summary>
        /// DataServiceQuery callback method to refresh the UI.
        /// </summary>
        /// <param name="result">Async operation result.</param>
        private async void OnCategoryComplete(IAsyncResult result)
        {
            try
            {
                IEnumerable<CATEGORY> cts = categoryDsq.EndExecute(result);
                categories = new List<CATEGORY>(cts);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    categoryComboBox.ItemsSource = categories;
                    categoryComboBox.SelectedIndex = 0;
                });
            }
            catch
            {
                ShowMessageDialog("Network connection error!4");
            }
        }

        /// <summary>
        /// Called when [pg complete].
        /// </summary>
        /// <param name="result">The result.</param>
        private async void OnPGComplete(IAsyncResult result)
        {
            try
            {
                IEnumerable<PARENT_GUIDE> ps = pgDsq.EndExecute(result);
                pgs = new List<PARENT_GUIDE>(ps);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    pgComboBox.ItemsSource = pgs;
                    pgComboBox.SelectedIndex = 0;
                });
            }
            catch
            {

            }
        }

        /// <summary>
        /// Shows the message dialog.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private async void ShowMessageDialog(string msg)
        {
            var messageDialog = new MessageDialog(msg);
            messageDialog.Commands.Add(new UICommand("Close"));
            await messageDialog.ShowAsync();
        }

        /// <summary>
        /// Handles the Click event of the CancelUploadButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void CancelUploadButton_Click(object sender, RoutedEventArgs e)
        {
            EditCoursePopup.IsOpen = false;
            courseTitle.Text = "";
            courseContent.Text = "";
            price.Text = "0.0";
            categoryComboBox.SelectedIndex = 0;
            pgComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Handles the Click event of the SaveNoteButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                editCourse.TITLE = courseTitle.Text;
                editCourse.INTRO = courseContent.Text;
                editCourse.PRICE = Convert.ToDecimal(price.Text);
                editCourse.CATEGORY = (categoryComboBox.SelectedItem as CATEGORY).ID;
                editCourse.PG = (pgComboBox.SelectedItem as PARENT_GUIDE).ID;
            }
            catch
            {
                ShowMessageDialog("Format error1! Please check your input.");
                return;
            }

            try
            {
                ctx.UpdateObject(editCourse);
                TaskFactory<DataServiceResponse> tf = new TaskFactory<DataServiceResponse>();
                await tf.FromAsync(ctx.BeginSaveChanges(null, null), iar => ctx.EndSaveChanges(iar));
            }
            catch
            {
                ShowMessageDialog("Edit failed! Network connection error.2");
                EditCoursePopup.IsOpen = false;
                return;
            }

            ShowMessageDialog("Edit successfully!");
            EditCoursePopup.IsOpen = false;
        }
    }
}
