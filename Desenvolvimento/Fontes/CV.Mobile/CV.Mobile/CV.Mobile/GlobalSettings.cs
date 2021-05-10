using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CV.Mobile.Helper;
using CV.Mobile.Models;

namespace CV.Mobile
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://192.168.15.2:8090/"; 

        public static string AppName = "CurtindoUmaViagem";

        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string iOSClientId = "997990659234-nf8a2ob032qi7v5nb9je3bhcg58g190q.apps.googleusercontent.com";
        public static string AndroidClientId = "997990659234-i02c3j8naujeq9880no2rnu1o4r5hoav.apps.googleusercontent.com";

        // These values do not need changing 
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email profile https://www.googleapis.com/auth/photoslibrary https://www.googleapis.com/auth/photoslibrary.sharing";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "com.googleusercontent.apps.997990659234-nf8a2ob032qi7v5nb9je3bhcg58g190q:/oauth2redirect";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.997990659234-i02c3j8naujeq9880no2rnu1o4r5hoav:/oauth2redirect";

        private static string chaveApi = "R4/aaqG3eonSTiga726loekiz5z+rQaQ+CIhLg3cu2q7Yo4JcqL+TQ==";
        public static string ClientAPI
        {
            get { return Criptografia.Descriptografa(chaveApi); }            
        }


        private string _baseEndpoint;

        public GlobalSetting()
        {
            AuthToken = "INSERT AUTHENTICATION TOKEN";

            BaseEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance { get; } = new GlobalSetting();

      
     

        public string BaseEndpoint
        {
            get { return _baseEndpoint; }
            set
            {
                _baseEndpoint = value;
            }
        }
                

        public string AuthToken { get; set; }
        public UsuarioLogado UsuarioLogado { get; set; }
        public Viagem ViagemSelecionado { get; internal set; }

        public ObservableCollection<Usuario> AmigosViagem { get; set; }
    }
}