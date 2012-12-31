using Caliburn.Micro;
using MetroPass.Core.Interfaces;
using MetroPass.Core.Model;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MetroPass.UI.ViewModels
{
    public class GroupEditViewModel : BaseScreen
    {
        private readonly IKdbTree _dbTree;
        private readonly INavigationService _navigationService;

        public GroupEditViewModel(IKdbTree dbTree, INavigationService navigationService, IPageServices pageServices)
            : base(navigationService,pageServices)
        {
            _dbTree = dbTree;
            _navigationService = navigationService;
        }

        private string _groupId;
        public string GroupId
        {
            get { return _groupId; }
            set
            {
                _groupId = value;
                var groupElement = _dbTree.FindGroupByUuid(value);
                Group = new PwGroup(groupElement);
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

        public async void Save()
        {        	
            CanSave = false;
            await PWDatabaseDataSource.Instance.SavePwDatabase();
            _navigationService.GoBack();
        }
    }
}