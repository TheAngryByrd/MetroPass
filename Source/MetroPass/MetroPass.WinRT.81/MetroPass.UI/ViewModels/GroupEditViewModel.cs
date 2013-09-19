using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using System.ComponentModel.DataAnnotations;
using Metropass.Core.PCL.Model;

namespace MetroPass.UI.ViewModels
{
    public class GroupEditViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;

        private readonly IPWDatabaseDataSource _dataSource;

        public GroupEditViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPageServices pageServices,
            IPWDatabaseDataSource dataSource)
            : base(navigationService, eventAggregator, pageServices)
        {
            _dataSource = dataSource;
            _navigationService = navigationService;
        }

        private string _groupId;
        public string GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                Group = _dataSource.PwDatabase.Tree.FindGroupByUuid(value);               
            }
        }

        PwGroup _pwGroup;
        public PwGroup Group
        {
        	get { return _pwGroup; }
            private set
            {
                _pwGroup = value;
                NotifyOfPropertyChange(() => Group);
            }
        }

        [Required(ErrorMessage="Group name is required")]
        public string GroupName
        {
            get { return Group.Name; }
            set
            {
            	Group.Name = value;
                NotifyOfPropertyChange(() => GroupName);
            }
        
        }

        private bool _canSave = true;
        public bool CanSave
        {
        	get { return _canSave; }
            set
            {
                _canSave = value;
                NotifyOfPropertyChange(() => CanSave);
            }
        }

        private bool isProgressEnabled;

        public bool IsProgressEnabled
        {
            get { return isProgressEnabled; }
            set { isProgressEnabled = value;

            NotifyOfPropertyChange(() => IsProgressEnabled);
            }
        }

        private bool canGoBack = true;

        public override bool CanGoBack
        {
            get { return base.CanGoBack && canGoBack; }
            set { canGoBack = value;
            NotifyOfPropertyChange(() => CanGoBack);
            }
        }


        public async void Save()
        {        	
            CanSave = false;
            canGoBack = false;
            IsProgressEnabled = true;
            await _dataSource.SavePwDatabase();
            _navigationService.GoBack();
        }
    }
}