using System;
using System.Collections.Generic;
using System.Text;

namespace CV.Mobile.Models
{
    public class SharedAlbum
    {
        public string id { get; set; }
        public string title { get; set; }
        public ShareInfo shareInfo { get; set; }
    }

    public class ShareInfo
    {
        public SharedAlbumOptions sharedAlbumOptions { get; set; }
        public string shareableUrl { get; set; }
        public string shareToken { get; set; }
        public bool isJoinable { get; set; }
        public bool isJoined { get; set; }
        public bool isOwned { get; set; }
    }

    public class SharedAlbumOptions
    {
        public bool isCollaborative { get; set; }
        public bool isCommentable { get; set; }
    }
}
