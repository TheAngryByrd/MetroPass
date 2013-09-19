using System;
using System.Windows;
using Metropass.Core.PCL.Model;
using MyToolkit.UI;
using MetroPass.WP8.UI.ViewModels;

namespace MetroPass.WP8.UI.Common
{

    public class PwCommonTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Group
        {
            get;
            set;
        }

        public DataTemplate Entry
        {
            get;
            set;
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PwCommon foodItem = item as PwCommon;
            if (foodItem != null)
            {
                if (foodItem is PwGroup)
                {
                    return Group;
                }              
                else
                {
                    return Entry;
                }
            }

            return null;
        }
    }

}
