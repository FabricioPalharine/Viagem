using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CV.Mobile.Services.RequestProvider
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(string uri, string token = "");

        Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "");
        Task<TReturn> PostAsync<TResult,TReturn>(string uri, TResult data, string token = "", int? Timeout=null);
        Task PostNullAsync<TResult>(string uri, TResult data, string token = "");


        Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "");

        Task<TResult> DeleteAsync<TResult>(string uri, string token = "");

        string GetUniqueToken();
        void SetUniqueToken(string Token);
    }
}
