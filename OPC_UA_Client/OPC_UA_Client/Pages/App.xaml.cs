using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace OPC_UA_Client
{
	public partial class App : Application
	{
        public static object Navigation { get; internal set; }

        public App ()
		{
			InitializeComponent();
            MainPage mp = new MainPage();
            mp.Title = "Client OPC";
            MainPage = new NavigationPage(mp);
            
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
