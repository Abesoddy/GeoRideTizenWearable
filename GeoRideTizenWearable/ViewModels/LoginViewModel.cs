using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GeoRideTizenWearable.Helpers;
using GeoRideTizenWearable.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoRideTizenWearable.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public LoginModel LoginModel { get; set; } = new LoginModel();
        public ICommand SignInCommand { get; }
        public ICommand RememberMeLabelCommand { get; }

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
        /// Check bool checked / unchecked
        /// </summary>
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        /// <summary>
        /// Button login bool enabled / disabled
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;
            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public LoginViewModel()
        {
            SignInCommand = new Command(async () => await SignInAsync());
            RememberMeLabelCommand = new Command(() => ToggleCheckRememberMe());
        }

        private async Task SignInAsync()
        {
            // Validate fields
            bool result = UtilsHelper.ValidateForm(LoginModel.Email, LoginModel.Password);

            if (!result)
                return;

            // Disabled button
            IsEnabled = false;

            // Enabled activity indicator
            IsBusy = true;

            if (IsChecked)
                await SaveCredentials();

            await APIHelper.LoginAsync(LoginModel.Email, LoginModel.Password);

            // Disabled activity indicator
            IsBusy = false;

            // Enabled button
            IsEnabled = true;
        }

        private async Task SaveCredentials()
        {
            // Save email and password in secure storage
            await UtilsHelper.SecureSaveValue(LoginModel.Email, "email");
            await UtilsHelper.SecureSaveValue(LoginModel.Password, "password");

            // Set rememberMe bool in preferences to bypass loginView in others launchs
            Preferences.Set("rememberMe", true);
        }

        private void ToggleCheckRememberMe()
        {
            // Invert
            IsChecked = !IsChecked;
        }
    }
}