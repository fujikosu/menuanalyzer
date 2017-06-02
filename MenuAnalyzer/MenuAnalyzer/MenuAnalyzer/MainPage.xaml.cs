using Plugin.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace MenuAnalyzer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Take_Photo_Clicked(object sender, EventArgs e)
        {
            TakePhoto();
        }

        private async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
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

                        using (var message = await client.PostAsync(SettingsReader.GetKey("url"), content))
                        {
                            try
                            {
                                var input = await message.Content.ReadAsStringAsync();
                                MyList.ItemsSource = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuItems>(input).menus;
                                MyList.IsVisible = true;

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

        private void Clear_Clicked(object sender, EventArgs e)
        {
            MyList.IsVisible = false;
        }
    }
}
