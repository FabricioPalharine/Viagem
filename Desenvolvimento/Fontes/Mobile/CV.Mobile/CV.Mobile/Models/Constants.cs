using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Models
{
    public static class Constants
    {
        public static string ClientId = "210037759249.apps.googleusercontent.com";
        public static string ClientSecret = "H1CNNlmDu-uNnGll5ylQmvgp";
        public static string ClientAPI = "AIzaSyAlUpOpwZWS_ZGlMAtB6lY76oy1QBWk97g";

        public static string Scopes = "profile email https://picasaweb.google.com/data/ https://www.googleapis.com/auth/youtube.upload https://www.googleapis.com/auth/youtube";
        public static string AppName = "CV";

        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://accounts.google.com/o/oauth2/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
        public static string RedirectToURL = "http://sites.architettura.com.br:5131/Login.html";

    }

    public enum ImageOrientation
    {
        /// <summary>
        /// The image to left
        /// </summary>
        ImageToLeft = 0,
        /// <summary>
        /// The image on top
        /// </summary>
        ImageOnTop = 1,
        /// <summary>
        /// The image to right
        /// </summary>
        ImageToRight = 2,
        /// <summary>
        /// The image on bottom
        /// </summary>
        ImageOnBottom = 3,
        /// <summary>
        /// The image centered
        /// </summary>
        ImageCentered = 4
    }
}
