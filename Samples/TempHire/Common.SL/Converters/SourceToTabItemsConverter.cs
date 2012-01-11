using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Common.Converters
{
    public class SourceToTabItemsConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var source = (IEnumerable) value;
                if (source != null)
                {
                    var tabItems = new List<TabItem>();

                    foreach (object item in source)
                    {
                        var tabItem = new TabItem
                                          {
                                              DataContext = item,
                                              Content = new ContentControl {Template = (ControlTemplate) parameter}
                                          };

                        var binding = new Binding("DisplayName");
                        tabItem.SetBinding(TabItem.HeaderProperty, binding);

                        tabItems.Add(tabItem);
                    }

                    return tabItems;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> 
        /// ConvertBack method is not supported 
        /// </summary> 
        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new NotSupportedException("ConvertBack method is not supported");
        }

        #endregion
    }
}