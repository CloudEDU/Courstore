using CloudEDU.Login;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234235 上有介绍

namespace CloudEDU.Common
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class UserProfileButton : Button
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public User user
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get
            {
                return (string)GetValue(UserNameProperty);
            }
            set
            {
                SetValue(UserNameProperty, value);
            }
        }

        /// <summary>
        /// The user name property
        /// </summary>
        public static readonly DependencyProperty UserNameProperty =
        DependencyProperty.Register("UserName", typeof(string), typeof(UserProfileButton),
          null);

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileButton"/> class.
        /// </summary>
        public UserProfileButton()
        {
            this.DefaultStyleKey = typeof(UserProfileButton);
        }
    }
}
