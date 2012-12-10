namespace SharedSource.YouTubeUpload.Util
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Google.YouTube;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Google.GData.Client;

    class YouTubeStats
    {

        string devKey = string.Empty;
        string clientID = string.Empty;
        string username = string.Empty;
        string password = string.Empty;
        string handleID = string.Empty;
        Sitecore.Data.Database master;
        string videoID;
        

        public YouTubeStats(string VideoID) 
        {
            this.videoID = VideoID;
            master = Factory.GetDatabase("master");
            Item settings = master.GetItem(Data.YouTubeSettingsID);

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


                GetStats();
            }
        }

        public int CommentCount
        {
            get;
            set;
        }

        public int Views
        {
            get;
            set;
        }

        public int Rating
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public DateTime Updated
        {
            get;
            set;
        }

        public string Link
        {
            get;
            set;
        }


        public YouTubeRequest GetRequest()
        {            
            YouTubeRequestSettings settings = new YouTubeRequestSettings("SitecoreYouTubeUploader", devKey, username, password);
            settings.AutoPaging = true;
            YouTubeRequest request = new YouTubeRequest(settings);
            request = new YouTubeRequest(settings);
            return request;
        }

        public void GetStats()
        {
            YouTubeRequest request = GetRequest();

            Uri uri = new Uri("https://gdata.youtube.com/feeds/api/videos/" + this.videoID + "?v=2");

            try
            {
                Feed<Video> feed = request.Get<Video>(uri);
                foreach (Video entry in feed.Entries)
                {
                    this.CommentCount = entry.CommmentCount;
                    this.Views = entry.ViewCount;
                    this.Title = entry.Title;
                    this.Updated = entry.Updated;
                    this.Link = entry.WatchPage.ToString();
                }
            }
            catch
            { }

        }
    }
}
