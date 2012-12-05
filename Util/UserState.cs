using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedSource.YouTubeUpload.Util
{
    /// <summary>
    /// userstate is the object that is passed to the async code to identify a particular upload
    /// this object remembers the http operation, the row in the spreadsheet, progress, etc.
    /// This object get's added to the the current queue, or the retryqueue
    /// </summary>
    internal class UserState
    {
        private long currentPosition;
        private string httpVerb;
        private Uri resumeUri;
        private int retryCounter;
        private string errorText;

        internal UserState()
        {
            this.currentPosition = 0;
            this.retryCounter = 0;
        }

        internal long CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
            set
            {
                this.currentPosition = value;
            }
        }

        internal string Error
        {
            get
            {
                return this.errorText;
            }
            set
            {
                this.errorText = value;
            }
        }

        internal int RetryCounter
        {
            get
            {
                return this.retryCounter;
            }
            set
            {
                this.retryCounter = value;
            }
        }

        internal string HttpVerb
        {
            get
            {
                return this.httpVerb;
            }
            set
            {
                this.httpVerb = value;
            }
        }

        internal Uri ResumeUri
        {
            get
            {
                return this.resumeUri;
            }
            set
            {
                this.resumeUri = value;
            }
        }
    }
}
