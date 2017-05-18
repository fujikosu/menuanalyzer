using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                //string url = "http://vizmenu.azurewebsites.net/api/getlists";

                var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "MenuAnalyzer",
                    Name = $"{DateTime.UtcNow}.jpg",
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium

                };

                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

                myActivityIndicator.IsRunning = true;
                myActivityIndicator.IsVisible = true;
                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StreamContent(file.GetStream()), "file", "1.jpg");
                        //Not sure below line is true or not   //http://seefoodapp.azurewebsites.net/api/getlistsmultipart
                        using (var message = await client.PostAsync("http://vizmenu.azurewebsites.net/api/getlistsmultipart", content))
                        {
                            try
                            {
                                var input = await message.Content.ReadAsStringAsync();
                                MyList.ItemsSource = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuItems>(input).menus;
                                myActivityIndicator.IsRunning = false;
                                myActivityIndicator.IsVisible = false;
                            }
                            catch
                            {
                                myActivityIndicator.IsRunning = false;
                                myActivityIndicator.IsVisible = false;
                                await DisplayAlert("Error", "No text was detected try taking a new picture", "Ok");
                            }
                        }
                    }
                }
            }
        }
    }
}
