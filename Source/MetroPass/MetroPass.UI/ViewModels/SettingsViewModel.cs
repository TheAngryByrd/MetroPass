using Caliburn.Micro;

namespace MetroPass.UI.ViewModels
{
    public class SettingsViewModel : Screen
    {
        public SettingsViewModel()
        {
            this.DisplayName = "Options";

            RecycleBinEnabled = true;
        }

        private bool _recycleBinEnabled;
        public bool RecycleBinEnabled
        {
            get { return _recycleBinEnabled; }
            set
            {
                _recycleBinEnabled = value;
                NotifyOfPropertyChange(() => RecycleBinEnabled);
            }
        }
    }
}