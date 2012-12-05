namespace SharedSource.YouTubeUpload.Util
{
    using System.IO;
    using Google.GData.Client;
    using Google.GData.Client.ResumableUpload;
    using Google.GData.Extensions.MediaRss;
    using Google.GData.YouTube;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using System.Threading;

    public class UploadVideoToYouTube
    {
        string devKey = string.Empty;
        string clientID = string.Empty;
        string username = string.Empty;
        string password = string.Empty;
        string handleID = string.Empty;
        Sitecore.Data.Database master;
        private ResumableUploader uploader = null;
        bool isUploaded = false;


        public void Execute(string title, string description, string keywords, string category, ID item, string handle)
        {
            master = Factory.GetDatabase("master");
            Item settings = master.GetItem(Data.YouTubeSettingsID);
            this.handleID = handle;

            if (settings == null)
            {
                Log.Error("YouTube Settings not found!", this);
                return;
            }
            else
            {
                devKey = settings.Fields[Data.DevKeyFieldID].Value;
                clientID = settings.Fields[Data.ClientIDFieldID].Value;
                username = settings.Fields[Data.UserNameFieldID].Value;
                password = settings.Fields[Data.PasswordFieldID].Value;
                

                EnsureRU();
                UserState u = new UserState();
                u.RetryCounter = 0;

                Authenticator youTubeAuthenticator = new ClientLoginAuthenticator("SitecoreYouTubeUploader", ServiceNames.YouTube, username, password);
                youTubeAuthenticator.DeveloperKey = devKey;

                YouTubeEntry newVideo = new YouTubeEntry();

                newVideo.Media = new Google.GData.YouTube.MediaGroup();
                newVideo.Media.Title = new MediaTitle(title);
                newVideo.Media.Keywords = new MediaKeywords(keywords);
                newVideo.Media.Description = new MediaDescription(description);
                newVideo.Media.Categories.Add(new MediaCategory(category, YouTubeNameTable.CategorySchema));

                MediaItem media = master.GetItem(item);

                Stream mediaStream = media.GetMediaStream();
                MemoryStream uploadStream = CopyStream.CopyMediaItemStream(mediaStream);

                newVideo.MediaSource = new MediaFileSource(uploadStream, media.DisplayName, media.MimeType);

                AtomLink link = new AtomLink("http://uploads.gdata.youtube.com/resumable/feeds/api/users/default/uploads");
                link.Rel = ResumableUploader.CreateMediaRelation;
                newVideo.Links.Add(link);

                // Async Method
                //uploader.InsertAsync(youTubeAuthenticator, newVideo, u);

                var response = uploader.Insert(youTubeAuthenticator, newVideo);
                string id = response.ResponseUri.PathAndQuery;
                id = id.Substring(id.IndexOf("/uploads/") + 9, id.Length - id.IndexOf("/uploads/") - 9);

                Sitecore.Context.Job.Status.Messages.Add(id);
            }
        }


        // helper to create a ResumableUploader object and setup the event handlers
        private void EnsureRU()
        {
            this.uploader = new ResumableUploader(25);
            this.uploader.AsyncOperationCompleted += new AsyncOperationCompletedEventHandler(uploader_AsyncOperationCompleted);
            this.uploader.AsyncOperationProgress += new AsyncOperationProgressEventHandler(uploader_AsyncOperationProgress);
        }

        void uploader_AsyncOperationProgress(object sender, AsyncOperationProgressEventArgs e)
        {
            if (Sitecore.Context.Job != null)
            {
                Sitecore.Context.Job.Status.Processed = e.ProgressPercentage;
                Sitecore.Context.Job.Status.Messages.Add(e.ProgressPercentage.ToString());
            }
        }

        void uploader_AsyncOperationCompleted(object sender, AsyncOperationCompletedEventArgs e)
        {
            
            if (Sitecore.Context.Job != null)
            {
                if (e.Error == null)
                {
                    Sitecore.Context.Job.Status.Processed = -1;
                    Sitecore.Context.Job.Status.Messages.Add(e.Entry.Id.ToString());
                }
                else
                {
                    Sitecore.Context.Job.Status.Processed = -2;
                    Sitecore.Context.Job.Status.Messages.Add("error: " + e.Error.Message);
                }
            }

            isUploaded = true;
        }
    }
}
