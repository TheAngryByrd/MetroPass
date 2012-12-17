using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MetroPass.UI.DataModel
{
    public class SettingsModel
    {

        private static SettingsModel instance = new SettingsModel();
        private SettingsModel()
        {
            IsAdsVisible = true;
        }
        public static SettingsModel Instance { get { return instance; } }

        private static ApplicationDataContainer settings = ApplicationData.Current.RoamingSettings;
    
        private static T Get<T>([CallerMemberName] String propertyName = null)
        {
            T retVal = default(T);
            if( settings.Values.ContainsKey(propertyName))
            {
                object val = settings.Values[propertyName];
                if (val is T)
                {
                    retVal = (T)val;
                }
            }
            return retVal;
        }
        private static void Set<T>(T value, [CallerMemberName] String propertyName = null)
        {
            settings.Values[propertyName] = value;        
        }

        public bool LockDatabaseAfterInactivityEnabled
        {
            get
            {
                return MinutesToLockDatabase > 0;
            }
        }

        public int MinutesToLockDatabase
        {
            get
            {
                return Get<int>();
            }
            set
            {
                Set<int>(value);
            }
        }

        public bool ClearClipboardEnabled
        {
            get
            {
                return SecondsToClearClipboard > 0;
            }

        }

        public int SecondsToClearClipboard
        {

            get
            {
                return Get<int>();
            }
            set
            {
                Set<int>(value);
            }
        }

        private bool _isAdsVisible;

        public bool IsAdsVisible
        {
            get { return _isAdsVisible; }
            set { _isAdsVisible = value; }
        }

        public string FileExtensions
        {
            get
            {
               var val = Get<string>();
                if(string.IsNullOrWhiteSpace(val))
                {
                    FileExtensions = ".doc .docx";
                }
                return Get<string>();
            }
            set
            {
                Set<string>(value);
            }
        }
        

        
    }
}
