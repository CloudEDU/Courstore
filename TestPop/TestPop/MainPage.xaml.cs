using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace TestPop
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string content;
        public MainPage()
        {
            this.InitializeComponent();
            string htmlcontent = "return true \n normal";
            content = "<html><head><link rel=\"stylesheet\" href=\"http://yandex.st/highlightjs/8.0/styles/default.min.css\"><script src=\"http://yandex.st/highlightjs/8.0/highlight.min.js\"></script> <script>hljs.initHighlightingOnLoad();</script></head><body><pre><code>"
                        +
                            htmlcontent
                        + "</code></pre></body> </html>";
        }
        
        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = true;
            Html.NavigateToString(content);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = false;
        }





    }
}
