<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YouTubeVideo.ascx.cs" Inherits="SharedSource.YouTubeUpload.Layouts.modules.youtube.YouTubeVideo" %>
<%@ Register TagPrefix="sc" Namespace="Sitecore.Web.UI.WebControls" Assembly="Sitecore.Kernel" %>

<div id="ytplayer"></div>

<script>
    // Load the IFrame Player API code asynchronously.
    var tag = document.createElement('script');
    tag.src = "https://www.youtube.com/player_api";
    var firstScriptTag = document.getElementsByTagName('script')[0];
    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

    // Replace the 'ytplayer' element with an <iframe> and
    // YouTube player after the API code downloads.
    var player;
    function onYouTubePlayerAPIReady() {
        player = new YT.Player('ytplayer', {
            height: '<%=this.Height%>',
            width: '<%=this.Width%>',
            videoId: '<%=this.VideoID%>',
            loop : <%=this.AutoHide%>,
            autoplay : <%=this.AutoPlay%>,
            disablekb : <%=this.DisableKeyboard%>,
            fs : <%=this.AllowFullScreen%>,
            modestbranding :<%=this.ModestBranding%>,
            start :<%=this.StartTime%>
        });
    }
</script>