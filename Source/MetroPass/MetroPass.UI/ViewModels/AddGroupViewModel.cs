using System;
using System.Xml.Linq;
using Caliburn.Micro;
using Framework;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using Metropass.Core.PCL.Model;
using Metropass.Core.PCL.Model.Kdb4;

namespace MetroPass.UI.ViewModels
{
    public class AddGroupViewModel : BaseScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IKdbTree _dbTree;

        public AddGroupViewModel(IKdbTree dbTree,
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IPageServices pageServices)
            : base(navigationService, eventAggregator, pageServices)
        {
            _dbTree = dbTree;
            _navigationService = navigationService;
        }

        private string _parentGroupID;
        public string ParentGroupID
        {
            get { return _parentGroupID; }
            set
            {
                _parentGroupID = value;
                var groupElement = _dbTree.FindGroupByUuid(value);
                ParentGroup = new PwGroup(groupElement);
            }
        }

        private PwGroup _parentGroup;
        public PwGroup ParentGroup
        {
            get { return _parentGroup; }
            set
            {
                _parentGroup = value;
                NotifyOfPropertyChange(() => ParentGroup);
            }
        }

        private string _folderName;
        public string FolderName
        {
            get { return _folderName; }
            set
            {
                _folderName = value;
                CanSave = !String.IsNullOrWhiteSpace(value);
                NotifyOfPropertyChange(() => FolderName);
            }
        }

        private bool _canSave = false;
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
            var groupElement = GetNewGroupElement();
            var group = new PwGroup(groupElement);
            ParentGroup.AddGroupToDocument(group);
            await PWDatabaseDataSource.Instance.SavePwDatabase();
            _navigationService.GoBack();
        }
  
        private XElement GetNewGroupElement()
        {
            var groupTemplate = @"
                <Group>
                    <UUID>{0}</UUID>
                    <Name>{2}</Name>
                    <IconID>0</IconID>
                    <Times>
                        <LastModificationTime>{1}</LastModificationTime>
                        <CreationTime>{1}</CreationTime>
                        <LastAccessTime>{1}</LastAccessTime>
                        <ExpiryTime>{1}</ExpiryTime>
                        <LocationChanged>{1}</LocationChanged>
                        <Expires>False</Expires>
                        <UsageCount>0</UsageCount>
                    </Times>
                    <IsExpanded>True</IsExpanded>
                    <DefaultAutoTypeSequence />
                    <EnableAutoType>null</EnableAutoType>
                    <EnableSearching>null</EnableSearching>
                </Group>
            ";
            var uuid = new PwUuid(true);
            groupTemplate = String.Format(groupTemplate, Convert.ToBase64String(uuid.UuidBytes), DateTime.Now.ToFormattedUtcTime(), FolderName);

            var element = XElement.Parse(groupTemplate);
            return element;
        }
    }
}