using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using MetroPass.WP8.UI.Utils;

using Metropass.Core.PCL.Model;
using ReactiveUI;
using Caliburn.Micro;

namespace MetroPass.WP8.UI.ViewModels
{
    public class AddOrEditGroupViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;

        

        public AddOrEditGroupViewModel(INavigationService navigationService)
        {            
            _navigationService = navigationService;
            this.ObservableForPropertyNotNull(vm => vm.GroupUuid).Subscribe(GetGroup);
            this.ObservableForPropertyNotNull(vm => vm.ParentGroupUuid).Subscribe(GetParentGroup);
            this.ObservableForPropertyNotNull(vm => vm.GroupName).Subscribe(SetupCanSave);            
        }

        private void SetupCanSave(IObservedChange<AddOrEditGroupViewModel, string> obj)
        {
            CanSave = !string.IsNullOrWhiteSpace(obj.Value);
        }

        public ReactiveCommand SaveCommand { get; set; }

        private string _groupUuid;
        public string GroupUuid
        {
            get { return _groupUuid; }
            set { this.RaiseAndSetIfChanged(ref _groupUuid, value); }
        }

        private PwGroup _pwGroup;
        public PwGroup PwGroup
        {
            get { return this._pwGroup; }
            set { this._pwGroup = value; }
        }

        private string _parentGroupUuid;
        public string ParentGroupUuid
        {
            get { return _parentGroupUuid; }
            set { this.RaiseAndSetIfChanged(ref _parentGroupUuid, value); }
        }

        private PwGroup _parentGroup;
        public PwGroup ParentGroup
        {
            get { return this._parentGroup; }
            set { this._parentGroup = value; }
        }

        private void GetGroup(IObservedChange<AddOrEditGroupViewModel, string> obj)
        {
            PwGroup = PWDatabaseDataSource.Instance.PwDatabase.Tree.FindGroupByUuid(GroupUuid);
        }

        private void GetParentGroup(IObservedChange<AddOrEditGroupViewModel, string> obj)
        {
            ParentGroup = PWDatabaseDataSource.Instance.PwDatabase.Tree.FindGroupByUuid(ParentGroupUuid);
        }

        private string _groupName;
        public string GroupName
        {
            get { return _groupName; }
            set { this.RaiseAndSetIfChanged(ref _groupName, value); }
        }

        protected override void OnActivate()
        {
            if(PwGroup != null)
            {
                GroupName = PwGroup.Name;
            }
        }

        private bool _canSave;
        public bool CanSave
        {
            get { return _canSave; }
            set { this.RaiseAndSetIfChanged(ref _canSave, value); }
        }

        public async void Save()
        {
            if(PwGroup == null)
            {
                PwGroup = PwGroup.GetNewGroupElement();                
                ParentGroup.AddGroupToDocument(PwGroup);
            }

            PwGroup.Name = GroupName;

            await PWDatabaseDataSource.Instance.SavePwDatabase();
            _navigationService.GoBack();
        }
    }
}
