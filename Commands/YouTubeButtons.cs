namespace SharedSource.YouTubeUpload.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Pipelines;
    using Sitecore.Resources;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Framework.Scripts;
    using Sitecore.Web.UI.Sheer;
    


    public class YouTubeButtons : Command
    {
        public override void Execute([NotNull] CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");
            var item = context.Items[0];

            if (WebUtil.GetFormValue("scEditorTabs").Contains("youtube:tab:show"))
            {
                SheerResponse.Eval("scContent.onEditorTabClick(null, null, 'OpenSocialCenter')");
            }
            else
            {
                if (item == null)
                {
                    SheerResponse.Alert("You don't have permission to upload this video", new string[0]);
                }
                else
                {
                /*    UrlString url = new UrlString(string.Concat(new object[] { "/sitecore modules/youtube/youtubeupload.aspx?id=", item.ID }));
                    item.Uri.AddToUrlString(url);
                    UIUtil.AddContentDatabaseParameter(url); */

                    UrlString str = new UrlString(UIUtil.GetUri("control:UploadYouTubeVideo"));
                    str["id"] = item.ID.ToString();

                    ShowEditorTab tab = new ShowEditorTab();
                    tab.Command = "youtube:tab:show";
                    tab.Header = "Upload Video to YouTube";
                    tab.Url = str.ToString();
                    tab.Icon = "/sitecore modules/youtube/images/youtube.png";
                    tab.Id = "YouTubeUploadTab";
                    tab.Closeable = true;
                    SheerResponse.Eval(tab.ToString());
                }
            }

        }

        public override CommandState QueryState(CommandContext context)
        {
            Item item = context.Items[0];

            if (item != null && item.TemplateID == Data.YouTubeFolderID)
            {
                return CommandState.Enabled;
            }
            else
            {
                return CommandState.Disabled;
            }
        }

    }
}
