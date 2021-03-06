﻿using CloudEDU.Common;
using CloudEDU.CourseStore;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudEDU.Login
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;

        /// <summary>
        /// The empty username
        /// </summary>
        private string emptyUsername;
        /// <summary>
        /// The first time for username
        /// </summary>
        private bool firstTimeForUsername = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            this.InitializeComponent();

            //emptyUsername = InputUsername.Text;
            ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InputUsername.IsEnabled = InputPassword.IsEnabled = LoginButton.IsEnabled = SignUpButton.IsEnabled = true;

            //csl = customers.ToList();
            // auto log
            //if (Constants.Read<bool>("AutoLog") == true)
            //{
            //    Constants.User = User.SelectLastUser();
            //    // navigate
            //    Frame.Navigate(typeof(CourseStore.Courstore));
            //}

            //// last user
            //if (Constants.Read<string>("LastUser") == default(string))
            //{

            //}
            //else
            //{
            //    System.Diagnostics.Debug.WriteLine(Constants.Read<string>("LastUser"));
            //    if (Frame.Navigate(typeof(SignUp)))
            //        System.Diagnostics.Debug.WriteLine("suc navigate");
            //    else
            //        System.Diagnostics.Debug.WriteLine("fail navigate");
            //}
        }

        /// <summary>
        /// Invoked when back button is clicked and navigate to sign up page.
        /// </summary>
        /// <param name="sender">The sign up button clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SignUp));
        }

        /// <summary>
        /// Handles the Click event of the LoginButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            InputUsername.IsEnabled = InputPassword.IsEnabled = LoginButton.IsEnabled = SignUpButton.IsEnabled = false;

            //login
            if (InputUsername.Text.Equals(emptyUsername) || InputPassword.Password.Equals(string.Empty))
            {
                var messageDialog = new MessageDialog("Check your input!");
                await messageDialog.ShowAsync();
                return;
            }
            //InputUsername.Text
            if (!Constants.isUserNameAvailable(InputUsername.Text))
            {
                var messageDialog = new MessageDialog("Check your input! Username can only contain 1-9 a-z and _");
                await messageDialog.ShowAsync();
                return;
            }
            //try
            //{
            //    TaskFactory<IEnumerable<CUSTOMER>> tf = new TaskFactory<IEnumerable<CUSTOMER>>();
            //    customerDsq = (DataServiceQuery<CUSTOMER>)(from user in ctx.CUSTOMER where user.NAME == InputUsername.Text select user);
            //    IEnumerable<CUSTOMER> cs = await tf.FromAsync(customerDsq.BeginExecute(null, null), iar => customerDsq.EndExecute(iar));
            //    csl = new List<CUSTOMER>(cs);
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("login request error: {0}", ex.Message);
            //    ShowMessageDialog();
            //}
            bool isLogined = false;
            try
            {
                foreach (CUSTOMER c in Constants.csl)
                {
                    if (c.NAME == InputUsername.Text)
                    {
                        if (c.PASSWORD == Constants.ComputeMD5(InputPassword.Password))
                        {
                            //login success
                            //CUSTOMER
                            //Constants.User = c;
                            System.Diagnostics.Debug.WriteLine(c.PASSWORD);
                            string Uri;
                            //User
                            if (!c.ALLOW)
                            {
                                var notAllowed = new MessageDialog("The User is forbidden!");
                                await notAllowed.ShowAsync();
                                Uri = "/AddDBLog?opr='TryLoginFailBecauseUserForbiddened'&msg='" + c.NAME + "'";
                                //ctx.UpdateObject(c);

                                try
                                {
                                    TaskFactory<IEnumerable<bool>> tf = new TaskFactory<IEnumerable<bool>>();
                                    IEnumerable<bool> result = await tf.FromAsync(ctx.BeginExecute<bool>(new Uri(Uri, UriKind.Relative), null, null), iar => ctx.EndExecute<bool>(iar));
                                }
                                catch
                                {
                                }
                                return;
                            }
                            Constants.Save<bool>("AutoLog", (bool)CheckAutoLogin.IsChecked);
                            Constants.User = new User(c);
                            isLogined = true;
                            System.Diagnostics.Debug.WriteLine("login success");
                            Uri = "/AddDBLog?opr='Login'&msg='" + Constants.User.NAME + "'";
                            //ctx.UpdateObject(c);

                            try
                            {
                                TaskFactory<IEnumerable<bool>> tf = new TaskFactory<IEnumerable<bool>>();
                                IEnumerable<bool> result = await tf.FromAsync(ctx.BeginExecute<bool>(new Uri(Uri, UriKind.Relative), null, null), iar => ctx.EndExecute<bool>(iar));

                            }
                            catch
                            {
                            }
                            Frame.Navigate(typeof(Courstore));
                            // navigate 
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine("Msg: {0}\nInnerExp:{1}\nStackTrace: {2} ",
                    exp.Message, exp.InnerException, exp.StackTrace);
                ShowMessageDialog();
            }
            // login fail
            if (isLogined) return;
            var msgDialog = new MessageDialog("Username Or Password is wrong");
            await msgDialog.ShowAsync();

            InputUsername.IsEnabled = InputPassword.IsEnabled = LoginButton.IsEnabled = SignUpButton.IsEnabled = true;
        }

        /// <summary>
        /// Handles the TextChanged event of the InputUsername control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void InputUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!firstTimeForUsername)
                return;
            emptyUsername = InputUsername.Text;
            firstTimeForUsername = false;
        }
        /// <summary>
        /// Network Connection error MessageDialog.
        /// </summary>
        private async void ShowMessageDialog()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var messageDialog = new MessageDialog("No Network has been found!");
                    messageDialog.Commands.Add(new UICommand("Try Again", (command) =>
                    {
                        Frame.Navigate(typeof(Login));
                    }));
                    messageDialog.Commands.Add(new UICommand("Close"));
                    //loadingProgressRing.IsActive = false;
                    await messageDialog.ShowAsync();
                }
                catch
                {
                    //ShowMessageDialog();
                }
            });
        }

        /// <summary>
        /// Handles the KeyDown event of the InputPassword control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs"/> instance containing the event data.</param>
        private void InputPassword_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginButton_Click(null, null);
            }
        }
    }
}
