using CloudEDU.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// 
    /// </summary>
    public class StoreData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StoreData"/> class.
        /// </summary>
        public StoreData()
        {
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return Collection.Count();
        }

        /// <summary>
        /// The _collection
        /// </summary>
        private CourseCollection _collection = new CourseCollection();
        /// <summary>
        /// Gets the collection.
        /// </summary>
        /// <value>
        /// The collection.
        /// </value>
        public CourseCollection Collection
        {
            get
            {
                return _collection;
            }
        }

        #region Add
        /// <summary>
        /// Add course to the list used to display.
        /// </summary>
        /// <param name="course">The course.</param>
        public void AddCourse(Course course)
        {
            Collection.Add(course);
        }

        /// <summary>
        /// Add a group of courses to the list used to display.
        /// </summary>
        /// <param name="courses">The courses.</param>
        public void AddCourses(List<Course> courses)
        {
            foreach (Course course in courses)
            {
                Collection.Add(course);
            }
        }
        #endregion

        #region Different ways used to get the groups

        //internal List<GroupInfoList<object>> GetSearchResultBySearchString(string searchString)
        //{
        //    List<GroupInfoList<object>> group = new List<GroupInfoList<object>>();


        //}

        /// <summary>
        /// Get the single group classified by the category Title.
        /// </summary>
        /// <param name="categoryTitle">The category title.</param>
        /// <returns>
        /// A list only contain a single GroupInfoList, which contain the
        /// elements that had been grouped.
        /// </returns>
        internal List<GroupInfoList<object>> GetSingleGroupByCategoryTitle(string categoryTitle)
        {
            List<GroupInfoList<object>> group = new List<GroupInfoList<object>>();

            var query = from course in Collection
                        where ((Course)course).Category == categoryTitle
                        group course by ((Course)course).Category into g
                        select new { GroupTitle = g.Key, Courses = g };

            foreach (var g in query)
            {
                GroupInfoList<object> info = new GroupInfoList<object>();
                info.Key = g.GroupTitle;
                foreach (var course in g.Courses)
                {
                    info.Add(course);
                }
                group.Add(info);
            }

            return group;
        }

        /// <summary>
        /// Get the list that has been grouped by the category.
        /// </summary>
        /// <returns>
        /// A list of GroupInfoList, each GroupInfoList contains the data
        /// classified according to the category.
        /// </returns>
        internal List<GroupInfoList<object>> GetGroupsByCategory()
        {
            List<GroupInfoList<object>> groups = new List<GroupInfoList<object>>();

            var query = from course in Collection
                        group course by ((Course)course).Category into g
                        select new { GroupTitle = g.Key, Courses = g };

            foreach (var g in query)
            {
                GroupInfoList<object> info = new GroupInfoList<object>();
                info.Key = g.GroupTitle;
                if (Constants.CategoryNameList.Contains(info.Key))
                {
                    info.CategoryImg = "ms-appx:///Images/Category/" + info.Key + ".jpg";
                }
                else
                {
                    info.CategoryImg = Constants.RecUriDic[info.Key as string];
                }

                Random ran = new Random();
                int random = ran.Next(3, 6);
                int size = 0;
                if (random > g.Courses.Count())
                {
                    size = g.Courses.Count();
                }
                else
                {
                    size = random;
                }

                switch (size)
                {
                    case 1:
                        {
                            (g.Courses.ElementAt(0) as Course).ItemContainerType = GridViewItemContainerType.SquareGridViewItemContainerSize;
                            break;
                        }
                    case 2:
                        {
                            (g.Courses.ElementAt(0) as Course).ItemContainerType = GridViewItemContainerType.DoubleHeightGridViewItemContainerSize;
                            (g.Courses.ElementAt(1) as Course).ItemContainerType = GridViewItemContainerType.DoubleHeightGridViewItemContainerSize;
                            break;
                        }
                    case 3:
                        {
                            (g.Courses.ElementAt(0) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(1) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(2) as Course).ItemContainerType = GridViewItemContainerType.SquareGridViewItemContainerSize;
                            break;
                        }
                    case 4:
                        {
                            (g.Courses.ElementAt(0) as Course).ItemContainerType = GridViewItemContainerType.DoubleWidthGridViewItemContsinerSize;
                            (g.Courses.ElementAt(1) as Course).ItemContainerType = GridViewItemContainerType.DoubleHeightGridViewItemContainerSize;
                            (g.Courses.ElementAt(2) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(3) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            break;
                        }
                    case 5:
                        {
                            (g.Courses.ElementAt(0) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(1) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(2) as Course).ItemContainerType = GridViewItemContainerType.DoubleWidthGridViewItemContsinerSize;
                            (g.Courses.ElementAt(3) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(4) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            break;
                        }
                    case 6:
                        {
                            (g.Courses.ElementAt(0) as Course).ItemContainerType = GridViewItemContainerType.DoubleHeightGridViewItemContainerSize;
                            (g.Courses.ElementAt(1) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(2) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(3) as Course).ItemContainerType = GridViewItemContainerType.DoubleWidthGridViewItemContsinerSize;
                            (g.Courses.ElementAt(4) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            (g.Courses.ElementAt(5) as Course).ItemContainerType = GridViewItemContainerType.DefaultGridViewItemContainerSize;
                            break;
                        }
                }
                for (int i = 0; i < size; ++i)
                {
                    var c = g.Courses.ElementAt(i);
                    info.Add(c);
                }
                groups.Add(info);
            }

            return groups;
        }



        /// <summary>
        /// Get the list that has been grouped by whether user has bought or teached.
        /// </summary>
        /// <returns>
        /// A list contains two GroupInfoList, one is attending list, another
        /// is teaching list.
        /// </returns>
        internal List<GroupInfoList<object>> GetGroupsByAttendingOrTeaching()
        {
            List<GroupInfoList<object>> groups = new List<GroupInfoList<object>>();

            GroupInfoList<object> attending = new GroupInfoList<object>();
            GroupInfoList<object> teaching = new GroupInfoList<object>();
            attending.Key = "Attending";
            teaching.Key = "Teaching";

            foreach (Course g in Collection)
            {
                if (g.IsBuy)
                {
                    attending.Add(g);
                }
                if (g.IsTeach)
                {
                    teaching.Add(g);
                }
            }

            groups.Add(attending);
            groups.Add(teaching);

            return groups;
        }

        /// <summary>
        /// Gets the search result group.
        /// </summary>
        /// <param name="searchCount">The search count.</param>
        /// <returns></returns>
        internal List<GroupInfoList<object>> GetSearchResultGroup(string searchCount)
        {
            List<GroupInfoList<object>> group = new List<GroupInfoList<object>>();

            GroupInfoList<object> searchResult = new GroupInfoList<object>();
            searchResult.Key = searchCount + " Result";

            foreach (Course g in Collection)
            {
                searchResult.Add(g);
            }

            group.Add(searchResult);

            return group;
        }
        #endregion
    }
}
