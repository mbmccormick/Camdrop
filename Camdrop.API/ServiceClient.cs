using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Camdrop.API
{
    public class ServiceClient
    {
        private string PrimaryServerAddress;
        private string SecondaryServerAddress;
        private Session CurrentSession;

        public ServiceClient()
        {
            PrimaryServerAddress = "www.dropcam.com";
            SecondaryServerAddress = "nexusapi.dropcam.com";
            CurrentSession = null;
        }

        public async Task LoginLogin(Action<LoginLoginResponse> callback, string username, string password)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            filter.AllowAutoRedirect = false;

            HttpClient client = new HttpClient(filter);

            client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.AcceptEncoding.Add(new HttpContentCodingWithQualityHeaderValue("gzip, deflate"));
            client.DefaultRequestHeaders.Referer = new Uri("https://www.dropcam.com/");
            client.DefaultRequestHeaders.UserAgent.Add(new HttpProductInfoHeaderValue("camdrop/1.0"));
            
            HttpStringContent content = new HttpStringContent(String.Format("password={0}&username={1}", WebUtility.UrlEncode(password), WebUtility.UrlEncode(username)), Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
                        
            LoginLoginResponse data = new LoginLoginResponse();

            try
            {
                var response = await client.PostAsync(new Uri("https://" + PrimaryServerAddress + "/login/submit"), content);

                if (response.Headers.Location.AbsolutePath != "/watch")
                {
                    data.status = 1;
                    data.status_description = "Login Failed";
                    data.status_detail = "The credentials you provided were not valid. Please check your Email Address and Password and try again.";
                }
                else
                {
                    string[] cookies = response.Headers["Set-Cookie"].Split(';');

                    CurrentSession = new Session();
                    CurrentSession.session_token = cookies[0].Split('=')[1].Trim();

                    data.status = 0;
                    data.status_description = "Login Succeeded";
                    data.status_detail = "Your credentials were valid and your session is now authorized.";
                }

                callback(data);
            }
            catch (Exception ex)
            {
                data.status = 2;
                data.status_description = "Service Unavailable";
                data.status_detail = "The Dropcam service is not available at this time. Please try again later.";

                callback(data);
            }
        }

        public async Task UsersGetCurrent(Action<UsersGetCurrentResponse> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("https://" + PrimaryServerAddress + "/api/users.get_current") as HttpWebRequest;
            request.Headers["Cookie"] = "website_2=" + CurrentSession.session_token;

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            UsersGetCurrentResponse data = new JsonSerializer().Deserialize<UsersGetCurrentResponse>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            callback(data);
        }

        public async Task CamerasGetVisible(Action<CamerasGetVisibleResponse> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("https://" + PrimaryServerAddress + "/api/cameras.get_visible") as HttpWebRequest;
            request.Headers["Cookie"] = "website_2=" + CurrentSession.session_token;

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            CamerasGetVisibleResponse data = new JsonSerializer().Deserialize<CamerasGetVisibleResponse>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            callback(data);
        }

        public async Task CamerasGetDemo(Action<CamerasGetVisibleResponse> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("https://" + PrimaryServerAddress + "/api/cameras.get_demo") as HttpWebRequest;
            request.Headers["Cookie"] = "website_2=" + CurrentSession.session_token;

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            CamerasGetVisibleResponse data = new JsonSerializer().Deserialize<CamerasGetVisibleResponse>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            callback(data);
        }

        public async Task CamerasGetImage(Action<byte[]> callback, string uuid)
        {
            await CamerasGetImage(callback, uuid, 1024);
        }

        public async Task CamerasGetImage(Action<byte[]> callback, string uuid, int width)
        {
            HttpWebRequest request = HttpWebRequest.Create("https://" + SecondaryServerAddress + "/get_image?uuid=" + uuid + "&width=" + width + "&ticks=" + DateTime.Now.Ticks) as HttpWebRequest;
            request.Headers["Cookie"] = "website_2=" + CurrentSession.session_token;

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            BinaryReader br = new BinaryReader(stream, encoding);

            int length = Convert.ToInt32(response.ContentLength);
            byte[] data = br.ReadBytes(length);

            br.Dispose();

            stream.Dispose();

            callback(data);
        }
    }
}
