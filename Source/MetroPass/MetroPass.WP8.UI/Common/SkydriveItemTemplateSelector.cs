using System.Windows;
using MetroPass.WP8.UI.ViewModels;
using MyToolkit.UI;

namespace MetroPass.WP8.UI.Common
{
    public class SkydriveItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Folder { get; set; }

        public DataTemplate File { get; set; }

        public DataTemplate KeepassFile { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var skyDriveItem = item as SkyDriveItem;
            if (skyDriveItem.IsFolder)            
                return Folder;            
            else if (skyDriveItem.IsKeePassItem)
                return KeepassFile;
            else 
                return File;
        }
    }
}