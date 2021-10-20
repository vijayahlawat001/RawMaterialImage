using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestApp1.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp1.Views
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CameraCapture : ContentPage
    {
        public List<string> pickerSource { get; set; }
        private MediaFile _mediaFile;
        string BaseAPIPath = "http://143.161.234.25/AndritzStoreImageApplication/api/";
        public CameraCapture()
        {
            try
            {
                InitializeComponent();
                GetFolderNamesAsync();
                btnStartCamera.Clicked += async (sender, args) =>
                {
                    try
                    {
                        image.Source = null;
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
                            Name = DateTime.Now.ToString("ddMMyyyyhhmmss") + ".jpeg"
                        });

                        if (_mediaFile == null)
                            return;
                        string FilePath = Encrypt(pickerProject.SelectedItem.ToString() + "/" + pickerAssemblyName.SelectedItem.ToString() + "/" + pickerUnitName.SelectedItem).Replace(@"+", @"---").Replace(@"/", @"___");
                        var content = new MultipartFormDataContent();
                        content.Add(new StreamContent(_mediaFile.GetStream()), "\"file\"", $"\"{_mediaFile.Path}\"");
                        var httpCient = new HttpClient();
                        //var uploadServerBaseAddress = BaseAPIPath+ "Upload?Location=" + FilePath;
                        var uploadServerBaseAddress = BaseAPIPath+ "Upload?Location=" + FilePath;
                        var httpResponseMessage = await httpCient.PostAsync(uploadServerBaseAddress, content);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            await DisplayAlert("File Uploaded", "File Uploaded Successfully", "OK");
                        }

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
            }
            catch (Exception ex)
            {
                DisplayAlert("Error in application", ex.Message, "OK");
            }
        }
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "jhvfkjhgshdjkghs";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            int I = clearText.Length;
            return clearText;

        }
        async public Task<string> GetFolderNamesAsync()
        {
            try
            {
                //var uploadServerBaseAddress = "https://143.161.234.25:444/AndritzStoreImageApplication/api/Upload";
                var uploadServerBaseAddress = BaseAPIPath+ "Upload";
                var client = new HttpClient();
                var response = await client.GetStringAsync(uploadServerBaseAddress);
                int TimeOut = 0;
                while (response == null || TimeOut > 20)
                {
                    System.Threading.Thread.Sleep(1000);
                    TimeOut += 1;
                }
                List<FolderName> responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FolderName>>(response.Replace(@"\", "").Substring(1, response.Replace(@"\", "").Length - 2));
                foreach (var item in responseInfo)
                {
                    pickerProject.Items.Add(item.Name.ToString());
                }
                return responseInfo.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void pickerProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetAssemblyName(pickerProject.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                DisplayAlert("Error in application", ex.Message, "OK");
            }
        }
        async public Task<string> GetAssemblyName(string ProjectName)
        {
            try
            {
                pickerAssemblyName.Items.Clear();
                var content = new StringContent(ProjectName);
                var httpCient = new HttpClient();
                var uploadServerBaseAddress = BaseAPIPath+ "AssemblyName?ProjectName=" + ProjectName;
                var response = await httpCient.GetStringAsync(uploadServerBaseAddress);
                int TimeOut = 0;
                while (response == null || TimeOut > 20)
                {
                    System.Threading.Thread.Sleep(1000);
                    TimeOut += 1;
                }
                List<FolderName> responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FolderName>>(response.Replace(@"\", "").Substring(1, response.Replace(@"\", "").Length - 2));
                foreach (var item in responseInfo)
                {
                    pickerAssemblyName.Items.Add(item.Name.ToString());
                }
                return responseInfo.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async public Task<string> GetUnitName(string ProjectName, string AssemblyName)
        {
            try
            {
                pickerUnitName.Items.Clear();
                var content = new StringContent(ProjectName);
                var httpCient = new HttpClient();
                var uploadServerBaseAddress = BaseAPIPath+ "UnitName?ProjectName=" + ProjectName + "&AssemblyName=" + AssemblyName;
                var response = await httpCient.GetStringAsync(uploadServerBaseAddress);
                int TimeOut = 0;
                while (response == null || TimeOut > 20)
                {
                    System.Threading.Thread.Sleep(1000);
                    TimeOut += 1;
                }
                List<FolderName> responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FolderName>>(response.Replace(@"\", "").Substring(1, response.Replace(@"\", "").Length - 2));
                foreach (var item in responseInfo)
                {
                    pickerUnitName.Items.Add(item.Name.ToString());
                }
                return responseInfo.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void pickerAssemblyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetUnitName(pickerProject.SelectedItem.ToString(), pickerAssemblyName.SelectedItem.ToString());
            }
            catch (Exception ex)
            {
                DisplayAlert("Error in application", ex.Message, "OK");
            }
        }
    }
    class FolderName
    {
        public string Name { get; set; }
    }
}