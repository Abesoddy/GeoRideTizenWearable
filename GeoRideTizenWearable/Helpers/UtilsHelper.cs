using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Tizen;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Essentials;

namespace GeoRideTizenWearable.Helpers
{
    public class UtilsHelper
    {
        public static async Task SecureSaveValue(string value, string valueName)
        {
            try
            {
                // Remove begin because method SetAsync not overwriting value
                SecureStorage.Remove(valueName);
                await SecureStorage.SetAsync(valueName, value);
            }
            catch (Exception ex)
            {
                Toast.DisplayText(ex.Message, 3000);
                Log.Error(Constants.logTag, ex.Message);
            }
        }

        public static bool ValidateForm(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                Toast.DisplayText(Constants.emailEmpty, 3000);
                return false;
            }
            else
            {
                try
                {
                    MailAddress mailCheck = new MailAddress(email);
                }
                catch (FormatException)
                {
                    Toast.DisplayText(Constants.emailInvalid, 3000);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(password))
            {
                Toast.DisplayText(Constants.passwordEmpty, 3000);
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Check if token in secure storage is valid (creation timestamp < 30 days)
        /// </summary>
        /// <returns>true if token valid / false if not valid</returns>
        public static async Task<bool> CheckTokenValid()
        {
            // Get token creation date from secure storage
            var creationTimestamp = await SecureStorage.GetAsync("timestamp");

            Log.Debug(Constants.logTag, "Creation timestamp : " + creationTimestamp);

            DateTime timestampDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            timestampDateTime = timestampDateTime.AddSeconds(double.Parse(creationTimestamp));

            // Calcule time of token, regenerate if 30 days
            DateTime nowDate = DateTime.Now;
            TimeSpan difference = nowDate - timestampDateTime;

            Log.Debug(Constants.logTag, "Difference days : " + difference.Days);

            if (difference.Days >= 30)
            {
                Log.Info(Constants.logTag, "Token expire");
                return false;
            }
            else
            {
                Log.Info(Constants.logTag, "Token valid");
                return true;
            }
        }
    }
}