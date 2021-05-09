using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Models
{
    public class ResultadoCriacaoFoto
    {
        public List<NewMediaItemResult> newMediaItemResults { get; set; }
        public List<NewMediaItemResult> mediaItemResults { get; set; }

    }

    public class NewMediaItemResult
    {
        public string uploadToken { get; set; }
        public NewMediaItemResultStatus status { get; set; }
        public MediaItem mediaItem { get; set; }
    }

  

    public class MediaItem
    {
        public string id { get; set; }
        public string description { get; set; }
        public string productUrl { get; set; }
        public string mimeType { get; set; }
        public string baseUrl { get; set; }
        public string filename { get; set; }

    }

    public class NewMediaItemResultStatus
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
