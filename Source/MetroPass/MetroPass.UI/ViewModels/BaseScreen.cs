using System;
using System.Linq;
using Caliburn.Micro;
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
            var page = (Page)view;

            Window.Current.SizeChanged += (o, e) =>
            {
                SetState(page, ApplicationView.Value);
            };

            SetState(page, ApplicationView.Value);
        }

        private void SetState(Control view, ApplicationViewState state)
        {
            VisualStateManager.GoToState(view, state.ToString(), true);
        }
    }
}