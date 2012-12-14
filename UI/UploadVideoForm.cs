namespace SharedSource.YouTubeUpload.UI
{
    using System;
    using System.Threading;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.Jobs;
    using Sitecore.Web;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Pages;
    using Sitecore.Web.UI.Sheer;
    using Sitecore.Web.UI.WebControls;
    using Sitecore.SecurityModel;
    using Google.YouTube;

    class UploadVideoForm :   DialogForm
    {
        #region proteced UI elements
        
        protected Edit VideoTitle;
        protected Edit VideoDescription;
        protected Edit VideoKeywords;
        protected Listbox VideoCategory;
        protected Border Progress;
        protected Border StartButton;
        protected Border form;
        protected GridPanel Grid;
        protected Literal StatusPC;
        protected Literal Status;
        protected Edit HandleId;
        protected Edit ItemId;
        protected Border ProgressText;
        protected Border VideoStatus;
        protected Literal VideoTitleText;
        protected Literal VideoIframe;
        protected Literal VideoInfo;
        protected Literal VideoViews;
        protected Literal VideoComments;
        protected Literal VideoUploaded;
        protected Literal VideoLink;
        #endregion

        Sitecore.Data.Database master;
        Item item;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.OK.Visible = false;
        }


        protected override void OnLoad(EventArgs e)
        {
            master = Factory.GetDatabase("master");

            item = GetCurrentItem();

            if (!Context.ClientPage.IsEvent)
            {
                if (string.IsNullOrEmpty(item.Fields["VideoID"].Value))
                {
                    VideoTitle.Value = item.Fields["Title"].Value;
                    VideoDescription.Value = item.Fields["Description"].Value;
                    VideoKeywords.Value = item.Fields["Keywords"].Value;
                    ItemId.Value = item.ID.ToString();
                    VideoStatus.Visible = false;
                }
                else
                {
                    form.Visible = false;
                    StartButton.Visible = false;
                    VideoStatus.Visible = true;
                    this.YouTubeVideoID = item.Fields["VideoID"].Value;

                    DisplayVideoStatus();
                }
            }
            base.OnLoad(e);
        }

        protected void StartUploadVideo()
        {
            // Validate
            if (this.ValidateFormInput())
            {
                Progress.Visible = true;
                ProgressText.Visible = true;
                form.Visible = false;
                StartButton.Visible = false;

                Status.Text = "Starting Upload....";
                var job = JobManager.Start(new JobOptions("YouTubeUpload", "Video Upload", Context.Site.Name, this, "UploadVideo"));
                HandleId.Value = job.Handle.ToString();
                SheerResponse.Timer("CheckStatus", 500);
            }
        }


        protected void UploadVideo()
        {
            var proc = new Util.UploadVideoToYouTube();
            proc.Execute(VideoTitle.Value, VideoDescription.Value, VideoKeywords.Value, VideoCategory.SelectedItem.Value, new ID(ItemId.Value), HandleId.Value);
        }

        protected void CheckStatus()
        {
            var handle = Handle.Parse(HandleId.Value);
            var job = JobManager.GetJob(handle);
            Status.Text = "Uploading...";


           // StatusPC.Text =  job.Status.Processed.ToString();

            if (!job.IsDone)
            {
                SheerResponse.Timer("CheckStatus", 500);
            }
            else
            {
                if (!job.Status.Failed)
                {
                    Status.Text = "Upload Completed";
                    StatusPC.Text = "";
                    Progress.Visible = false;

                    item.Editing.BeginEdit();
                    item.Fields["VideoID"].Value = job.Status.Messages[1];
                    item.Editing.EndEdit();

                    String refresh = String.Format("item:refreshchildren(id={0})", item.Parent.ID);
                    Sitecore.Context.ClientPage.SendMessage(this, refresh);

                    form.Visible = false;
                    VideoStatus.Visible = true;
                }
                else
                {
                    SheerResponse.ShowError(job.Status.Messages[1],"X");
                }
            }
        }

        public string YouTubeVideoID
        {
            get;
            set;
        }

        protected void DisplayVideoStatus()
        {

            Util.YouTubeStats stats = new Util.YouTubeStats(item.Fields["VideoID"].Value);
            VideoTitleText.Text = stats.Title;
            VideoIframe.Text = "<iframe  width=\"480\" height=\"360\" src=\"http://www.youtube.com/embed/" + this.YouTubeVideoID +  "?rel=0\" frameborder=\"0\" allowfullscreen></iframe>";
            VideoInfo.Text += "<div style=\"float:right;width:200px\">Video Information<br /></div>";
            VideoViews.Text = stats.Views.ToString();
            VideoComments.Text = stats.CommentCount.ToString();
            VideoUploaded.Text = stats.Updated.ToString("dd MMMM yyyy HH:mm");
            VideoLink.Text = "<a href=\"#\" onclick=\"window.open('" + stats.Link + "')\">" + stats.Link + "</a>";
        }



        protected bool ValidateFormInput()
        {
            if (string.IsNullOrEmpty(VideoTitle.Value))
            {
                Sitecore.Context.ClientPage.ClientResponse.Alert("Video Title missing");  
                return false;
            }
            if (VideoTitle.Value.Length < 4)
            {
                Sitecore.Context.ClientPage.ClientResponse.Alert("Video Title is to short");
                return false;
            }

            if (string.IsNullOrEmpty(VideoDescription.Value))
            {
                Sitecore.Context.ClientPage.ClientResponse.Alert("Video Description missing");
                return false;
            }
            if (VideoDescription.Value.Length < 4)
            {
                Sitecore.Context.ClientPage.ClientResponse.Alert("Video Description is to short");
                return false;
            }

            if (string.IsNullOrEmpty(VideoKeywords.Value))
            {
                Sitecore.Context.ClientPage.ClientResponse.Alert("Video Keywords missing");
                return false;
            }
            if (VideoKeywords.Value.Length < 4)
            {
                Sitecore.Context.ClientPage.ClientResponse.Alert("Video Keywords is to short");
                return false;
            }

            return true;
        }


        private Item GetCurrentItem()
        {
            string queryString = WebUtil.GetQueryString("id");
            return master.GetItem(new ID(queryString));
        }
    }
}
