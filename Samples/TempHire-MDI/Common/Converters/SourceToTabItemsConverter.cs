//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
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
                var source = (IEnumerable)value;
                if (source != null)
                {
                    var tabItems = new List<TabItem>();

                    foreach (object item in source)
                    {
                        var tabItem = new TabItem
                                          {
                                              DataContext = item,
                                              Content = new ContentControl { Template = (ControlTemplate)parameter }
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