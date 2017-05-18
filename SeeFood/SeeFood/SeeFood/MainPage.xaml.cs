using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SeeFood
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

        private void Button_Clicked(object sender, EventArgs e)
        {
            TakePhoto();
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static byte[] StreamToByteArray(Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }
            else
            {
                // Jon Skeet's accepted answer 
                return ReadFully(stream);
            }
        }

        private async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                //string url = "http://vizmenu.azurewebsites.net/api/getlistsmultipart";
                string url = "http://vizmenu.azurewebsites.net/api/getlists";

                var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "MenuAnalyzer",
                    Name = $"{DateTime.UtcNow}.jpg"
                };

                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

                /*HttpClient client = new HttpClient();

                
                MultipartFormDataContent content = new MultipartFormDataContent();
                ByteArrayContent baContent = new ByteArrayContent(StreamToByteArray(file.GetStream()));

                content.Add(baContent, "File", "filename.ext");
                

                //upload MultipartFormDataContent content async and store response in response var
                var response =
                  await client.PostAsync(url, content);

                //read response result as a string async into json var
                var responsestr = response.Content.ReadAsStringAsync().Result;
                //Socket s = new Socket(SocketType.Stream);


                //byte[] filedata = GetByteArray(file.GetStream());*/
                var request = (HttpWebRequest)WebRequest.Create("http://vizmenu.azurewebsites.net/api/getlists");

                request.Method = "POST";
                request.ContentType = "multipart/form-data";

                var data = StreamToByteArray(file.GetStream());

                //request..ContentLength = data.Length;

                using (var stream = await request.GetRequestStreamAsync())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = await request.GetResponseAsync();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
        }
    }
}
