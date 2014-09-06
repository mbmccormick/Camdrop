using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;

namespace Camdrop.API
{
    public class ServiceClient
    {
        private string ServerAddress;
        private Session CurrentSession;

        public ServiceClient()
        {
            ServerAddress = "www.dropcam.com";
            CurrentSession = null;
        }

        public async Task LoginLogin(Action<LoginLoginResponse> callback, string username, string password)
        {
            HttpWebRequest request = HttpWebRequest.Create("https://" + ServerAddress + "/api/login.login?username=" + username + "&password=" + password) as HttpWebRequest;

            var response = await request.GetResponseAsync().ConfigureAwait(false);

            Stream stream = response.GetResponseStream();
            UTF8Encoding encoding = new UTF8Encoding();
            StreamReader sr = new StreamReader(stream, encoding);

            JsonTextReader tr = new JsonTextReader(sr);
            LoginLoginResponse data = new JsonSerializer().Deserialize<LoginLoginResponse>(tr);

            tr.Close();
            sr.Dispose();

            stream.Dispose();

            if (data.status == 0)
                CurrentSession = data.items[0];

            callback(data);
        }

        public async Task UsersGetCurrent(Action<UsersGetCurrentResponse> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create("https://" + ServerAddress + "/api/users.get_current") as HttpWebRequest;
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
            HttpWebRequest request = HttpWebRequest.Create("https://" + ServerAddress + "/api/cameras.get_visible") as HttpWebRequest;
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
            HttpWebRequest request = HttpWebRequest.Create("https://" + ServerAddress + "/api/cameras.get_demo") as HttpWebRequest;
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
            HttpWebRequest request = HttpWebRequest.Create("https://" + ServerAddress + "/api/cameras.get_image?uuid=" + uuid + "&width=" + width + "&ticks=" + DateTime.Now.Ticks) as HttpWebRequest;
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
