using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234235 上有介绍

namespace CloudEDU.Login
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class UserSelButtonControl : Button
    {
        /// <summary>
        /// The grid
        /// </summary>
        public Grid grid;
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
       DependencyProperty.Register("UserName", typeof(string), typeof(UserSelButtonControl),
          null);

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSelButtonControl"/> class.
        /// </summary>
        public UserSelButtonControl()
        {
            this.DefaultStyleKey = typeof(UserSelButtonControl);
        }
        /// <summary>
        /// 在应用程序代码或内部进程（如重新生成布局处理过程）调用 ApplyTemplate 方法调用。简而言之，这意味着将在应用程序中显示 UI 元素前一刻调用该方法。重写此方法会影响类的默认后模板逻辑。
        /// </summary>
        protected override void OnApplyTemplate()
        {
            grid = GetTemplateChild("grid") as Grid;
            grid.DataContext = user;
        }
    }
}
