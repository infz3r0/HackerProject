using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HackerProject.Utilities
{
    public static class Client
    {
        public const string Domain = "http://www.hacker-project.com/";
        public static CookieContainer CookieContainer = new CookieContainer();
        public static HttpClientHandler HttpClientHandler = new HttpClientHandler() { CookieContainer = CookieContainer };
        public static HttpClient HttpClient = new HttpClient(HttpClientHandler);
        public static Uri GetUri(string uri)
        {
            return new Uri(Domain + uri);
        }
    }
}
