using CloudEDU.Common;
using CloudEDU.Service;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// 
    /// </summary>
    class DBAccessAPIs
    {
        /// <summary>
        /// The CTX
        /// </summary>
        private CloudEDUEntities ctx = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="DBAccessAPIs"/> class.
        /// </summary>
        public DBAccessAPIs()
        {
            ctx = new CloudEDUEntities(new Uri(Constants.DataServiceURI));
        }


        /// <summary>
        /// The customer DSQ
        /// </summary>
        public DataServiceQuery<CUSTOMER> customerDsq = null;
        /// <summary>
        /// The lesson DSQ
        /// </summary>
        public DataServiceQuery<LESSON> lessonDsq = null;
        /// <summary>
        /// The resource DSQ
        /// </summary>
        public DataServiceQuery<RESOURCE> resourceDsq = null;
        /// <summary>
        /// The note DSQ
        /// </summary>
        public DataServiceQuery<NOTE> noteDsq = null;
        /// <summary>
        /// The resource type DSQ
        /// </summary>
        public DataServiceQuery<RES_TYPE> resTypeDsq = null;
        /// <summary>
        /// The course DSQ
        /// </summary>
        public DataServiceQuery<COURSE> courseDsq = null;
        /// <summary>
        /// The teach DSQ
        /// </summary>
        public DataServiceQuery<COURSE_AVAIL> teachDsq = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public delegate string deleMethod(int a, string b);

        /// <summary>
        /// Tests the specified m.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static string test(deleMethod m)
        {
            return m(1, "aaa");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result">The result.</param>
        public delegate void OnQueryComplete(IAsyncResult result);
        /// <summary>
        /// The on uqc
        /// </summary>
        private OnQueryComplete onUQC;



        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void getUserById(int id, OnQueryComplete onComplete)
        {
            customerDsq = (DataServiceQuery<CUSTOMER>)(from cus in ctx.CUSTOMER where cus.ID == Constants.User.ID select cus);
            this.onUQC = onComplete;
            customerDsq.BeginExecute(onQueryComplete2, null);
        }


        /// <summary>
        /// Ons the query complete2.
        /// </summary>
        /// <param name="result">The result.</param>
        private void onQueryComplete2(IAsyncResult result)
        {
            this.onUQC(result);
        }



        /// <summary>
        /// Gets the lesson by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetLessonById(int id, OnQueryComplete onComplete)
        {
            lessonDsq = (DataServiceQuery<LESSON>)(from les in ctx.LESSON where les.ID == id select les);
            this.onUQC = onComplete;
            lessonDsq.BeginExecute(onQueryComplete2, null);
        }

        /// <summary>
        /// Gets the lessons by course identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetLessonsByCourseId(int id, OnQueryComplete onComplete)
        {
            lessonDsq = (DataServiceQuery<LESSON>)(from les in ctx.LESSON where les.COURSE_ID == id select les);
            this.onUQC = onComplete;
            lessonDsq.BeginExecute(onQueryComplete2, null);
        }


        /// <summary>
        /// Gets the learned lessons in course by customer identifier.
        /// </summary>
        /// <param name="customer_id">The customer_id.</param>
        /// <param name="course_id">The course_id.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetLearnedLessonsInCourseByCustomerId(int customer_id, int course_id, OnQueryComplete onComplete)
        {
            //lessonDsq = (DataServiceQuery<LESSON>(from les in ctx.LESSON where les.COURSE_ID == customer_id and les))
        }



        /// <summary>
        /// Gets the resources by lesson identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetResourcesByLessonId(int id, OnQueryComplete onComplete)
        {
            resourceDsq = (DataServiceQuery<RESOURCE>)(ctx.RESOURCE.Where(r => r.LESSON_ID == id));
            this.onUQC = onComplete;
            resourceDsq.BeginExecute(onQueryComplete2, null);
        }

        /// <summary>
        /// Gets the note by customer identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetNoteByCustomerID(int id, OnQueryComplete onComplete)
        {
            noteDsq = (DataServiceQuery<NOTE>)(ctx.NOTE.Where(n => n.CUSTOMER_ID == id));
            this.onUQC = onComplete;
            noteDsq.BeginExecute(onQueryComplete2, null);
        }

        /// <summary>
        /// Gets the type by resource identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetTypeByResourceID(int id, OnQueryComplete onComplete)
        {
            resourceDsq = (DataServiceQuery<RESOURCE>)(ctx.RESOURCE.Where(r => r.ID == id));
            this.onUQC = onComplete;
            resourceDsq.BeginExecute(onResComplete, null);
        }


        /// <summary>
        /// Gets the note by lesson identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetNoteByLessonId(int id, OnQueryComplete onComplete)
        {
            noteDsq = (DataServiceQuery<NOTE>)(from note in ctx.NOTE where note.LESSON_ID == id select note);
            this.onUQC = onComplete;
            noteDsq.BeginExecute(onQueryComplete2, null);
        }

        /// <summary>
        /// Gets the resource by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetResourceByID(int id, OnQueryComplete onComplete)
        {
            resourceDsq = (DataServiceQuery<RESOURCE>)(ctx.RESOURCE.Where(r => r.ID == id));
            this.onUQC = onComplete;
            resourceDsq.BeginExecute(onQueryComplete2, null);
        }

        /// <summary>
        /// Ons the resource complete.
        /// </summary>
        /// <param name="res">The resource.</param>
        private void onResComplete(IAsyncResult res)
        {
            IEnumerable<RESOURCE> resources = resourceDsq.EndExecute(res);
            RESOURCE resource = resources.FirstOrDefault();
            resTypeDsq = (DataServiceQuery<RES_TYPE>)(ctx.RES_TYPE.Where(r => r.ID == resource.TYPE));
            resTypeDsq.BeginExecute(onQueryComplete2, null);
        }

        /// <summary>
        /// Inserts the note.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <param name="onComplete">The on complete.</param>
        public void InsertNote(NOTE note, OnQueryComplete onComplete)
        {
            ctx.AddToNOTE(note);
            this.onUQC = onComplete;
            ctx.BeginSaveChanges(onQueryComplete2, null);
        }

        /// <summary>
        /// Edits the note.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <param name="onComplete">The on complete.</param>
        public void EditNote(NOTE note, OnQueryComplete onComplete)
        {
            ctx.UpdateObject(note);
            this.onUQC = onComplete;
            ctx.BeginSaveChanges(onQueryComplete2, null);
        }

        /// <summary>
        /// Edits the lesson.
        /// </summary>
        /// <param name="lesson">The lesson.</param>
        /// <param name="onComplete">The on complete.</param>
        public void EditLesson(LESSON lesson, OnQueryComplete onComplete)
        {
            ctx.UpdateObject(lesson);
            this.onUQC = onComplete;
            ctx.BeginSaveChanges(onQueryComplete2, null);
        }

        /// <summary>
        /// Edits the course.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <param name="onComplete">The on complete.</param>
        public void EditCourse(COURSE course, OnQueryComplete onComplete)
        {
            ctx.UpdateObject(course);
            this.onUQC = onComplete;
            ctx.BeginSaveChanges(onQueryComplete2, null);
        }

        /// <summary>
        /// Deletes the note.
        /// </summary>
        /// <param name="note">The note.</param>
        /// <param name="onComplete">The on complete.</param>
        public void DeleteNote(NOTE note, OnQueryComplete onComplete)
        {
            ctx.DeleteObject(note);
            this.onUQC = onComplete;
            ctx.BeginSaveChanges(onQueryComplete2, null);
        }

        /// <summary>
        /// Gets the note by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetNoteByID(int id, OnQueryComplete onComplete)
        {
            noteDsq = (DataServiceQuery<NOTE>)(ctx.NOTE.Where(r => r.ID == id));
            this.onUQC = onComplete;
            resourceDsq.BeginExecute(onResComplete, null);
        }

        /// <summary>
        /// Gets the lesson by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetLessonByID(int id, OnQueryComplete onComplete)
        {
            lessonDsq = (DataServiceQuery<LESSON>)(ctx.LESSON.Where(r => r.ID == id));
            this.onUQC = onComplete;
            resourceDsq.BeginExecute(onResComplete, null);
        }

        /// <summary>
        /// Gets the course by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="onComplete">The on complete.</param>
        public void GetCourseByID(int id, OnQueryComplete onComplete)
        {
            courseDsq = (DataServiceQuery<COURSE>)(ctx.COURSE.Where(r => r.ID == id));
            this.onUQC = onComplete;
            resourceDsq.BeginExecute(onResComplete, null);
        }


        //public void GetAttendedCourseAvailByName(String name,)
    }
}
