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

    class UploadVideoForm :  ApplicationForm 
    {
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

        Sitecore.Data.Database master;
        Item item;

        protected override void OnLoad(EventArgs e)
        {
            master = Factory.GetDatabase("master");

            item = GetCurrentItem();

            if (!Context.ClientPage.IsEvent)
            {
                

                VideoTitle.Value = item.Fields["Title"].Value;
                VideoDescription.Value = item.Fields["Description"].Value;
                VideoKeywords.Value = item.Fields["Keywords"].Value;
                ItemId.Value = item.ID.ToString();
            }
            base.OnLoad(e);
        }

        protected void StartUploadVideo()
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
                Status.Text = "Upload Completed";
                StatusPC.Text = "";
                Progress.Visible = false;

 
                item.Editing.BeginEdit();
                item.Fields["VideoID"].Value = job.Status.Messages[1];
                item.Editing.EndEdit();

                String refresh = String.Format("item:refreshchildren(id={0})", item.Parent.ID);
                Sitecore.Context.ClientPage.SendMessage(this, refresh);

            }
        }


        private Item GetCurrentItem()
        {
            string queryString = WebUtil.GetQueryString("id");
            return master.GetItem(new ID(queryString));
        }
    }
}
