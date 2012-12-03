namespace SharedSource.YouTubeUpload.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;

    public static class CopyStream
    {
        /// <summary>
        /// YouTube API didn't like the sream from item.GetMediaStream(), copying it in to a memory stream sorts the issue 
        /// </summary>
        /// <param name="mediaItemStream"></param>
        /// <returns></returns>
        public static MemoryStream CopyMediaItemStream(Stream mediaItemStream)
        {
            var fileSize = mediaItemStream.Length;
            byte[] buffer = new byte[(int)fileSize];

            mediaItemStream.Read(buffer, 0, (int)fileSize);
            mediaItemStream.Close();

            MemoryStream memStream = new MemoryStream();
            memStream.Write(buffer, 0, buffer.Length);

            return memStream;
        }
    }
}
