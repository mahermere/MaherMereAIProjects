using System;
using System.Configuration;
using System.Web;
using Hyland.Unity;

namespace Triple_S_AEP_API.Services
{
    public static class HylandUnityConnectionService
    {
        // URL and DataSource from web.config
        private static readonly string AppServerUrl = System.Configuration.ConfigurationManager.AppSettings["HylandUnity:AppServerUrl"];
        private static readonly string DataSource = System.Configuration.ConfigurationManager.AppSettings["HylandUnity:DataSource"];

        public static Application Connect(HttpRequest request)
        {
            // Get username and password from headers
            string username = request.Headers["Hyland-Username"];
            string password = request.Headers["Hyland-Password"];
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                throw new Exception("Missing Hyland-Username or Hyland-Password in request headers.");

            Application app = null;
            var authProps = Application.CreateOnBaseAuthenticationProperties(AppServerUrl, username, password, DataSource);
            authProps.LicenseType = LicenseType.Default;
            authProps.DisconnectedMode = true;
            
            try
            {
                app = Application.Connect(authProps);
            }
            catch (MaxLicensesException)
            {
                throw new Exception("All available licenses have been consumed.");
            }
            catch (SystemLockedOutException)
            {
                throw new Exception("The system is currently in lockout mode.");
            }
            catch (InvalidLoginException)
            {
                throw new Exception("Invalid Login Credentials.");
            }
            catch (UserAccountLockedException)
            {
                throw new Exception("This account has been temporarily locked.");
            }
            catch (AuthenticationFailedException)
            {
                throw new Exception("NT Authentication Failed.");
            }
            catch (MaxConcurrentLicensesException)
            {
                throw new Exception("All concurrent licenses for this user group have been consumed.");
            }
            catch (InvalidLicensingException)
            {
                throw new Exception("Invalid Licensing.");
            }

            if (app == null)
            {
                throw new Exception("Failed to connect to Hyland Unity API.");
            }

            return app;
        }

        public static bool VerifyUser(HttpRequest request)
        {
            try
            {
                var app = Connect(request);
                return app != null;
            }
            catch
            {
                return false;
            }
        }

        public static Application GetVerifiedApplication(HttpRequest request)
        {
            try
            {
                var app = Connect(request);
                if (app != null)
                    return app;
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
