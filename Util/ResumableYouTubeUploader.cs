namespace SharedSource.YouTubeUpload.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Diagnostics;

    using Google.GData.Client;
    using Google.GData.YouTube;
    using Google.YouTube;
    using Google.GData.Extensions.MediaRss;
    using Google.GData.Client.ResumableUpload;

    class ResumableYouTubeUploader
    {
        private ResumableUploader ru = null;

        public ResumableYouTubeUploader()
        {
            EnsureRU();

 
        }

        public void UploadVideo(YouTubeEntry entry, Authenticator authentication, string userName)
        {
            UserState u = new UserState();
            u.RetryCounter = 0;

            AtomLink link = new AtomLink("http://uploads.gdata.youtube.com/resumable/feeds/api/users/default/uploads");
            link.Rel = ResumableUploader.CreateMediaRelation;
            entry.Links.Add(link);
           // ru.Insert(authentication, entry);
            ru.InsertAsync(authentication, entry, u);  
        }

        // helper to create a ResumableUploader object and setup the event handlers
        private void EnsureRU()
        {
            this.ru = new ResumableUploader(25);
            this.ru.AsyncOperationCompleted += new AsyncOperationCompletedEventHandler(ru_AsyncOperationCompleted);
            this.ru.AsyncOperationProgress += new AsyncOperationProgressEventHandler(ru_AsyncOperationProgress);
        }

        public void ru_AsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
        {

           // throw new NotImplementedException();
        }

        public void ru_AsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
        {
          //  throw new NotImplementedException();
        }

    }



}
