using System.Threading.Tasks;
using Tizen.Security;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace GeoRideTizenWearable
{
    class Program : Xamarin.Forms.Platform.Tizen.FormsApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            GetPermission();
            LoadApplication(new App());
        }

        static void Main(string[] args)
        {
            var app = new Program();
            Forms.Init(app);
            Tizen.Wearable.CircularUI.Forms.Renderer.FormsCircularUI.Init();
            app.Run(args);
        }

        async void GetPermission()
        {
            Tizen.Log.Info(Constants.logTag, "Internet Privilege : " + PrivacyPrivilegeManager.CheckPermission(Constants.internetPrivilege).ToString());

            bool internetGranted;

            if (PrivacyPrivilegeManager.CheckPermission(Constants.internetPrivilege) != CheckResult.Allow)
                internetGranted = await RequestPermission(Constants.internetPrivilege);
            else
                internetGranted = true;

            if (!internetGranted)
            {
                Tizen.Log.Error(Constants.logTag, "Failed to obtain permission '" + Constants.internetPrivilege + "'.");
                Toast.DisplayText(Constants.permissionNotGranted, 5000);

                return;
            }
        }

        static Task<bool> RequestPermission(string privilege)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            var response = PrivacyPrivilegeManager.GetResponseContext(privilege);
            PrivacyPrivilegeManager.ResponseContext target;
            response.TryGetTarget(out target);
            target.ResponseFetched += (s, e) =>
            {
                if (e.cause == CallCause.Error)
                {
                    /// Handle errors
                    Tizen.Log.Error(Constants.logTag, "An error occurred while requesting an user permission");
                    tcs.SetResult(false);
                    return;
                }

                // Now, we can check if the permission is granted or not
                switch (e.result)
                {
                    case RequestResult.AllowForever:
                        // Permission is granted.
                        // We can do this permission-related task we want to do.
                        Tizen.Log.Info(Constants.logTag, "Response : RequestResult.AllowForever");
                        tcs.SetResult(true);
                        break;
                    case RequestResult.DenyForever:
                    case RequestResult.DenyOnce:
                        // Functionality that depends on this permission will not be available
                        Tizen.Log.Info(Constants.logTag, "Response: RequestResult." + e.result.ToString());
                        tcs.SetResult(false);
                        break;
                }
            };

            PrivacyPrivilegeManager.RequestPermission(privilege);

            return tcs.Task;
        }
    }
}