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
     

        public GroupEditViewModel( INavigationService navigationService) : base(navigationService)
        {
        
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
