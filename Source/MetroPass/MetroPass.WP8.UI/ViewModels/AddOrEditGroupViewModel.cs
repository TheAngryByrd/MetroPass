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
        private PwGroup _pwGroup;

        private readonly INavigationService _navigationService;

        public AddOrEditGroupViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            this.ObservableForPropertyNotNull(vm => vm.GroupUuid).Subscribe(GetGroup);
            SaveCommand = new ReactiveCommand(this.WhenAny(vm => vm.GroupName, x => !string.IsNullOrEmpty(x.Value)));
            SaveCommand.Subscribe(Save);
        }

       

        public ReactiveCommand SaveCommand { get; set; }

        private string _groupUuid;

        public PwGroup PwGroup
        {
            get { return this._pwGroup; }
            set { this._pwGroup = value; }
        }

        public string GroupUuid
        {
            get { return _groupUuid; }
            set { this.RaiseAndSetIfChanged(ref _groupUuid, value); }
        }

        private void GetGroup(IObservedChange<AddOrEditGroupViewModel, string> obj)
        {
            PwGroup = PWDatabaseDataSource.Instance.PwDatabase.Tree.FindGroupByUuid(GroupUuid);
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

        private void Save(object obj)
        {
            
        }
    }
}
