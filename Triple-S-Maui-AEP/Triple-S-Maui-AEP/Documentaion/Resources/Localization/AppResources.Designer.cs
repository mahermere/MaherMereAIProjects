// Auto-generated resource accessor for AppResources
using System;
using System.Globalization;
using System.Resources;

namespace TripleSPOC.Resources.Localization
{
    public static class AppResources
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        public static ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    resourceMan = new ResourceManager("TripleSPOC.Resources.Localization.AppResources", typeof(AppResources).Assembly);
                }
                return resourceMan;
            }
        }
        public static CultureInfo Culture {
            get { return resourceCulture; }
            set { resourceCulture = value; }
        }
        public static string Language => ResourceManager.GetString("Language", resourceCulture);
        public static string English => ResourceManager.GetString("English", resourceCulture);
        public static string Spanish => ResourceManager.GetString("Spanish", resourceCulture);
        public static string SignIn => ResourceManager.GetString("SignIn", resourceCulture);
        public static string NPNNumber => ResourceManager.GetString("NPNNumber", resourceCulture);
        public static string Password => ResourceManager.GetString("Password", resourceCulture);
        public static string RememberMyNPN => ResourceManager.GetString("RememberMyNPN", resourceCulture);
        public static string ForgotPassword => ResourceManager.GetString("ForgotPassword", resourceCulture);
        public static string FirstTimeUser => ResourceManager.GetString("FirstTimeUser", resourceCulture);
        public static string ContactOfficeManager => ResourceManager.GetString("ContactOfficeManager", resourceCulture);
        public static string AgentPortalAccess => ResourceManager.GetString("AgentPortalAccess", resourceCulture);
        public static string EnterCredentials => ResourceManager.GetString("EnterCredentials", resourceCulture);
        public static string Success => ResourceManager.GetString("Success", resourceCulture);
        public static string LoginSuccessful => ResourceManager.GetString("LoginSuccessful", resourceCulture);
        public static string Cancel => ResourceManager.GetString("Cancel", resourceCulture);
        public static string Back => ResourceManager.GetString("Back", resourceCulture);
        public static string Next => ResourceManager.GetString("Next", resourceCulture);
        public static string QuickActions => ResourceManager.GetString("QuickActions", resourceCulture);
        public static string DashboardHeaderTitle => ResourceManager.GetString("DashboardHeaderTitle", resourceCulture);
        public static string DashboardHeaderSubtitle => ResourceManager.GetString("DashboardHeaderSubtitle", resourceCulture);
        public static string StartNewSOA => ResourceManager.GetString("StartNewSOA", resourceCulture);
        public static string StartNewEnrollment => ResourceManager.GetString("StartNewEnrollment", resourceCulture);
        public static string Exit => ResourceManager.GetString("Exit", resourceCulture);
        public static string SOACountToday => ResourceManager.GetString("SOACountToday", resourceCulture);
        public static string EnrollmentCountToday => ResourceManager.GetString("EnrollmentCountToday", resourceCulture);
        public static string TodaysActivity => ResourceManager.GetString("TodaysActivity", resourceCulture);
        public static string SOAWizard_Title => ResourceManager.GetString("SOAWizard_Title", resourceCulture);
        public static string SOAWizard_Step1 => ResourceManager.GetString("SOAWizard_Step1", resourceCulture);
        public static string SOAWizard_FirstName => ResourceManager.GetString("SOAWizard_FirstName", resourceCulture);
        public static string SOAWizard_LastName => ResourceManager.GetString("SOAWizard_LastName", resourceCulture);
        public static string SOAWizard_DOB => ResourceManager.GetString("SOAWizard_DOB", resourceCulture);
        public static string SOAWizard_Medicare => ResourceManager.GetString("SOAWizard_Medicare", resourceCulture);
        public static string SOAWizard_Phone => ResourceManager.GetString("SOAWizard_Phone", resourceCulture);
        public static string SOAWizard_Step2 => ResourceManager.GetString("SOAWizard_Step2", resourceCulture);
        public static string SOAWizard_ApptDate => ResourceManager.GetString("SOAWizard_ApptDate", resourceCulture);
        public static string SOAWizard_ApptTime => ResourceManager.GetString("SOAWizard_ApptTime", resourceCulture);
        public static string SOAWizard_AgentName => ResourceManager.GetString("SOAWizard_AgentName", resourceCulture);
        public static string SOAWizard_ContactMethod => ResourceManager.GetString("SOAWizard_ContactMethod", resourceCulture);
        public static string SOAWizard_ContactMethod_Phone => ResourceManager.GetString("SOAWizard_ContactMethod_Phone", resourceCulture);
        public static string SOAWizard_ContactMethod_InPerson => ResourceManager.GetString("SOAWizard_ContactMethod_InPerson", resourceCulture);
        public static string SOAWizard_ContactMethod_Video => ResourceManager.GetString("SOAWizard_ContactMethod_Video", resourceCulture);
        public static string SOAWizard_Step3 => ResourceManager.GetString("SOAWizard_Step3", resourceCulture);
        public static string SOAWizard_SelectPlans => ResourceManager.GetString("SOAWizard_SelectPlans", resourceCulture);
        public static string SOAWizard_Advantage => ResourceManager.GetString("SOAWizard_Advantage", resourceCulture);
        public static string SOAWizard_Drug => ResourceManager.GetString("SOAWizard_Drug", resourceCulture);
        public static string SOAWizard_Supplement => ResourceManager.GetString("SOAWizard_Supplement", resourceCulture);
        public static string SOAWizard_Dental => ResourceManager.GetString("SOAWizard_Dental", resourceCulture);
        public static string SOAWizard_Signature => ResourceManager.GetString("SOAWizard_Signature", resourceCulture);
        public static string SOAWizard_ClearSignature => ResourceManager.GetString("SOAWizard_ClearSignature", resourceCulture);
        public static string SOAWizard_Step4 => ResourceManager.GetString("SOAWizard_Step4", resourceCulture);
        public static string SOAWizard_Attest => ResourceManager.GetString("SOAWizard_Attest", resourceCulture);
        public static string SOAWizard_Back => ResourceManager.GetString("SOAWizard_Back", resourceCulture);
        public static string SOAWizard_Next => ResourceManager.GetString("SOAWizard_Next", resourceCulture);

        public static string Enrollment_HeaderTitle => ResourceManager.GetString("Enrollment_HeaderTitle", resourceCulture);
        public static string Enrollment_PersonalInfo => ResourceManager.GetString("Enrollment_PersonalInfo", resourceCulture);
        public static string Enrollment_FirstName => ResourceManager.GetString("Enrollment_FirstName", resourceCulture);
        public static string Enrollment_LastName => ResourceManager.GetString("Enrollment_LastName", resourceCulture);
        public static string Enrollment_DOB => ResourceManager.GetString("Enrollment_DOB", resourceCulture);
        public static string Enrollment_Gender => ResourceManager.GetString("Enrollment_Gender", resourceCulture);
        public static string Enrollment_PrimaryPhone => ResourceManager.GetString("Enrollment_PrimaryPhone", resourceCulture);
        public static string Enrollment_Email => ResourceManager.GetString("Enrollment_Email", resourceCulture);
        public static string Enrollment_Medicare => ResourceManager.GetString("Enrollment_Medicare", resourceCulture);
        public static string Enrollment_Validation_FirstNameRequired => ResourceManager.GetString("Enrollment_Validation_FirstNameRequired", resourceCulture);
        public static string Enrollment_Validation_LastNameRequired => ResourceManager.GetString("Enrollment_Validation_LastNameRequired", resourceCulture);
        public static string Enrollment_Validation_DOBInvalid => ResourceManager.GetString("Enrollment_Validation_DOBInvalid", resourceCulture);
        public static string Enrollment_Validation_DOBTooYoung => ResourceManager.GetString("Enrollment_Validation_DOBTooYoung", resourceCulture);
        public static string Enrollment_Validation_GenderRequired => ResourceManager.GetString("Enrollment_Validation_GenderRequired", resourceCulture);
        public static string Enrollment_Validation_PhoneRequired => ResourceManager.GetString("Enrollment_Validation_PhoneRequired", resourceCulture);
        public static string Enrollment_Validation_MedicareRequired => ResourceManager.GetString("Enrollment_Validation_MedicareRequired", resourceCulture);
    }
}
