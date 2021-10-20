using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestApp1.Model;

namespace TestApp1.ViewModel
{
    class FolderNameViewModel
    {
        public IList<FolderName> FolderNameListProperty { get; set; }
        public FolderNameViewModel()
        {
            GetFolderNamesAsync();
        }
        async public Task<string> GetFolderNamesAsync()
        {
            try
            {
                var uploadServerBaseAddress = "http://143.161.235.70/UploadServerAPI/api/Upload";
                var client = new HttpClient();
                var response = await client.GetStringAsync(uploadServerBaseAddress);
                int TimeOut = 0;
                while (response == null || TimeOut > 20)
                {
                    System.Threading.Thread.Sleep(1000);
                    TimeOut += 1;
                }
                var responseInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(response); 
                FolderNameListProperty = new ObservableCollection<FolderName>();
                var subdirs = responseInfo.ToString().Split(',') ;
                foreach (var item in subdirs)
                {
                    FolderNameListProperty.Add(new FolderName { FolderNameProperty = item.ToString() });
                }
                
                return responseInfo.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
