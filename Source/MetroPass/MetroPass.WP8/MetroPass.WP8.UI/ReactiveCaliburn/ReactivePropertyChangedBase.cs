using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Caliburn.Micro;
using ReactiveUI;

namespace ReactiveCaliburn
{
    public class ReactivePropertyChangedBase : ReactiveObject, INotifyPropertyChangedEx
    {
#if !WinRT
        [Browsable(false)]
#endif
        public bool IsNotifying
        {
            get { return areChangeNotificationsEnabled; }
            set { throw new NotSupportedException(); }
        }


        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <typeparam name = "TProperty">The type of the property.</typeparam>
        /// <param name = "property">The property expression.</param>
        public virtual void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            NotifyOfPropertyChange(property.GetMemberInfo().Name);
        }


        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
#if WinRT || NET45
        public virtual void NotifyOfPropertyChange([CallerMemberName]string propertyName = "")
#else
        public virtual void NotifyOfPropertyChange(string propertyName)
#endif
        {
            raisePropertyChanged(propertyName);
        }


        public void Refresh()
        {
            NotifyOfPropertyChange(string.Empty);
        }
    }

}
