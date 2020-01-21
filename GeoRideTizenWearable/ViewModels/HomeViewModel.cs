using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GeoRideTizenWearable.Helpers;
using GeoRideTizenWearable.Models;
using GeoRideTizenWearable.Views;
using Microsoft.VisualBasic.CompilerServices;
using Tizen;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoRideTizenWearable.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        public ICommand ToggleButtonCommand { get; }
        public ICommand RetryButtonCommand { get; }

        public ICommand SettingsCommand { get; }
        public ICommand LogoutCommand { get; }

        #region Properties notifyOnChanged
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
        /// Button toggle
        /// </summary>
        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        /// <summary>
        /// Stacklayout with image and button
        /// </summary>
        private bool _stackLayoutUpdatedIsVisible;
        public bool StackLayoutUpdatedIsVisible
        {
            get { return _stackLayoutUpdatedIsVisible; }
            set
            {
                _stackLayoutUpdatedIsVisible = value;
                OnPropertyChanged(nameof(StackLayoutUpdatedIsVisible));
            }
        }

        /// <summary>
        /// Stacklayout error
        /// </summary>
        private bool _stackLayoutErrorIsVisible;
        public bool StackLayoutErrorIsVisible
        {
            get { return _stackLayoutErrorIsVisible; }
            set
            {
                _stackLayoutErrorIsVisible = value;
                OnPropertyChanged(nameof(StackLayoutErrorIsVisible));
            }
        }

        /// <summary>
        /// Button toggle text
        /// </summary>
        private string _toggleButtonText;
        public string ToggleButtonText
        {
            get { return _toggleButtonText; }
            set
            {
                _toggleButtonText = value;
                OnPropertyChanged(nameof(ToggleButtonText));
            }
        }

        /// <summary>
        /// Image locker
        /// </summary>
        private string _lockerImage;
        public string LockerImage
        {
            get { return _lockerImage; }
            set
            {
                _lockerImage = value;
                OnPropertyChanged(nameof(LockerImage));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public HomeViewModel()
        {
            // Init all commands
            ToggleButtonCommand = new Command(async () => await ToggleAsync());
            RetryButtonCommand = new Command(() => RetryAsync());
            SettingsCommand = new Command(() => OpenTrackersListView());
            LogoutCommand = new Command(() => LogoutPopup());

            // Update UI with status of tracker
            GetStatusTracker();
        }

        private async void GetStatusTracker()
        {
            IsBusy = true;

            // Hide stacklayout
            StackLayoutUpdatedIsVisible = false;

            bool tokenIsValid = await UtilsHelper.CheckTokenValid();

            if (!tokenIsValid)
            {
                bool result = await APIHelper.RegenerateTokenAsync();

                if (!result)
                {
                    // Show error stack layout
                    StackLayoutErrorIsVisible = true;
                    return;
                }
                else
                    Toast.DisplayText(Constants.regenerateToken, 5000);
            }

            // Get tracker id from secure storage
            var trackerId = await SecureStorage.GetAsync("trackerId");

            // Call api to update list with trackers
            _ = APIHelper.GetTrackersAsync(trackers =>
            {
                if (trackers.Count == 0)
                {
                    // Show error stack layout
                    StackLayoutErrorIsVisible = true;
                }

                foreach (TrackerModel tracker in trackers)
                {
                    if (tracker.TrackerId == int.Parse(trackerId))
                    {
                        // The tracker is locked
                        if (tracker.IsLocked)
                        {
                            ToggleButtonText = Constants.unlockButtonText;
                            LockerImage = Constants.lockerLock;
                        }
                        else
                        {
                            ToggleButtonText = Constants.lockButtonText;
                            LockerImage = Constants.lockerUnlock;
                        }

                        // Show stacklayout
                        StackLayoutUpdatedIsVisible = true;
                    }
                }

                IsBusy = false;
            });
        }

        private async Task ToggleAsync()
        {
            // Disabled button
            IsEnabled = false;

            // Activity indicator
            IsBusy = true;

            await APIHelper.ToggleTrackerAsync(result =>
            {
                // If return true, the tracker is locked
                if (result)
                {
                    ToggleButtonText = Constants.unlockButtonText;
                    LockerImage = Constants.lockerLock;
                }
                else
                {
                    ToggleButtonText = Constants.lockButtonText;
                    LockerImage = Constants.lockerUnlock;
                }
            });

            IsEnabled = true;
            IsBusy = false;
        }

        private void RetryAsync()
        {
            // Disabled button retry
            IsEnabled = false;

            // Update UI with status of tracker
            GetStatusTracker();

            // Enabled button retry
            IsEnabled = true;
        }

        private void OpenTrackersListView()
        {
            Application.Current.MainPage = new TrackersListView();
        }

        private void LogoutPopup()
        {
            var popup = new TwoButtonPopup();

            var leftButton = new MenuItem()
            {
                IconImageSource = new FileImageSource { File = Constants.iconButtonCancel },
                Command = new Command(() => {
                    popup.Dismiss();
                })
            };

            var rightButton = new MenuItem()
            {
                IconImageSource = new FileImageSource { File = Constants.iconButtonDone },
                Command = new Command(() => {

                    // Remove all properties in secure storage
                    SecureStorage.RemoveAll();

                    Application.Current.MainPage = new LoginView();
                    popup.Dismiss();
                })
            };

            popup.FirstButton = leftButton;
            popup.SecondButton = rightButton;

            popup.Content = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                   new Label
                   {
                       Text = Constants.textPopupLogout,
                       VerticalTextAlignment = TextAlignment.Center,
                       HorizontalTextAlignment = TextAlignment.Center
                   }
                }
            };

            popup.BackButtonPressed += (s, e) =>
            {
                popup.Dismiss();
            };

            popup.Show();
        }
    }
}