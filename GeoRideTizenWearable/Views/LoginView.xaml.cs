using GeoRideTizenWearable.ViewModels;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms.Xaml;

namespace GeoRideTizenWearable.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : CirclePage
    {
        public LoginView()
        {
            InitializeComponent();

            BindingContext = new LoginViewModel();
        }
    }
}