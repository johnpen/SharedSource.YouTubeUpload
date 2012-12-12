namespace SharedSource.YouTubeUpload.Layouts.modules.youtube
{
    using System;
    using System.Collections.Specialized;
    using Sitecore.Data.Items;
    using Sitecore.Web.UI.WebControls;
    using Sitecore.Web;

    public partial class YouTubeVideo : System.Web.UI.UserControl
    {
        private NameValueCollection properties;
        private Item dataItem;

        protected string DataSource
        {
            get;
            set;
        }

        #region Public Properties

        /// <summary>
        /// YouTube ID
        /// </summary>
        public string VideoID { get; set; }
        /// <summary>
        /// Video Width
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Video Height
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Auto Hide Video Controls
        /// </summary>
        public int AutoHide { get; set; }
        /// <summary>
        /// Start playing video on load
        /// </summary>
        public int AutoPlay { get; set; }
        /// <summary>
        /// Tuen off keyboards support
        /// </summary>
        public int DisableKeyboard { get; set; }
        /// <summary>
        /// Show full screen button
        /// </summary>
        public int AllowFullScreen { get; set; }
        /// <summary>
        /// Keep looping the video
        /// </summary>
        public int Loop { get; set; }
        /// <summary>
        /// Number of seconds to skip from the begining
        /// </summary>
        public int StartTime { get; set; }
        /// <summary>
        /// Number of seconds to stop the video
        /// </summary>
        public int EndTime { get; set; }
        /// <summary>
        /// Don't show google branding
        /// </summary>
        public int ModestBranding { get; set; }

        #endregion



        public Item SourceItem
        {
            get
            {
                if (dataItem == null)
                {
                    if (!string.IsNullOrEmpty(DataSource))
                    {
                        dataItem = Sitecore.Context.Database.GetItem(DataSource);
                    }
                }
                return dataItem;
            }
        }

        private void Page_Load(object sender, EventArgs e)
        {
            Item item = SourceItem;


            if (item != null)
            {
                this.VideoID = item.Fields[Data.YouTubeIDFieldID].Value;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            var sublayout = (Sublayout)Parent;
            if (sublayout != null)
            {
                DataSource = sublayout.DataSource;

                string parameters = Attributes["sc_parameters"];
                if (string.IsNullOrEmpty(parameters))
                {
                    parameters = sublayout.Parameters;
                }

                properties = WebUtil.ParseUrlParameters(parameters);

                this.Width = TryParseIntDefault(properties["Width"],640);
                this.Height = TryParseIntDefault(properties["Height"], 390);
                this.AutoHide = TryParseBoolIntDefault(properties["Auto Hide"], true);
                this.AutoPlay = TryParseBoolIntDefault(properties["Auto Play"], false);
                this.DisableKeyboard = TryParseBoolIntDefault(properties["Disable Keyboard"], false);
                this.AllowFullScreen = TryParseBoolIntDefault(properties["Allow Full Screen"], true);
                this.Loop = TryParseBoolIntDefault(properties["Allow Full Screen"], true);
                this.StartTime = TryParseIntDefault(properties["Start Time"], 0);
                this.ModestBranding = TryParseBoolIntDefault(properties["Modest Branding"], true);
            }
        }


        public int TryParseIntDefault(string s, int defaultValue)
        {
            int newValue;
            if (int.TryParse(s, out newValue))
            {
                return newValue;
            }
            return defaultValue;
        }

        public int TryParseBoolIntDefault(string s, bool defaultValue)
        {
            bool newValue;
            if (! bool.TryParse(s, out newValue))
            {
                newValue = defaultValue;
            }
            if (defaultValue)
            {
                return 1;
            }
            return 0;
        }
    }
}