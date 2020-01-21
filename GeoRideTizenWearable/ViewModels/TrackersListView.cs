using System.Collections.ObjectModel;
using System.ComponentModel;
using GeoRideTizenWearable.Helpers;
using GeoRideTizenWearable.Models;

namespace GeoRideTizenWearable.ViewModels
{
    public class TrackersListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TrackerModel> Trackers { get; } = new ObservableCollection<TrackerModel>();

        /// <summary>
        /// Activity indicator visible / enabled
        /// </summary>
        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        /// <summary>
        /// Trackers list visible / invisible
        /// </summary>
        private bool _trackersListIsVisible;
        public bool TrackersListIsVisible
        {
            get { return _trackersListIsVisible; }
            set
            {
                _trackersListIsVisible = value;
                OnPropertyChanged(nameof(TrackersListIsVisible));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public TrackersListViewModel()
        {
            IsBusy = true;

            // Call api to update list with trackers
            _ = APIHelper.GetTrackersAsync(trackersResult =>
            {
                foreach (TrackerModel tracker in trackersResult)
                    Trackers.Add(tracker);

                IsBusy = false;
                TrackersListIsVisible = true;
            });
        }
    }
}