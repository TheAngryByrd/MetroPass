using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;

namespace MetroPass.UI.ViewModels
{
    public class EntryEditViewModel : BaseScreen
    {
        public EntryEditViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        PwEntry pwEntry;

        public PwEntry Entry
        {
        	get
        	{
                return pwEntry;
        	}
            set
            {
                pwEntry = value;
                NotifyOfPropertyChange(() => Entry);
            }
        }

        public string Title
        {
        	get
        	{
                return Entry.Title;
        	}

            set
            {
                this.Entry.Title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }
        public string Username
        {
            get
            {
                return Entry.Username;
            }

            set
            {
                this.Entry.Username = value;
                NotifyOfPropertyChange(() => Username);
            }
        }

        public string Password
        {
            get
            {
                return Entry.Password;
            }

            set
            {
                this.Entry.Password = value;
                NotifyOfPropertyChange(() => Password);
            }
        }

        public string Url
        {
        	get
        	{
                return Entry.Url;
        	}
            set
            {
                this.Entry.Url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        public string Notes
        {
        	get
        	{
                return Entry.Notes;
        	}
            set
            {
                Entry.Notes = value;
                NotifyOfPropertyChange(() => Notes);
            }
        
        }

        private bool canSave = true;
        public bool CanSave
        {
            get
            {
                return canSave;
            }
            set
            {
                canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

 

        public async void Save()
        {
            CanSave = false;
    
                await PWDatabaseDataSource.Instance.SavePwDatabase();

                CanSave = true;
    
        }
    }
}
