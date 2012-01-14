//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cocktail
{
    /// <summary>Converts a string source path to an image source.</summary>
    public class PathToImageSourceConverter : IValueConverter
    {
        /// <summary>Convert a string filepath to an <see cref="ImageSource"/>.</summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                throw new InvalidOperationException("Target type must be System.Windows.Media.ImageSource.");

            return ConvertImageFromPath(value as string);
        }

        private static object ConvertImageFromPath(string filePath)
        {
            try
            {
                return ToMissingImageIfNull(GetImageFromPath(filePath));
            }
            catch
            {
                return ToMissingImageIfNull();
            }
        }

        private static object ToMissingImageIfNull(object image = null)
        {
            return image ?? MissingImage ?? DependencyProperty.UnsetValue;
        }

        /// <summary>Convert a string filepath to an <see cref="ImageSource"/>.</summary>
        public static ImageSource GetImageFromPath(string filePath)
        {
            filePath = PathFilter(filePath);
            if (String.IsNullOrEmpty(filePath)) return null;
            var uri = new Uri(filePath, UriKind.RelativeOrAbsolute);
            var img = new BitmapImage();
#if !SILVERLIGHT
                img.BeginInit();
#endif
            img.UriSource = uri;
#if !SILVERLIGHT
                img.EndInit();
#endif
            return img;
        }

        /// <summary>
        /// Filter or otherwise transform the incoming image filepath into an application-appropriate URI string.
        /// </summary>
        /// <returns>The morphed string or null if there is no image.</returns>
        /// <remarks>
        /// Replace with your own version if you need to manipulate the nominal filepath as you might if
        /// the filepath is an image name and you will prefix it with a base path.
        /// </remarks>
        /// <example>
        /// PathToImageSourceConverter.PathFilter = path => "/MyApp;component/assets/" + path.Trim();
        /// </example>
        public static Func<string, string> PathFilter = path => path;

        /// <summary>
        /// Convert to this missing image if there is no image filepath string or can't find the requested image.
        /// </summary>
        public static ImageSource MissingImage { get; set; }

        /// <summary>Conversion from image to filepath is not implemented.</summary>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}