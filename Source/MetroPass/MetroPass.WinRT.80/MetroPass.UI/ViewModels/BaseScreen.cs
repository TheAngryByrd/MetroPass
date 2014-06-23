using System;
using System.Collections.Generic;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using MetroPass.UI.Services;
using Metropass.Core.PCL.Model;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ReactiveCaliburn;
using System.Threading.Tasks;

namespace MetroPass.UI.ViewModels
{
    public class BaseScreen : ReactiveScreen
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private Queue<string> _stateQueue;
        private IPageServices _pageServices;

        public BaseScreen(INavigationService navigationService, IEventAggregator eventAggregator, IPageServices pageServices)
        { 
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            _pageServices = pageServices;
            _stateQueue = new Queue<string>();
        }

        protected Page View { get; private set;}

        public async void LaunchUrl(Uri uri)
        {
            try
            {
                var result = await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            catch
            {
                _pageServices.Toast("This entry's url is invalid");
            }
        }

        protected Uri GetPasswordUri(PwEntry password)
        {
            if (password != null)
            {
                Uri parsedUri;
                Uri.TryCreate(password.Url, UriKind.RelativeOrAbsolute, out parsedUri);
                if (parsedUri != null && parsedUri.IsWellFormedOriginalString())
                {
                    try
                    {
                        string uriScheme = parsedUri.Scheme;
                    }
                    catch (InvalidOperationException)
                    {
                        // I know ... WTF right?!?!? The Scheme property doesn't return null or empty if it wasn't part of the original URL string. It throws an exception.
                        parsedUri = new Uri("http://" + parsedUri.OriginalString);
                    }
                    return parsedUri;
                }
            }
            return new Uri("");
        }

        public virtual void GoBack()
        {
            _navigationService.GoBack();
        }

        public virtual bool CanGoBack
        {
            get { return _navigationService.CanGoBack; }
            set { }
        }

        protected internal override async Task OnViewAttached(object view, object context)
        {
            await base.OnViewAttached(view, context);
            View = (Page)view;

            Window.Current.SizeChanged += Window_SizeChanged;
            IsAdVisible = true;

            if (_stateQueue.Count == 0)
            {
                _stateQueue.Enqueue(ApplicationView.Value.ToString());
            }
            SetState(_stateQueue.Dequeue());
        }

        protected override async Task OnDeactivate(bool close)
        {
            await base.OnDeactivate(close);
            Window.Current.SizeChanged -= Window_SizeChanged;
            _eventAggregator.Unsubscribe(this);
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