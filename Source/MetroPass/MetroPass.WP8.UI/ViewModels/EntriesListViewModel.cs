using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media;
using MetroPass.WP8.UI.DataModel;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;
using Metropass.Core.PCL.Model;
using ReactiveUI;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;

namespace MetroPass.WP8.UI.ViewModels
{
   

    public class EntriesListViewModel : ReactiveScreen
    {      

        private ObservableCollection<PwCommon> _items;

        public EntriesListViewModel()
        {
            this.ObservableForProperty(vm => vm.GroupId).Subscribe(GetGroup);
            this.ObservableForProperty(vm => vm.Group).Subscribe(GetSubgroupsAndEntries);
        }

        public ObservableCollection<PwCommon> Items
        {
            get {
                return _items;
            }
            set {
                this.RaiseAndSetIfChanged(ref _items, value);
            }
        }

        private async void GetSubgroupsAndEntries(IObservedChange<EntriesListViewModel, PwGroup> obj)
        {
                              
        }

        protected async override void OnViewReady(object view)
        {
            await Task.Delay(10);
            await RunAsync(() => Group.SubGroupsAndEntries, val => { Items = val; }); 
        }

        private static Task RunAsync<T>(Func<T> asyncFunc, Action<T> dispatcher)
        {
            return Task.Run(async () =>
            {
                var val = asyncFunc();
                Deployment.Current.Dispatcher.BeginInvoke(() => dispatcher(val));
            });     
        }

        private void GetGroup(IObservedChange<EntriesListViewModel, string> obj)
        {            
            Group = PWDatabaseDataSource.Instance.PwDatabase.Tree.FindGroupByUuid(obj.Value);                     
        }    

        const string Key = "\uE192";
        const string Folder = "\uE1C1";

        private string _groupId;
        public string GroupId {
            get {
                return _groupId;
            }
            set {
                this.RaiseAndSetIfChanged(ref _groupId,value);
            }
        }

        private PwGroup _group;
        public PwGroup Group {
            get {
                return _group;
            }
            set {
                this.RaiseAndSetIfChanged(ref _group, value);
            }
        }

        ///// <summary>
        ///// Initializes the items.
        ///// </summary>
        //private void InitializeItems()
        //{
        //    Items = new ObservableCollection<Items>();
        //    for (int i = 1; i <= 7; i++)
        //    {
        //        this.Items.Add(new Items()
        //        {
        //            Count = i,
        //            Title = "Folder " + i + " With " + i + " To   be " + i,
        //            Icon = Folder,
        //            Color = App.Current.Resources["MainFolderColor"] as SolidColorBrush
        //        });
        //    }

        //    for (int i = 1; i <= 7; i++)
        //    {
        //        this.Items.Add(new Items()
        //        {
        //            Count = i,
        //            Title = "Entry " + i +" With " + i + " To   be " + i,
        //            Icon = Key
        //            ,
        //            Color = App.Current.Resources["MainAppColor"] as SolidColorBrush
        //        });
        //    }
        //}

        /// <summary>
        /// A collection for <see cref="DataItemViewModel"/> objects.
        /// </summary>
    
    }
}
