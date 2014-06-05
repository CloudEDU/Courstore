﻿using CloudEDU.Common;
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

namespace CloudEDU.Login
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignUp : Page
    {
        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;
        /// <summary>
        /// The customer DSQ
        /// </summary>
        private DataServiceQuery<CUSTOMER> customerDsq = null;
        /// <summary>
        /// The CSL
        /// </summary>
        private List<CUSTOMER> csl;
        /// <summary>
        /// The c
        /// </summary>
        CUSTOMER c;
        /// <summary>
        /// Initializes a new instance of the <see cref="SignUp"/> class.
        /// </summary>
        public SignUp()
        {
            this.InitializeComponent();
            ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            customerDsq = (DataServiceQuery<CUSTOMER>)(from user in ctx.CUSTOMER select user);
            customerDsq.BeginExecute(OnCustomerComplete, null);
        }

        /// <summary>
        /// Called when [customer complete].
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnCustomerComplete(IAsyncResult result)
        {
            csl = customerDsq.EndExecute(result).ToList();
            System.Diagnostics.Debug.WriteLine(csl[0].NAME);
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (InputUsername.Text.Equals(string.Empty) || InputPassword.Password.Equals(string.Empty))
            {
                var messageDialog = new MessageDialog("Check your input!");
                await messageDialog.ShowAsync();
                return;
            }

            if (!Constants.isUserNameAvailable(InputUsername.Text))
            {
                var messageDialog = new MessageDialog("Check your input! Username can only contain 1-9 a-z and _");
                await messageDialog.ShowAsync();
                return;
            }

            if (!InputPassword.Password.Equals(ReInputPassword.Password))
            {
                var dialog = new MessageDialog("Passwords are not same! Try again, thx!");
                await dialog.ShowAsync();
                return;
            }

            if (isUserAlreadyThere())
            {
                var dialog = new MessageDialog("Username already exists");
                await dialog.ShowAsync();
                return;
            }

            //CUSTOMER c = CUSTOMER.CreateCUSTOMER(null, InputUsername.Text, InputPassword.Password, null, null, null);
            c = new CUSTOMER()
            {
                NAME = InputUsername.Text,
                PASSWORD = Constants.ComputeMD5(InputPassword.Password),
                ALLOW = true,
                BALANCE = 100,
            };
            ctx.AddToCUSTOMER(c);
            ctx.BeginSaveChanges(OnCustomerSaveChange, null);
        }

        /// <summary>
        /// Called when [customer save change].
        /// </summary>
        /// <param name="result">The result.</param>
        private async void OnCustomerSaveChange(IAsyncResult result)
        {
            try
            {
                ctx.EndSaveChanges(result);
                string Uri = "/AddDBLog?opr='SignUp'&msg='" + c.NAME + "'";
                //ctx.UpdateObject(c);

                try
                {
                    TaskFactory<IEnumerable<bool>> tf = new TaskFactory<IEnumerable<bool>>();
                    IEnumerable<bool> resulta = await tf.FromAsync(ctx.BeginExecute<bool>(new Uri(Uri, UriKind.Relative), null, null), iar => ctx.EndExecute<bool>(iar));
                }
                catch
                {
                }
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Frame.Navigate(typeof(Login));
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Msg: {0}\nInnerExp:{1}\nStackTrace: {2} ",
                    e.Message, e.InnerException, e.StackTrace);
                ShowMessageDialog();
                //Network Connection error.
            }
        }

        /// <summary>
        /// Determines whether [is user already there].
        /// </summary>
        /// <returns></returns>
        private bool isUserAlreadyThere()
        {
            foreach (CUSTOMER c in csl)
            {
                if (c.NAME == InputUsername.Text)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Network Connection error MessageDialog.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private async void ShowMessageDialog(String msg = "No network has been founddddddd!")
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var messageDialog = new MessageDialog(msg);
                    messageDialog.Commands.Add(new UICommand("Try Again", (command) =>
                    {
                        Frame.Navigate(typeof(SignUp));
                    }));
                    messageDialog.Commands.Add(new UICommand("Close"));
                    //loadingProgressRing.IsActive = false;
                    await messageDialog.ShowAsync();
                }
                catch
                {
                    ShowMessageDialog();
                }
            });
        }

        /// <summary>
        /// Handles the Click event of the LoginButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Login));
        }
    }
}
