using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HackerProject.Utilities
{
    public static class SendRequest
    {
        public static async Task<string> POST(string uri, Dictionary<string, string> param)
        {
            var values = param;
            var content = new FormUrlEncodedContent(values);
            var response = await Client.HttpClient.PostAsync(Client.Domain + uri, content);
            string responseString = string.Empty;
            try
            {
                //will throw an exception if not successful
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return responseString;
        }
        public static async Task<string> GET(string uri)
        {
            var response = await Client.HttpClient.GetAsync(Client.Domain + uri);
            string responseString = string.Empty;

            try
            {
                //will throw an exception if not successful
                response.EnsureSuccessStatusCode();
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return responseString;
        }
    }
}
