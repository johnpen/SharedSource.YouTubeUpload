using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Google.GData.Client;
using Google.GData.Extensions.MediaRss;
using Google.GData.YouTube;
using Google.YouTube;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using SharedSource.YouTubeUpload.Util;
using Google.GData.Client.ResumableUpload;

namespace SharedSource.YouTubeUpload.Web
{
    public partial class YouTubeUpload : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            YouTubeRequestSettings settings = new YouTubeRequestSettings(
                "Sitecore", "AI39si5z3Gh_lWiiSJ-WR70W0PvgXM-NY3sTUcZHRGGCc9byu39NdzE8GXM1Ih82MYa6GQOquTU1vhQBrSfSTWY6Su_OOkP-ew", 
                "john.penfold@gmail.com", "odhran123."
            );

            Authenticator youTubeAuthenticator = new ClientLoginAuthenticator("SitecoreYouTubeUploader", ServiceNames.YouTube, "john.penfold@gmail.com", "odhran123.");
            youTubeAuthenticator.DeveloperKey = "AI39si5z3Gh_lWiiSJ-WR70W0PvgXM-NY3sTUcZHRGGCc9byu39NdzE8GXM1Ih82MYa6GQOquTU1vhQBrSfSTWY6Su_OOkP-ew";

            YouTubeEntry newVideo = new YouTubeEntry();

            newVideo.Media = new Google.GData.YouTube.MediaGroup();
            newVideo.Media.Title = new MediaTitle("test");
            newVideo.Media.Keywords = new MediaKeywords("test");
            newVideo.Media.Description = new MediaDescription("test description");
            newVideo.Media.Categories.Add(new MediaCategory("Autos", YouTubeNameTable.CategorySchema));

            Sitecore.Data.Database master = Factory.GetDatabase("master");

            MediaItem media = master.GetItem(new Sitecore.Data.ID(Request.QueryString["id"]));

            Stream mediaStream = media.GetMediaStream();
            MemoryStream uploadStream = CopyStream.CopyMediaItemStream(mediaStream);

            newVideo.MediaSource = new MediaFileSource(uploadStream, media.DisplayName, media.MimeType);



           // YouTubeService service = new YouTubeService("SitecoreYoutTubeUploader","AI39si5z3Gh_lWiiSJ-WR70W0PvgXM-NY3sTUcZHRGGCc9byu39NdzE8GXM1Ih82MYa6GQOquTU1vhQBrSfSTWY6Su_OOkP-ew");
          //  service.Credentials = new GDataCredentials("john.penfold@gmail.com", "odhran123.");

          //  service.Upload(newVideo);
           

            var ru = new ResumableYouTubeUploader();
            ru.UploadVideo(newVideo, youTubeAuthenticator, "MrJohnPenfold");

        }



    }

}

