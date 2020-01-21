using GeoRideTizenWearable.ViewModels;
using Tizen.Wearable.CircularUI.Forms;

namespace GeoRideTizenWearable.Views
{
    public partial class HomeView : CirclePage
    {
        public HomeView()
        {
            InitializeComponent();

            BindingContext = new HomeViewModel();
        }
    }
}