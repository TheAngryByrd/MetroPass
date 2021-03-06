﻿using System;
using System.Linq;

namespace MetroPass.WP8.UI.Services.Cloud.Dropbox
{
    public class DropboxItem : ICloudItem
    {
        public DropboxItem(string path, string name, string icon)
        {
            ID = path;
            Name = name;
            ItemType = icon;
        }

        public string ID { get; private set; }

        public string Name { get; private set; }

        public string ItemType { get; private set; }

        public string UploadPath
        {
            get
            {
                var path = ID;

                return path.Substring(path.LastIndexOf('/'));
            }
        }

        public bool IsFolder
        {
            get
            {
                return !string.IsNullOrEmpty(this.ItemType) &&
                        (this.ItemType.Contains("folder"));
            }
        }

        private string[] knownFileTypes = new[] { "kdbx", "key" };
        public bool IsKeePassItem
        {
            get
            {
                var filetype = Name.Split('.').Last();
                return knownFileTypes.Contains(filetype);
            }
        }
    }
}
