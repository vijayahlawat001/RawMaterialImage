using System;
using TestApp1.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp1
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            var isLoogged = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
            if (isLoogged == "1")
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new Login();
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
