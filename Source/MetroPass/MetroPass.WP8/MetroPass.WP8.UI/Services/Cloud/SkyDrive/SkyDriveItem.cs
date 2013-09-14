using System;
using System.Collections.Generic;
using System.Linq;

namespace MetroPass.WP8.UI.Services.Cloud.Skydrive
{
    public class SkyDriveItem : ICloudItem
    {
        public SkyDriveItem(IDictionary<string, object> properties)
        {
            if (properties.ContainsKey("id"))
            {
                this.ID = properties["id"] as string;
            }

            if (properties.ContainsKey("name"))
            {
                this.Name = properties["name"] as string;
            }

            if (properties.ContainsKey("type"))
            {
                this.ItemType = properties["type"] as string;
            }
        }

        public SkyDriveItem(string Id, string name, string itemType)
        {
            ID = Id;
            Name = name;
            ItemType = itemType;
        }

        public string ID { get; private set; }

        public string Name { get; private set; }

        public string ItemType { get; private set; }

        public string UploadPath
        {
            get
            {
            return ID;
            }
        }

        public bool IsFolder
        {
            get
            {
                return !string.IsNullOrEmpty(this.ItemType) &&
                       (this.ItemType.Equals("folder") || this.ItemType.Equals("album"));
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

        public override string ToString()
        {
            return this.Name;
        }
    }
}
