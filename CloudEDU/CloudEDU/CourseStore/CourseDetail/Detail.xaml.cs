using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CloudEDU.CourseStore.CourseDetail
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Detail : Page
    {
        /// <summary>
        /// The course
        /// </summary>
        Course course;

        /// <summary>
        /// Initializes a new instance of the <see cref="Detail"/> class.
        /// </summary>
        public Detail()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            course = e.Parameter as Course;
            DataContext = course;
            submitOfTimeTextBlock.Text = course.StartTime.Year.ToString() + "." + course.StartTime.Month.ToString() + "." + course.StartTime.Day.ToString();
        }
    }
}
