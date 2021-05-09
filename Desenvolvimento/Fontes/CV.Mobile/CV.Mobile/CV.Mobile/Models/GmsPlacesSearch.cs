using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public class GmsPlacesSearch
    {
        public List<GmsSearchResults> results { get; set; }
        public string status { get; set; }

        
    }
    public class GmsSearchResults
    {
        public GmsGeometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public string reference { get; set; }
        public string[] types { get; set; }
        public string vicinity { get; set; }

        public string name_full
        {
            get
            {
                return string.Concat(name, " - ", vicinity);
            }
        }
        public class GmsGeometry
        {
            public GmsPosition location { get; set; }
            public class GmsPosition
            {
                public double lat { get; set; }
                public double lng { get; set; }
            }
        }
    }
}
