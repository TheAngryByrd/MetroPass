using System.Windows;
using MyToolkit.UI;
using MetroPass.WP8.UI.Services;

namespace MetroPass.WP8.UI.Common
{
    public class CloudItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Folder { get; set; }

        public DataTemplate File { get; set; }

        public DataTemplate KeepassFile { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var skyDriveItem = item as ICloudItem;
            if (skyDriveItem.IsFolder)            
                return Folder;            
            else if (skyDriveItem.IsKeePassItem)
                return KeepassFile;
            else 
                return File;
        }
    }
}