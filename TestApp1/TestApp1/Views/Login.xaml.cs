using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestApp1.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Login : ContentPage
	{
		public Login ()
		{
			InitializeComponent ();
			loginbtn.Clicked += async (sender, args) =>
			{
                if (string.IsNullOrEmpty(Email.Text) || string.IsNullOrEmpty(Password.Text))
                {
                    await DisplayAlert("Empty Values", "Please enter Email and Password", "OK");
                }
                else
                {
                    if (Email.Text == "vijay.ahlawat@andritz.com" && Password.Text == "1234")
                    {
                        await Xamarin.Essentials.SecureStorage.SetAsync("isLogged","1");
                        Application.Current.MainPage = new AppShell();
                    }
                    else
                        await DisplayAlert("Login Fail", "Please enter correct Email and Password", "OK");
                }
            };
		}
	}
}