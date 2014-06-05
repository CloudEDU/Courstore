using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace CloudEDU.CourseStore
{
    /// <summary>
    /// 
    /// </summary>
    public static class StorageFolderExtension
    {
        /// <summary>
        /// Checks the file existed.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static async Task<bool> CheckFileExisted(this StorageFolder folder, string fileName)
        {
            return folder != null &&
                   (await (
                              folder.CreateFileQueryWithOptions(
                                  new QueryOptions
                                  {
                                      FolderDepth = FolderDepth.Shallow,
                                      UserSearchFilter = "System.FileName:\"" + fileName + "\""
                                  })).GetFilesAsync()).Count > 0
                       ? true
                       : false;
        }
    }
}
