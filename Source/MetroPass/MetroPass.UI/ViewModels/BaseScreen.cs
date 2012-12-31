using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.ViewModels
{
    public class BaseScreen : Screen
    {
        private readonly INavigationService _navigationService;
        private Queue<string> _stateQueue;
        private IPageServices _pageServices;

        public BaseScreen(INavigationService navigationService, IPageServices pageServices)
        {  
            this._navigationService=navigationService;
            this._pageServices = pageServices;
            _stateQueue = new Queue<string>();

        }

        protected Page View { get; private set;}

        public async void LaunchUrl(string url)
        {
            try
            {
                var result = await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            }
            catch
            {
                _pageServices.Toast("This entry's url is invalid");
            }
          
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public bool CanGoBack
        {
            get { return _navigationService.CanGoBack; }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            View = (Page)view;

            Window.Current.SizeChanged += Window_SizeChanged;
            IsAdVisible = true;

            if (_stateQueue.Count == 0)
            {
                _stateQueue.Enqueue(ApplicationView.Value.ToString());
            }
            SetState(_stateQueue.Dequeue());
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetState(ApplicationView.Value);
        }

        protected void SetState(ApplicationViewState state)
        {
            SetState(state.ToString());
        }

        protected void SetState(string state)
        {
            VisualStateManager.GoToState(View, state, true);
        }

        protected void QueueState(string state)
        {
            _stateQueue.Enqueue(state);
        }

        private bool _isPageAvailable;

        public bool IsAdVisible
        {
            get { return SettingsModel.Instance.IsAdsVisible && _isPageAvailable; }
            set
            {
                _isPageAvailable = value;
                NotifyOfPropertyChange(() => IsAdVisible);
            }
        }
    }
    
}
