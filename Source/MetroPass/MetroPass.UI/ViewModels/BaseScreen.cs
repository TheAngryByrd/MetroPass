﻿using System;
using System.Linq;
using Caliburn.Micro;
using MetroPass.UI.DataModel;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroPass.UI.ViewModels
{
    public class BaseScreen : Screen
    {
        private readonly INavigationService _navigationService;

        public BaseScreen(INavigationService navigationService)
        {
  
            this._navigationService=navigationService;
        }

        protected Page View { get; private set;}

        

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
            SetState(ApplicationView.Value);
            
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