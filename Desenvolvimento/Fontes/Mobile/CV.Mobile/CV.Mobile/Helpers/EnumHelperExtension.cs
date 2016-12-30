using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ExifLib;

namespace CV.Mobile.Helpers
{
    public static class EnumHelperExtension
    {
        public static string Descricao(this Enum enumValue)
        {
            ResourceManager _resources = new ResourceManager("CV.Mobile.Resource.EnumDescription", typeof(enumMoeda).GetTypeInfo().Assembly);


            string rk = String.Format("{0}_{1}", enumValue.GetType().Name, enumValue);
            string localizedDescription = _resources.GetString(rk);

            if (localizedDescription == null)
            {
                return enumValue.ToString();

            }
            else
                return localizedDescription;



        }
    }

    public static class MediaFileExtension
    {
        public static double DecimalLatitude(this ExifLib.JpegInfo self)
        {
            double sd = 0.0;
            double min = 0.0;
            double sec = 0.0;
            double deg = self.GpsLatitude[0];

            min = self.GpsLatitude[1] / ((double)60);
            sec = self.GpsLatitude[2] / ((double)3600);
            sd = deg + min + sec;
            if (self.GpsLatitudeRef == ExifGpsLatitudeRef.South)
                sd = sd * -1;
            sd = Math.Round(sd, 6);

            return sd;
        }

        public static double DecimalLongitude(this ExifLib.JpegInfo self)
        {
            double sd = 0.0;
            double min = 0.0;
            double sec = 0.0;
            double deg = self.GpsLongitude[0];

            min = self.GpsLongitude[1] / ((double)60);
            sec = self.GpsLongitude[2] / ((double)3600);
            sd = deg + min + sec;
            if (self.GpsLongitudeRef == ExifGpsLongitudeRef.West)
                sd = sd * -1;
            sd = Math.Round(sd, 6);

            return sd;
        }
    }
}
