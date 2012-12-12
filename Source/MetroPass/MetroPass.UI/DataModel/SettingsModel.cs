using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MetroPass.UI.DataModel
{
    public static class SettingsModel
    {

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

        public static int MinutesToLockDatabase
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

        public static int SecondsToClearClipboard
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
    }
}
