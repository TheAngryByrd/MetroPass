using Caliburn.Micro;
using Windows.ApplicationModel;

namespace MetroPass.UI.ViewModels
{
    public class AboutSettingsViewModel : Screen
    {
        public AboutSettingsViewModel()
        {
            DisplayName = "About MetroPass";
            Version = string.Format("{0}.{1}.{2}.{3}", Package.Current.Id.Version.Major, Package.Current.Id.Version.Minor, Package.Current.Id.Version.Build, Package.Current.Id.Version.Revision);
        }

        public string Version { get; set; }
    }
}