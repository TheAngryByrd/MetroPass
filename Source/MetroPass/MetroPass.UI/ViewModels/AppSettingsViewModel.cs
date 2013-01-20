using Caliburn.Micro;
using MetroPass.UI.DataModel;

namespace MetroPass.UI.ViewModels
{
    public class AppSettingsViewModel : Screen
    {
        public AppSettingsViewModel()
        {
            this.DisplayName = "MetroPass Options";
        }

        public const int MinMinutesToLockDatabase = 1;
        public bool LockDatabaseAfterInactivityEnabled
        {
            get
            {

                return SettingsModel.Instance.LockDatabaseAfterInactivityEnabled;
            }
            set
            {
                SettingsModel.Instance.MinutesToLockDatabase = value ? MinMinutesToLockDatabase : 0;

                NotifyOfPropertyChange(() => LockDatabaseAfterInactivityEnabled);
                NotifyOfPropertyChange(() => MinutesToLockDatabase);
            }
        }

        public int MinutesToLockDatabase
        {
            get
            {
                return SettingsModel.Instance.MinutesToLockDatabase;
            }
            set
            {
                SettingsModel.Instance.MinutesToLockDatabase = value;
                NotifyOfPropertyChange(() => MinutesToLockDatabase);
            }
        }

        public const int MinClearClipboardSeconds = 10;

        public bool SecondsToClearClipboardEnabled
        {
            get
            {
                return SettingsModel.Instance.ClearClipboardEnabled;
            }
            set
            {
                SettingsModel.Instance.SecondsToClearClipboard = value ? MinClearClipboardSeconds : 0;


                NotifyOfPropertyChange(() => SecondsToClearClipboardEnabled);
                NotifyOfPropertyChange(() => SecondsToClearClipboard);
            }
        }

        public int SecondsToClearClipboard
        {
            get
            {
                return SettingsModel.Instance.SecondsToClearClipboard;
            }
            set
            {
                SettingsModel.Instance.SecondsToClearClipboard = value;
                NotifyOfPropertyChange(() => SecondsToClearClipboard);
            }
        }
    }
}