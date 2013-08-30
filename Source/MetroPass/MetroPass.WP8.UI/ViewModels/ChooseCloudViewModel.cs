using Caliburn.Micro;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace MetroPass.WP8.UI.ViewModels
{
    public class ChooseCloudViewModel : ReactiveScreen
    {
        private readonly INavigationService _navigationService;

        public ReactiveCommand NavigateToSkyDriveCommand { get; set; }
        public ReactiveCommand NavigateToDropboxCommand { get; set; }

        public ChooseCloudViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToSkyDriveCommand = new ReactiveCommand();
            NavigateToSkyDriveCommand.Subscribe(NavigateToSkyDrive);
            
            NavigateToDropboxCommand = new ReactiveCommand(null,false);
            NavigateToDropboxCommand.Subscribe(NavigateToDropbox);
        }

        private void NavigateToDropbox(object obj)
        {
 	       
        }

        private void NavigateToSkyDrive(object obj)
        {
            _navigationService.UriFor<SkydriveAccessViewModel>().Navigate();
        }
    }
}
