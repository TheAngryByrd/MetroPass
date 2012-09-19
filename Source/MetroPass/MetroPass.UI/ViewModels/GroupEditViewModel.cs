using Caliburn.Micro;
using MetroPass.Core.Model;
using MetroPass.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroPass.UI.ViewModels
{
    public class GroupEditViewModel : BaseScreen
    {
        private readonly IPageServices pageServices;

        public GroupEditViewModel(IPageServices pageServices, INavigationService navigationService) : base(navigationService)
        {
            this.pageServices = pageServices;
        }
        PwGroup pwGroup;

        public PwGroup Group
        {
        	get
        	{
                return pwGroup;
        	}
            set
            {
                pwGroup = value;
                NotifyOfPropertyChange(() => Group);
            }
        }

        private string myVar;

        public string GroupName
        {
            get { return Group.Name; }
            set
            {
            	Group.Name = value;
                NotifyOfPropertyChange(() => GroupName);
            }
        }

    }
}
