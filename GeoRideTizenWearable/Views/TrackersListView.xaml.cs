using GeoRideTizenWearable.Helpers;
using GeoRideTizenWearable.Models;
using GeoRideTizenWearable.ViewModels;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace GeoRideTizenWearable.Views
{
    public partial class TrackersListView : CirclePage
    {
        public TrackersListView()
        {
            InitializeComponent();

            BindingContext = new TrackersListViewModel();
        }

        async void TrackerSelected(object sender, SelectedItemChangedEventArgs e)
        {
            TrackerModel tracker = e.SelectedItem as TrackerModel;

            if (tracker == null)
                return;

            // Save trackerId for all application
            await UtilsHelper.SecureSaveValue(tracker.TrackerId.ToString(), "trackerId");

            // Instanciate home view
            Application.Current.MainPage = new HomeView();
        }
    }
}