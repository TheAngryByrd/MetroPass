using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace MetroPass.WP8.UI.ViewModels
{
    public class SkydriveAccessViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get { return "test1"; } }
        public IScreen HostScreen { get; private set; }

        public SkydriveAccessViewModel(IScreen screen)
        {
            HostScreen = screen;
        }
    }
}
