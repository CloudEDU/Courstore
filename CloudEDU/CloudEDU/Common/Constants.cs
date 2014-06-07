using CloudEDU.CourseStore;
using CloudEDU.Login;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CloudEDU.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// The base URL
        /// </summary>
        public static string BaseURL = "http://221.239.198.33";
        //public static string BaseURL = "http://luyirenmax.oicp.net";
        /// <summary>
        /// The data center URI
        /// </summary>
        public static string DataCenterURI = BaseURL + "/BackgroundTransferSample/";
        /// <summary>
        /// The data service URI
        /// </summary>
        public static string DataServiceURI = BaseURL + "/DataService/Service.svc/";

        /// <summary>
        /// The fill star
        /// </summary>
        public static string FillStar = "\x2605";
        /// <summary>
        /// The blank star
        /// </summary>
        public static string BlankStar = "\x2606";
        /// <summary>
        /// The star width
        /// </summary>
        public static double StarWidth = 22.2133331298828;

        /// <summary>
        /// The resource type
        /// </summary>
        public static List<string> ResourceType = new List<string> { "DOCUMENT", "VIDEO", "AUDIO" };
        /// <summary>
        /// The coursing
        /// </summary>
        public static Coursing coursing;
        /// <summary>
        /// The user entity
        /// </summary>
        public static CUSTOMER UserEntity;
        /// <summary>
        /// The user
        /// </summary>
        public static User User;

        /// <summary>
        /// The category name list
        /// </summary>
        public static List<string> CategoryNameList = new List<string>();
        /// <summary>
        /// The record URI dic
        /// </summary>
        public static Dictionary<string, string> RecUriDic = new Dictionary<string, string>();
        /// <summary>
        /// The CSL
        /// </summary>
        public static List<CUSTOMER> csl;

        /// <summary>
        /// Determines whether this instance is internet.
        /// </summary>
        /// <returns></returns>
        public static bool IsInternet()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }

        /// <summary>
        /// Cast the first character of every word in a string from lower to upper.
        /// </summary>
        /// <param name="v">The string to be transformed.</param>
        /// <returns>
        /// The string after transformed.
        /// </returns>
        public static string UpperInitialChar(string v)
        {
            string[] words = null;
            StringBuilder strBuff = null;

            v = v.Trim();
            words = System.Text.RegularExpressions.Regex.Split(v, @"\s+");

            strBuff = new StringBuilder();
            for (int i = 0; i < words.Length; ++i)
            {
                words[0] = words[i].ToLower();
                strBuff.AppendFormat("{0}{1}", Char.ToUpper(words[i][0]), words[i].Substring(1));
                strBuff.Append(' ');
            }

            return strBuff.ToString();
        }



        /// <summary>
        /// 
        /// </summary>
        public class DepCourse
        {
            /// <summary>
            /// The course name
            /// </summary>
            public string CourseName = null;
            /// <summary>
            /// The learned
            /// </summary>
            public bool Learned = false;

            /// <summary>
            /// Initializes a new instance of the <see cref="DepCourse"/> class.
            /// </summary>
            /// <param name="_CourseName">Name of the _ course.</param>
            public DepCourse(string _CourseName)
            {
                this.CourseName = _CourseName;
                DepCourses = new List<DepCourse>();
                GetAllDepCourse().Add(this);

                /////check database if the user has learned this course and:
                // learned = ??


            }
            /// <summary>
            /// All dep courses
            /// </summary>
            private static List<DepCourse> AllDepCourses = null;
            /// <summary>
            /// Gets all dep course.
            /// </summary>
            /// <returns></returns>
            public static List<DepCourse> GetAllDepCourse()
            {
                if (AllDepCourses == null)
                {
                    AllDepCourses = new List<DepCourse>();
                }
                return AllDepCourses;
            }
            /// <summary>
            /// The dep courses
            /// </summary>
            public List<DepCourse> DepCourses;


            /// <summary>
            /// Gets the dep courses.
            /// </summary>
            /// <param name="courseName">Name of the course.</param>
            /// <param name="list">The list.</param>
            /// <returns></returns>
            public static List<DepCourse> GetDepCourses(String courseName, List<DepCourse> list = null)
            {
                if (list == null)
                {
                    list = new List<DepCourse>();
                }
                DepCourse course = null;
                foreach (DepCourse dc in GetAllDepCourse())
                {
                    if (dc.CourseName.Equals(courseName))
                    {
                        course = dc;
                        break;
                    }
                }
                if (course == null)
                {
                    return new List<DepCourse>();
                }
                foreach (DepCourse dc in course.DepCourses)
                {
                    if (!dc.Learned)
                    {
                        list.Add(dc);
                    }
                    list = DepCourse.GetDepCourses(dc.CourseName, list);

                }


                return list;

            }

            /// <summary>
            /// The learned courses
            /// </summary>
            private static List<COURSE_AVAIL> LearnedCourses = null;


            /// <summary>
            /// Ifs the learned.
            /// </summary>
            /// <param name="courseName">Name of the course.</param>
            /// <returns></returns>
            public static bool IfLearned(string courseName)
            {
                if (LearnedCourses == null)
                {
                    GetAllLearned();
                }
                foreach (COURSE_AVAIL ca in LearnedCourses)
                {
                    if (ca.TITLE.Equals(courseName))
                    {
                        return true;
                    }
                }
                return false;
            }
            /// <summary>
            /// Gets all learned.
            /// </summary>
            public async static void GetAllLearned()
            {
                if (LearnedCourses == null)
                {
                    LearnedCourses = new List<COURSE_AVAIL>();

                    CloudEDUEntities ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
                    TaskFactory<IEnumerable<COURSE_AVAIL>> tf = new TaskFactory<IEnumerable<COURSE_AVAIL>>();

                    IEnumerable<COURSE_AVAIL> attends = await tf.FromAsync(ctx.BeginExecute<COURSE_AVAIL>(
                        new Uri("/GetAllCoursesAttendedByCustomer?customer_id=" + Constants.User.ID, UriKind.Relative), null, null),
                        iar => ctx.EndExecute<COURSE_AVAIL>(iar));
                    foreach (var ca in attends)
                    {
                        LearnedCourses.Add(ca);
                    }
                }


            }


        }



        /// <summary>
        /// Constructs the dependent courses.
        /// </summary>
        public static void ConstructDependentCourses()
        {
            DepCourse datamining = new DepCourse("DataMining");
            DepCourse statistics = new DepCourse("Probability and Statistics");
            DepCourse ml = new DepCourse("Machine Learning");
            ml.Learned = true;
            DepCourse algorithms = new DepCourse("Algorithm Design");
            DepCourse discrete = new DepCourse("Discrete Mathematics");
            DepCourse datastruct = new DepCourse("Data Structure");
            DepCourse compiler = new DepCourse("Compilers");
            DepCourse linux = new DepCourse("Linux Programming");
            DepCourse os = new DepCourse("Operating System");
            DepCourse mobile = new DepCourse("Mobile Programming");
            DepCourse oop = new DepCourse("OOP");
            DepCourse web = new DepCourse("Web2.0 Program Design");
            DepCourse c = new DepCourse("C Language");
            DepCourse embedded = new DepCourse("Embedded Programming");
            DepCourse ca = new DepCourse("Computer Architecture");
            DepCourse intro = new DepCourse("Computer Basics");
            DepCourse network = new DepCourse("Computer Network");

            datamining.DepCourses.Add(statistics);
            datamining.DepCourses.Add(ml);
            ml.DepCourses.Add(algorithms);
            algorithms.DepCourses.Add(discrete);
            algorithms.DepCourses.Add(datastruct);
            compiler.DepCourses.Add(datastruct);
            linux.DepCourses.Add(os);
            linux.DepCourses.Add(c);
            os.DepCourses.Add(datastruct);
            mobile.DepCourses.Add(oop);
            web.DepCourses.Add(oop);
            web.DepCourses.Add(network);
            oop.DepCourses.Add(c);
            embedded.DepCourses.Add(c);
            embedded.DepCourses.Add(ca);
            ca.DepCourses.Add(intro);
            network.DepCourses.Add(intro);

            List<DepCourse> allCourse = DepCourse.GetAllDepCourse();
            foreach (DepCourse course in allCourse)
            {
                System.Diagnostics.Debug.WriteLine(course.CourseName);

            }


        }


        /// <summary>
        /// Haves the dependent.
        /// </summary>
        /// <returns></returns>
        public static List<string> HaveDependent()
        {
            List<string> dependent = new List<string>();


            return dependent;
        }
        /// <summary>
        /// Convert a dataservice COURSE_AVAIL view to Course model.
        /// </summary>
        /// <param name="c">COURSE_AVAIL to be converted.</param>
        /// <returns>
        /// Course after converted.
        /// </returns>
        public static Course CourseAvail2Course(COURSE_AVAIL c)
        {
            Course course = new Course();

            course.Title = c.TITLE;
            course.Intro = c.INTRO;
            course.ID = c.ID;
            course.Teacher = c.TEACHER_NAME;
            course.Category = c.CATE_NAME;
            course.Price = c.PRICE;
            course.Rate = c.RATE;
            course.PG = c.RESTRICT_AGE;
            course.LessonNum = c.LESSON_NUM;
            course.RatedUser = c.RATED_USERS;
            course.ImageUri = DataCenterURI + c.ICON_URL.Replace('\\', '/');
            course.IsBuy = false;
            course.IsTeach = false;

            return course;
        }




        /// <summary>
        /// Courses the record avail2 course.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static Course CourseRecAvail2Course(COURSE_RECO_AVAIL c)
        {
            Course course = new Course();

            course.Title = c.TITLE;
            course.Intro = c.INTRO;
            course.ID = c.ID;
            course.Teacher = c.TEACHER_NAME;
            course.Category = c.RECO_TITLE;
            course.Price = c.PRICE;
            course.Rate = c.RATE;
            course.PG = c.RESTRICT_AGE;
            course.LessonNum = c.LESSON_NUM;
            course.RatedUser = c.RATED_USERS;
            course.ImageUri = DataCenterURI + c.ICON_URL.Replace('\\', '/');
            course.IsBuy = false;
            course.IsTeach = false;

            return course;
        }

        /// <summary>
        /// Courses the rec2 course.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns></returns>
        public static Course CourseRec2Course(COURSE_AVAIL c)
        {
            Course course = new Course();

            course.Title = c.TITLE;
            course.Intro = c.INTRO;
            course.ID = c.ID;
            course.Teacher = c.TEACHER_NAME;
            course.Category = "Recommendation";
            course.Price = c.PRICE;
            course.Rate = c.RATE;
            course.PG = c.RESTRICT_AGE;
            course.LessonNum = c.LESSON_NUM;
            course.RatedUser = c.RATED_USERS;
            course.ImageUri = DataCenterURI + c.ICON_URL.Replace('\\', '/');
            course.IsBuy = false;
            course.IsTeach = false;

            return course;
        }

        /// <summary>
        /// Computes the m d5.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static string ComputeMD5(string str)
        {
            var alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void Save<T>(string key, T value)
        {
            ApplicationData.Current.LocalSettings.Values[key] = value;
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <returns>
        /// 值
        /// </returns>
        public static T Read<T>(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                return (T)ApplicationData.Current.LocalSettings.Values[key];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>
        /// 成功true/失败false
        /// </returns>
        public static bool Remove(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.Remove(key);
        }

        /// <summary>
        /// Determines whether [is user name available] [the specified un].
        /// </summary>
        /// <param name="un">The un.</param>
        /// <returns></returns>
        public static bool isUserNameAvailable(string un)
        {
            string Regextest = "^[a-zA-Z_][a-zA-Z0-9_]{2,14}$";
            return Regex.IsMatch(un, Regextest);
        }

        /// <summary>
        /// Determines whether [is email available] [the specified em].
        /// </summary>
        /// <param name="em">The em.</param>
        /// <returns></returns>
        public static bool isEmailAvailable(string em)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            return re.IsMatch(em);
        }
    }

    /// <summary>
    /// Used as selector to select container size.
    /// </summary>
    public enum GridViewItemContainerType
    {
        /// <summary>
        /// The default grid view item container size
        /// </summary>
        DefaultGridViewItemContainerSize = 0,
        /// <summary>
        /// The double height grid view item container size
        /// </summary>
        DoubleHeightGridViewItemContainerSize = 1,
        /// <summary>
        /// The double width grid view item contsiner size
        /// </summary>
        DoubleWidthGridViewItemContsinerSize = 2,
        /// <summary>
        /// The square grid view item container size
        /// </summary>
        SquareGridViewItemContainerSize = 3,
    }
}

