using CloudEDU.Common;
using CloudEDU.Service;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace CloudEDU.Login
{
    /// <summary>
    /// User model
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [SQLite.PrimaryKey]
        public string NAME { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="User"/> is allow.
        /// </summary>
        /// <value>
        ///   <c>true</c> if allow; otherwise, <c>false</c>.
        /// </value>
        public bool ALLOW { get; set; }
        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        /// <value>
        /// The image source.
        /// </value>
        public string ImageSource { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string EMAIL { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string PASSWORD { get; set; }
        /// <summary>
        /// Gets or sets the birthday.
        /// </summary>
        /// <value>
        /// The birthday.
        /// </value>
        public DateTime BIRTHDAY { get; set; }
        /// <summary>
        /// Gets or sets the degree.
        /// </summary>
        /// <value>
        /// The degree.
        /// </value>
        public string DEGREE { get; set; }
        /// <summary>
        /// Gets or sets the lear n_ rate.
        /// </summary>
        /// <value>
        /// The lear n_ rate.
        /// </value>
        public double LEARN_RATE { get; set; }
        /// <summary>
        /// Gets or sets the teac h_ rate.
        /// </summary>
        /// <value>
        /// The teac h_ rate.
        /// </value>
        public double TEACH_RATE { get; set; }
        /// <summary>
        /// Gets or sets the balance.
        /// </summary>
        /// <value>
        /// The balance.
        /// </value>
        public decimal BALANCE { get; set; }
        /// <summary>
        /// Gets or sets the atten d_ count.
        /// </summary>
        /// <value>
        /// The atten d_ count.
        /// </value>
        public int ATTEND_COUNT { get; set; }
        /// <summary>
        /// Gets or sets the teac h_ count.
        /// </summary>
        /// <value>
        /// The teac h_ count.
        /// </value>
        public int TEACH_COUNT { get; set; }
        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;
        //private DataServiceQuery<CUSTOMER> customerDsq = null;
        //private List<CUSTOMER> csl;
        /// <summary>
        /// The teach DSQ
        /// </summary>
        private DataServiceQuery<COURSE_AVAIL> teachDsq = null;

        //public NOTE NOTE { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="c">The c.</param>
        public User(CUSTOMER c)
        {
            Constants.UserEntity = c;
            //ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
            NAME = c.NAME;
            ID = c.ID;
            EMAIL = c.EMAIL;
            PASSWORD = c.PASSWORD;
            TEACH_RATE = c.TEACH_RATE;
            LEARN_RATE = c.LEARN_RATE;
            DEGREE = c.DEGREE;
            BIRTHDAY = c.BIRTHDAY;
            BALANCE = c.BALANCE;
            ALLOW = c.ALLOW;
            //!!!!!!!
            SetAttendTeachNumber();
            //TaskFactory<IEnumerable<CUSTOMER>> tf = new TaskFactory<IEnumerable<CUSTOMER>>();
            //customerDsq = (DataServiceQuery<CUSTOMER>)(from user in ctx.CUSTOMER where user.NAME.Equals(InputUsername.Text) select user);
            //IEnumerable<CUSTOMER> cs = await tf.FromAsync(customerDsq.BeginExecute(null, null), iar => customerDsq.EndExecute(iar));
            //csl = new List<CUSTOMER>(cs);
            Constants.Save<string>("LastUser", NAME);
            ImageSource = (c.EMAIL != null) ? "http://www.gravatar.com/avatar/" + Constants.ComputeMD5(c.EMAIL) + "?s=400" : "";
            CreateDBAndInsert();
        }

        /// <summary>
        /// Sets the attend teach number.
        /// </summary>
        public async void SetAttendTeachNumber()
        {
            try
            {
                ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
                teachDsq = (DataServiceQuery<COURSE_AVAIL>)(from course in ctx.COURSE_AVAIL
                                                            where course.TEACHER_NAME == this.NAME
                                                            select course);
                TaskFactory<IEnumerable<COURSE_AVAIL>> tf = new TaskFactory<IEnumerable<COURSE_AVAIL>>();
                IEnumerable<COURSE_AVAIL> attends = await tf.FromAsync(ctx.BeginExecute<COURSE_AVAIL>(
                    new Uri("/GetAllCoursesAttendedByCustomer?customer_id=" + this.ID, UriKind.Relative), null, null),
                    iar => ctx.EndExecute<COURSE_AVAIL>(iar));
                ATTEND_COUNT = attends.Count();
                IEnumerable<COURSE_AVAIL> teaches = await tf.FromAsync(teachDsq.BeginExecute(null, null), iar => teachDsq.EndExecute(iar));
                TEACH_COUNT = teaches.Count();
            }
            catch
            {
                ShowMessageDialog("Set Attend Teach Number ");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="un">The un.</param>
        /// <param name="ims">The ims.</param>
        public User(string un, string ims)
        {
            NAME = un;
            ImageSource = ims;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        { }

        /// <summary>
        /// Creates the database and insert.
        /// </summary>
        private void CreateDBAndInsert()
        {
            using (SQLiteConnection db = CreateSQLiteConnection())
            {
                //db.DropTable<User>();
                db.CreateTable<User>();
                if (db.Table<User>().Count() != 0)
                {
                    //    db.DeleteAll<User>();
                }
                System.Diagnostics.Debug.WriteLine("Count of table after deleteall: {0}", db.Table<User>().Count());
                db.InsertOrReplace(this);
                db.Close();
            }
        }

        /// <summary>
        /// Creates the sq lite connection.
        /// </summary>
        /// <returns></returns>
        private static SQLiteConnection CreateSQLiteConnection()
        {
            var dbPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "User.db");
            return new SQLite.SQLiteConnection(dbPath);
        }

        /// <summary>
        /// Selects this instance.
        /// </summary>
        /// <returns></returns>
        public static List<User> Select()
        {
            SQLiteConnection db = User.CreateSQLiteConnection();
            return db.Query<User>("select * from User");
        }

        /// <summary>
        /// Selects the last user.
        /// </summary>
        /// <returns></returns>
        public static User SelectLastUser()
        {
            SQLiteConnection db = User.CreateSQLiteConnection();
            User u = null;
            try
            {
                u = db.Query<User>("select * from User where NAME =? ", Constants.Read<string>("LastUser"))[0];
            }
            catch (SQLiteException e)
            {
                System.Diagnostics.Debug.WriteLine("in SelectLastUser Function error:{0}", e.Message);
            }
            //u.SetAttendTeachNumber();
            return u;
        }

        /// <summary>
        /// Shows the message dialog.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        private async void ShowMessageDialog(String msg = "No Network has beeeeeen found!")
        {
            try
            {
                var messageDialog = new MessageDialog(msg);

                messageDialog.Commands.Add(new UICommand("Close"));
                //loadingProgressRing.IsActive = false;
                await messageDialog.ShowAsync();
            }
            catch
            {
                //ShowMessageDialog();
            }
        }
    }
}
