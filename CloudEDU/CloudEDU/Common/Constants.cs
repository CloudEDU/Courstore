using CloudEDU.CourseStore;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using CloudEDU.Login;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Windows.Storage;
using System.Text.RegularExpressions;

namespace CloudEDU.Common
{
    public class Constants
    {
        //public static string BaseURI = "http://luyirenmax.oicp.net/BackgroundTransferSample/";
        //public static string DataServiceURI = "http://luyirenmax.oicp.net/DataService/Service.svc/";

        public static string BaseURI = "http://218.78.189.144/BackgroundTransferSample/";
        public static string DataServiceURI = "http://218.78.189.144/DataService/Service.svc/";

        public static string FillStar = "\x2605";
        public static string BlankStar = "\x2606";
        public static double StarWidth = 22.2133331298828;

        public static List<string> ResourceType = new List<string> { "DOCUMENT", "VIDEO", "AUDIO" };
        public static Coursing coursing;
        public static CUSTOMER UserEntity;
        public static User User;

        public static List<string> CategoryNameList = new List<string>();
        public static Dictionary<string, string> RecUriDic = new Dictionary<string, string>();
        /// <summary>
        /// Cast the first character of every word in a string from lower to upper.
        /// </summary>
        /// <param name="v">The string to be transformed.</param>
        /// <returns>The string after transformed.</returns>
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



        public class DepCourse
        {
            public string CourseName = null;
            public bool Learned = false;
            
            public DepCourse(string _CourseName)
            {
                this.CourseName = _CourseName;
                DepCourses = new List<DepCourse>();
                GetAllDepCourse().Add(this);

                /////check database if the user has learned this course and:
                // learned = ??
                
                
            }
            private static List<DepCourse> AllDepCourses = null;
            public static List<DepCourse> GetAllDepCourse()
            {
                if(AllDepCourses==null){
                    AllDepCourses= new List<DepCourse>();
                }
                return AllDepCourses;
            }
            public List<DepCourse> DepCourses;


            public static List<DepCourse> GetDepCourses(String courseName, List<DepCourse> list =null)
            {
                if(list == null){
                    list = new List<DepCourse>();
                }
                DepCourse course = null;
                foreach(DepCourse dc in GetAllDepCourse()){
                    if(dc.CourseName.Equals(courseName)){
                        course = dc;
                        break;
                    }
                }


                foreach(DepCourse dc in course.DepCourses){
                    if (!dc.Learned)
                    {
                        list.Add(dc);
                    }
                    list = DepCourse.GetDepCourses(dc.CourseName, list);

                }


                return list;

            }

            private static List<COURSE_AVAIL> LearnedCourses= null;
            

            public static bool IfLearned(string courseName){
                if(LearnedCourses==null){
                    GetAllLearned();
                }
                foreach(COURSE_AVAIL ca in LearnedCourses){
                    if (ca.TITLE.Equals(courseName)){
                        return true;
                    }
                }
                return false;
            }
            public async static void GetAllLearned()
            {
                if(LearnedCourses==null){
                    LearnedCourses = new List<COURSE_AVAIL>();

                    CloudEDUEntities ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
                    TaskFactory<IEnumerable<COURSE_AVAIL>> tf = new TaskFactory<IEnumerable<COURSE_AVAIL>>();

                    IEnumerable<COURSE_AVAIL> attends = await tf.FromAsync(ctx.BeginExecute<COURSE_AVAIL>(
                        new Uri("/GetAllCoursesAttendedByCustomer?customer_id=" + Constants.User.ID, UriKind.Relative), null, null),
                        iar => ctx.EndExecute<COURSE_AVAIL>(iar));
                    foreach(var ca in attends){
                        LearnedCourses.Add(ca);
                    }
                }
     

            }


        }

        
        
        public static void ConstructDependentCourses()
        {
            DepCourse datamining = new DepCourse("DataMining");
            DepCourse statistics = new DepCourse("Statistics");
            DepCourse ml = new DepCourse("MachineLearning");
            ml.Learned = true;
            DepCourse algorithms = new DepCourse("Algorithems");
            DepCourse discrete = new DepCourse("DiscreteMathematics");
            DepCourse datastruct = new DepCourse("DataStructure");
            DepCourse compiler = new DepCourse("Compiler");
            DepCourse linux = new DepCourse("LinuxProgramming");
            DepCourse os = new DepCourse("OperatingSystem");
            DepCourse mobile = new DepCourse("MobileProgramming");
            DepCourse oop = new DepCourse("OOP");
            DepCourse web = new DepCourse("WebProgramming");
            DepCourse c = new DepCourse("CProgramming");
            DepCourse embedded = new DepCourse("EmbeddedProgramming");
            DepCourse ca = new DepCourse("ComputerArchitecture");
            DepCourse intro = new DepCourse("IntroductionToCS");
            DepCourse network = new DepCourse("ComputerNetwork");

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
            foreach(DepCourse course in allCourse)
            {
                //System.Diagnostics.Debug.WriteLine(course.CourseName);
                
            }


        }


        public static List<string> HaveDependent()
        {
            List<string> dependent = new List<string>();


            return dependent;
        }
        /// <summary>
        /// Convert a dataservice COURSE_AVAIL view to Course model.
        /// </summary>
        /// <param name="c">COURSE_AVAIL to be converted.</param>
        /// <returns>Course after converted.</returns>
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
            course.ImageUri = BaseURI + c.ICON_URL.Replace('\\', '/');
            course.IsBuy = false;
            course.IsTeach = false;

            return course;
        }




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
            course.ImageUri = BaseURI + c.ICON_URL.Replace('\\', '/');
            course.IsBuy = false;
            course.IsTeach = false;

            return course;
        }

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
            course.ImageUri = BaseURI + c.ICON_URL.Replace('\\', '/');
            course.IsBuy = false;
            course.IsTeach = false;

            return course;
        }

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
        /// <returns>值</returns>
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
        /// <returns>成功true/失败false</returns>
        public static bool Remove(string key)
        {
            return ApplicationData.Current.LocalSettings.Values.Remove(key);
        }

        public static bool isUserNameAvailable(string un)
        {
            string Regextest = "^[a-zA-Z_][a-zA-Z0-9_]{2,14}$";
            return Regex.IsMatch(un, Regextest);
        }

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
        DefaultGridViewItemContainerSize = 0,
        DoubleHeightGridViewItemContainerSize = 1,
        DoubleWidthGridViewItemContsinerSize = 2,
        SquareGridViewItemContainerSize = 3,
    }
}

