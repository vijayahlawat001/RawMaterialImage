using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp1.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Camera : ContentPage
    {
        private MediaFile _mediaFile; 
        string BaseAPIPath = "http://143.161.234.25/AndritzStoreImageApplication/api/";
        public Camera()
        {
            InitializeComponent();
            takePhoto.Clicked += async (sender, args) =>
            {
                try
                {
                    await CrossMedia.Current.Initialize();

                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                        return;
                    }

                    if (_mediaFile != null)
                    {
                        _mediaFile.Dispose();
                    }
                    _mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                        Directory = "Sample",
                        Name = "test.jpg"
                    });

                    if (_mediaFile == null)
                        return;

                    await DisplayAlert("File Location", _mediaFile.Path, "OK");

                    image.Source = ImageSource.FromStream(() =>
                    {
                        var stream = _mediaFile.GetStream();
                        return stream;
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error in application", ex.Message, "OK");
                }
            };

            pickPhoto.Clicked += async (sender, args) =>
            {
                try
                {
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                        return;
                    }
                    if (_mediaFile != null)
                    {
                        _mediaFile.Dispose();
                    }
                    _mediaFile = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                    {
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                    });


                    if (_mediaFile == null)
                        return;

                    image.Source = ImageSource.FromStream(() =>
                    {
                        var stream = _mediaFile.GetStream();
                        return stream;
                    });
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error in application", ex.Message, "OK");
                }
            };

            takeVideo.Clicked += async (sender, args) =>
            {
                try
                {
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
                    {
                        await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                        return;
                    }

                    var file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
                    {
                        Name = "video.mp4",
                        Directory = "DefaultVideos",
                    });

                    if (file == null)
                        return;

                    await DisplayAlert("Video Recorded", "Location: " + file.Path, "OK");

                    file.Dispose();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error in application", ex.Message, "OK");
                }
            };

            pickVideo.Clicked += async (sender, args) =>
            {
                try
                {

                    if (!CrossMedia.Current.IsPickVideoSupported)
                    {
                        await DisplayAlert("Videos Not Supported", ":( Permission not granted to videos.", "OK");
                        return;
                    }
                    var file = await CrossMedia.Current.PickVideoAsync();

                    if (file == null)
                        return;

                    await DisplayAlert("Video Selected", "Location: " + file.Path, "OK");
                    file.Dispose();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error in application", ex.Message, "OK");
                }
            };
            UploadServer.Clicked += async (sender, args) =>
            {
                try
                {
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");
                    var httpCient = new HttpClient();
                    var uploadServerBaseAddress = BaseAPIPath + "Upload";
                    var httpResponseMessage = await httpCient.PostAsync(uploadServerBaseAddress, content);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        await DisplayAlert("File Uploaded", "Location: ", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error in application", ex.Message, "OK");
                }
            };
        }
    }
}