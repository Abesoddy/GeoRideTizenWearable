using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GeoRideTizenWearable.Models;
using GeoRideTizenWearable.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tizen;
using Tizen.Network.Bluetooth;
using Tizen.Network.Connection;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoRideTizenWearable.Helpers
{
    public class APIHelper
    {
        private static HttpClient InitHttpClient()
        {
            ConnectionItem currentConnection = ConnectionManager.CurrentConnection;

            Log.Debug(Constants.logTag, "Current connection : " + currentConnection.Type.ToString());

            if (currentConnection.Type == ConnectionType.Disconnected)
            {
                Toast.DisplayText(Constants.noNetwork, 5000);
                Log.Info(Constants.logTag, "No network");
                return null;
            }
            else if (currentConnection.Type == ConnectionType.Ethernet)
            {
                Log.Info(Constants.logTag, "Connection type ethernet");

                // For Tizen Emulator, it is not necessary to set up web proxy.
                // It's for Samsung Galaxy Watch which is paired with the mobile phone.
                if (DeviceInfo.DeviceType != DeviceType.Virtual)
                {
                    Log.Info(Constants.logTag, "Device not virtual, create webproxy");

                    // Use web proxy
                    string proxyAddr = ConnectionManager.GetProxy(AddressFamily.IPv4);

                    HttpClientHandler handler = new HttpClientHandler()
                    {
                        Proxy = new WebProxy(proxyAddr, true),
                        UseProxy = true
                    };

                    return new HttpClient(handler);
                }
            }

            return new HttpClient();
        }

        /// <summary>
        /// Get all trackers by token in secureStorage
        /// </summary>
        public static async Task GetTrackersAsync(Action<List<TrackerModel>> action)
        {
            HttpClient httpClient = InitHttpClient();

            // Get token auth from secure storage
            var token = await SecureStorage.GetAsync("token");

            Log.Debug(Constants.logTag, "Token : " + token);

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await httpClient.GetAsync(Constants.baseUrlApiGeoride + Constants.trackersEndpointApiGeoride);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    var trackers = JsonConvert.DeserializeObject<List<TrackerModel>>(await response.Content.ReadAsStringAsync());
                    action(trackers);
                }
                catch (JsonSerializationException ex)
                {
                    Toast.DisplayText(ex.Message, 5000);
                    Log.Error(Constants.logTag, ex.Message);
                    action(new List<TrackerModel>());
                }
            }
            else
                action(new List<TrackerModel>());
        }

        /// <summary>
        /// Toogle tracker lock / unlock by tracker id
        /// </summary>
        public static async Task ToggleTrackerAsync(Action<bool> action)
        {
            // Get tracker id from secure storage
            var trackerId = await SecureStorage.GetAsync("trackerId");

            Log.Debug(Constants.logTag, "Tracker id : " + trackerId);

            var urlLogin = Constants.baseUrlApiGeoride + "tracker/" + trackerId + "/toggleLock";

            try
            {
                HttpClient httpClient = InitHttpClient();

                // Get token auth from secure storage
                var token = await SecureStorage.GetAsync("token");

                Log.Debug(Constants.logTag, "Token : " + token);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.PostAsync(urlLogin, null);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var boolResult = JObject.Parse(json)["locked"];
                    action((bool)boolResult);
                }
            }
            catch (HttpRequestException ex)
            {
                Toast.DisplayText(ex.Message, 5000);
                Log.Error(Constants.logTag, ex.Message);
            }
        }

        /// <summary>
        /// Post login endpoint api georide
        /// </summary>
        public static async Task LoginAsync(string email, string password)
        {
            // Create body content
            var bodyContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", email),
                new KeyValuePair<string, string>("password", password)
            });

            var urlLogin = Constants.baseUrlApiGeoride + Constants.loginEndpointApiGeoride;

            try
            {
                HttpClient httpClient = InitHttpClient();

                var response = await httpClient.PostAsync(urlLogin, bodyContent);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var result = JsonConvert.DeserializeObject<SuccessLoginModel>(json);

                    // Save token securely
                    await UtilsHelper.SecureSaveValue(result.AuthToken, "token");

                    // Save timestamp for expiration date of token
                    var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                    await UtilsHelper.SecureSaveValue(timestamp, "timestamp");

                    // Instanciate trackers view for selecting one traker
                    Application.Current.MainPage = new TrackersListView();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    Toast.DisplayText(Constants.invalidLogin, 5000);
                else
                    Toast.DisplayText(Constants.invalidRequest, 5000);
            }
            catch (HttpRequestException ex)
            {
                Toast.DisplayText("Erreur : " + ex.Message, 5000);
                Log.Error(Constants.logTag, ex.Message);
            }
        }

        /// <summary>
        /// Regenerate token
        /// </summary>
        public static async Task<bool> RegenerateTokenAsync()
        {
            var url = Constants.baseUrlApiGeoride + Constants.regenerateTokenEndpointApiGeoride;

            Log.Debug(Constants.logTag, "Url : " + url);

            try
            {
                HttpClient httpClient = InitHttpClient();

                // Get token auth from secure storage
                var token = await SecureStorage.GetAsync("token");

                Log.Debug(Constants.logTag, "Token : " + token);

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var tokenResult = JObject.Parse(json)["authToken"];

                    Log.Debug(Constants.logTag, "Token result : " + token);

                    // Update token in secure storage
                    await UtilsHelper.SecureSaveValue(tokenResult.ToString(), "token");

                    // Save timestamp for expiration date of token
                    var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                    await UtilsHelper.SecureSaveValue(timestamp, "timestamp");

                    return true;
                }
                else
                {
                    Toast.DisplayText(Constants.errorRegenerateToken, 5000);
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Toast.DisplayText(ex.Message, 5000);
                Log.Error(Constants.logTag, ex.Message);
                return false;
            }
        }
    }
}