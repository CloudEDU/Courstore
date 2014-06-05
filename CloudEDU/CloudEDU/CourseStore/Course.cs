using CloudEDU.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// Course Model
    /// </summary>
    public class Course : INotifyPropertyChanged
    {
        /// <summary>
        /// 在更改属性值时发生。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// The _id
        /// </summary>
        private int? _id = null;
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int? ID
        {
            get
            {
                return _id.HasValue ? _id.Value : int.MaxValue;
            }
            set
            {
                if (this._id != value)
                {
                    this._id = value;
                    this.OnPropertyChanged("ID");
                }
            }
        }

        /// <summary>
        /// The _price
        /// </summary>
        private decimal? _price = null;
        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>
        /// The price.
        /// </value>
        public decimal? Price
        {
            get
            {
                return _price.HasValue ? _price.Value : decimal.MaxValue;
            }
            set
            {
                if (this._price != value)
                {
                    this._price = value;
                    this.OnPropertyChanged("PRICE");
                }
            }
        }

        /// <summary>
        /// The _rate
        /// </summary>
        private double? _rate = null;
        /// <summary>
        /// Gets or sets the rate.
        /// </summary>
        /// <value>
        /// The rate.
        /// </value>
        public double? Rate
        {
            get
            {
                return _rate.HasValue ? _rate.Value : double.MaxValue;
            }
            set
            {
                if (this._rate != value)
                {
                    this._rate = value;
                    this.OnPropertyChanged("RATE");
                }
            }
        }

        /// <summary>
        /// The _title
        /// </summary>
        private string _title = String.Empty;
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.OnPropertyChanged("TITLE");
                }
            }
        }

        /// <summary>
        /// The _intro
        /// </summary>
        private string _intro = String.Empty;
        /// <summary>
        /// Gets or sets the intro.
        /// </summary>
        /// <value>
        /// The intro.
        /// </value>
        public string Intro
        {
            get
            {
                return _intro;
            }
            set
            {
                if (this._intro != value)
                {
                    this._intro = value;
                    this.OnPropertyChanged("INTRO");
                }
            }
        }

        /// <summary>
        /// The _teacher
        /// </summary>
        private string _teacher = String.Empty;
        /// <summary>
        /// Gets or sets the teacher.
        /// </summary>
        /// <value>
        /// The teacher.
        /// </value>
        public string Teacher
        {
            get
            {
                return _teacher;
            }
            set
            {
                if (this._teacher != value)
                {
                    this._teacher = value;
                    this.OnPropertyChanged("TEACHER");
                }
            }
        }

        /// <summary>
        /// The _category
        /// </summary>
        private string _category = String.Empty;
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (this._category != value)
                {
                    this._category = value;
                    this.OnPropertyChanged("CATEGORY");
                }
            }
        }

        /// <summary>
        /// The _course state
        /// </summary>
        private string _courseState = String.Empty;
        /// <summary>
        /// Gets or sets the state of the course.
        /// </summary>
        /// <value>
        /// The state of the course.
        /// </value>
        public string CourseState
        {
            get
            {
                return _courseState;
            }
            set
            {
                if (this._courseState != value)
                {
                    this._courseState = value;
                    this.OnPropertyChanged("COURSE_STATE");
                }
            }
        }

        /// <summary>
        /// The _PG
        /// </summary>
        private int? _pg = null;
        /// <summary>
        /// Gets or sets the pg.
        /// </summary>
        /// <value>
        /// The pg.
        /// </value>
        public int? PG
        {
            get
            {
                return _pg.HasValue ? _pg.Value : int.MaxValue;
            }
            set
            {
                if (this._pg != value)
                {
                    this._pg = value;
                    this.OnPropertyChanged("PG");
                }
            }
        }

        /// <summary>
        /// The _image URI
        /// </summary>
        private string _imageUri = null;
        /// <summary>
        /// Gets or sets the image URI.
        /// </summary>
        /// <value>
        /// The image URI.
        /// </value>
        public string ImageUri
        {
            get
            {
                return _imageUri;
            }
            set
            {
                if (this._imageUri != value)
                {
                    this._imageUri = value;
                    this.OnPropertyChanged("ICON_URL");
                }
            }
        }

        /// <summary>
        /// The _lesson number
        /// </summary>
        private int? _lessonNum = null;
        /// <summary>
        /// Gets or sets the lesson number.
        /// </summary>
        /// <value>
        /// The lesson number.
        /// </value>
        public int? LessonNum
        {
            get
            {
                return _lessonNum.HasValue ? _lessonNum.Value : 0;
            }
            set
            {
                if (this._lessonNum != value)
                {
                    this._lessonNum = value;
                    this.OnPropertyChanged("LESSON_NUM");
                }
            }
        }

        /// <summary>
        /// The _start time
        /// </summary>
        private DateTime? _startTime = null;
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime
        {
            get
            {
                return _startTime.HasValue ? _startTime.Value : DateTime.Now;
            }
            set
            {
                if (this._startTime != value)
                {
                    this._startTime = value;
                    this.OnPropertyChanged("START_TIME");
                }
            }
        }

        /// <summary>
        /// The _rated user
        /// </summary>
        private int? _ratedUser = null;
        /// <summary>
        /// Gets or sets the rated user.
        /// </summary>
        /// <value>
        /// The rated user.
        /// </value>
        public int? RatedUser
        {
            get
            {
                return _ratedUser.HasValue ? _ratedUser.Value : 0;
            }
            set
            {
                if (this._ratedUser != value)
                {
                    this._ratedUser = value;
                    this.OnPropertyChanged("RATED_USER");
                }
            }
        }

        /// <summary>
        /// The _item container type
        /// </summary>
        private GridViewItemContainerType _itemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
        /// <summary>
        /// Gets or sets the type of the item container.
        /// </summary>
        /// <value>
        /// The type of the item container.
        /// </value>
        public GridViewItemContainerType ItemContainerType
        {
            get
            {
                return _itemContainerType;
            }
            set
            {
                if (this._itemContainerType != value)
                {
                    this._itemContainerType = value;
                    this.OnPropertyChanged("ITEM_CONTAINER_TYPE");
                }
            }
        }

        #region Will be cast off
        /// <summary>
        /// The _is buy
        /// </summary>
        private bool _isBuy = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is buy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is buy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBuy
        {
            get
            {
                return _isBuy;
            }
            set
            {
                if (this._isBuy != value)
                {
                    this._isBuy = value;
                    this.OnPropertyChanged("IsBuy");
                }
            }
        }

        /// <summary>
        /// The _is teach
        /// </summary>
        private bool _isTeach = false;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is teach.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is teach; otherwise, <c>false</c>.
        /// </value>
        public bool IsTeach
        {
            get
            {
                return _isTeach;
            }
            set
            {
                if (this._isTeach != value)
                {
                    this._isTeach = value;
                    this.OnPropertyChanged("IsTeaching");
                }
            }
        }
        #endregion
    }

    // Workaround: data binding works best with an enumeration of objects that does not implement IList
    /// <summary>
    /// 
    /// </summary>
    public class CourseCollection : IEnumerable<object>
    {
        /// <summary>
        /// The course collection
        /// </summary>
        private ObservableCollection<Course> courseCollection = new ObservableCollection<Course>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<object> GetEnumerator()
        {
            return courseCollection.GetEnumerator();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举数。
        /// </summary>
        /// <returns>
        /// 一个可用于循环访问集合的 <see cref="T:System.Collections.IEnumerator" /> 对象。
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified course.
        /// </summary>
        /// <param name="course">The course.</param>
        public void Add(Course course)
        {
            courseCollection.Add(course);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GroupInfoList<T> : List<object>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public object Key { get; set; }
        /// <summary>
        /// Gets or sets the category img.
        /// </summary>
        /// <value>
        /// The category img.
        /// </value>
        public string CategoryImg { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public new IEnumerator<object> GetEnumerator()
        {
            return (IEnumerator<object>)base.GetEnumerator();
        }
    }
}
