﻿using System;
using System.Collections.ObjectModel;
using MetroPass.WP8.UI.ViewModels.DesignTime;

namespace MetroPass.WP8.UI.ViewModels
{
    public struct Items
    {
        public string Icon { get; set; }
        public String Title { get; set; }
        public int Count { get; set; }
    }

    public class EntriesListViewModel
    {

        public EntriesListViewModel()
        {
            InitializeItems();
        }

        const string Key = "\uE192";
        const string Folder = "\uE1C1";

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
                    Title = "Folder " + i,
                    Icon = Folder
                });
            }

            for (int i = 1; i <= 7; i++)
            {
                this.Items.Add(new Items()
                {
                    Count = i,
                    Title = "Entry " + i,
                    Icon = Key
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
