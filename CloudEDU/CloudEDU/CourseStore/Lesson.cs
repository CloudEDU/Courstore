using System.Collections.Generic;

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// 
    /// </summary>
    class Lesson
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        public int Number { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }
        /// <summary>
        /// The docs
        /// </summary>
        private List<Resource> docs;
        /// <summary>
        /// The audios
        /// </summary>
        private List<Resource> audios;
        /// <summary>
        /// The videos
        /// </summary>
        private List<Resource> videos;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lesson"/> class.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        public Lesson(int number, string title, string content)
        {
            Number = number;
            Title = title;
            Content = content;
            docs = new List<Resource>();
            audios = new List<Resource>();
            videos = new List<Resource>();
        }

        /// <summary>
        /// Gets the document list.
        /// </summary>
        /// <returns></returns>
        public List<Resource> GetDocList()
        {
            return docs;
        }

        /// <summary>
        /// Gets the audio list.
        /// </summary>
        /// <returns></returns>
        public List<Resource> GetAudioList()
        {
            return audios;
        }

        /// <summary>
        /// Gets the video list.
        /// </summary>
        /// <returns></returns>
        public List<Resource> GetVideoList()
        {
            return videos;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class Resource
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public string Uri { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="type">The type.</param>
        public Resource(string title, string uri, string type)
        {
            Title = title;
            Uri = uri;
            Type = type;
        }
    }
}
