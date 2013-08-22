﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using MetroPass.WP8.UI.ViewModels.ReactiveCaliburn;

namespace MetroPass.WP8.UI.ViewModels
{
    public struct Items
    {
        public string Icon { get; set; }
        public String Title { get; set; }
        public int Count { get; set; }

        public SolidColorBrush Color { get; set; }
    }

    public class EntriesListViewModel : ReactiveScreen
    {

        public EntriesListViewModel()
        {
            InitializeItems();
        }

        const string Key = "\uE192";
        const string Folder = "\uE1C1";

        public string GroupId { get; set; }

        /// <summary>
        /// Initializes the items.
        /// </summary>
        private void InitializeItems()
        {
            Items = new ObservableCollection<Items>();
            for (int i = 1; i <= 7; i++)
            {
                this.Items.Add(new Items()
                {
                    Count = i,
                    Title = "Folder " + i + " With " + i + " To   be " + i,
                    Icon = Folder,
                    Color = App.Current.Resources["MainFolderColor"] as SolidColorBrush
                });
            }

            for (int i = 1; i <= 7; i++)
            {
                this.Items.Add(new Items()
                {
                    Count = i,
                    Title = "Entry " + i +" With " + i + " To   be " + i,
                    Icon = Key
                    ,
                    Color = App.Current.Resources["MainAppColor"] as SolidColorBrush
                });
            }
        }

        /// <summary>
        /// A collection for <see cref="DataItemViewModel"/> objects.
        /// </summary>
        public ObservableCollection<Items> Items
        {
            get;
            set;
        }
    }
}
