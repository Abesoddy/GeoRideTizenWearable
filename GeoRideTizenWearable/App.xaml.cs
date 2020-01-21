using GeoRideTizenWearable.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GeoRideTizenWearable
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Check if remember me is checked on login page
            CheckRememberMe();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public void CheckRememberMe()
        {
            var rememberMe = Preferences.Get("rememberMe", false);

            if (rememberMe)
                MainPage = new HomeView();
            else
                MainPage = new LoginView();
        }
    }
}