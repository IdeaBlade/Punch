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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cocktail
{
    /// <summary>Converts a string source path to an image source.</summary>
    public class PathToImageSourceConverter : IValueConverter
    {
        /// <summary>
        /// Register this instance with <see cref="ValueConverterConventionRegistry"/>
        /// </summary>
        public void RegisterConvention()
        {
            ValueConverterConventionRegistry.RegisterConvention(this, Image.SourceProperty, typeof(string));
        }

        /// <summary>Convert a string filepath to an <see cref="ImageSource"/>.</summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                throw new InvalidOperationException(StringResources.TargetTypeMustBeImageSource);

            return ConvertImageFromPath(value as string);
        }

        private object ConvertImageFromPath(string filePath)
        {
            try
            {
                return ToMissingImageIfNull(GetImageFromFilteredPath(filePath));
            }
            catch
            {
                return ToMissingImageIfNull();
            }
        }

        private object ToMissingImageIfNull(object image = null)
        {
            return image ?? MissingImage ?? DefaultMissingImage ?? DependencyProperty.UnsetValue;
        }

        private ImageSource GetImageFromFilteredPath(string filePath)
        {
            var path = (filePath ?? String.Empty).Trim();
            var img = GetImageFromPath((PathFilter ?? DefaultPathFilter)(path));
            ListenForImageLoad(img);
            return img;
        }

        /// <summary>
        /// Convert an unfiltered string filepath to an <see cref="BitmapImage"/>.
        /// </summary>
        /// <remarks>
        /// Gets the <see cref="ImageSource"/> from the raw <cref param="filePath"/>
        /// without using the <see cref="PathFilter"/>.
        /// </remarks>
        public static BitmapImage GetImageFromPath(string filePath)
        {
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
        public Func<string, string> PathFilter { get; set; }

        /// <summary>
        /// Convert to this missing image if there is no image filepath string or can't find the requested image.
        /// </summary>
        public ImageSource MissingImage { get; set; }

        /// <summary>
        /// Default <see cref="MissingImage"/> value;
        /// </summary>
        public static ImageSource DefaultMissingImage { get; set; }

        /// <summary>
        /// Default <see cref="PathFilter"/> function; 
        /// </summary>
        public static Func<string, string> DefaultPathFilter = path => path;

        #region Image Load Handling

        private void ListenForImageLoad(BitmapImage img)
        {
#if SILVERLIGHT
            img.ImageOpened += UnhookImageHandlers;
            img.ImageFailed += ImageFailed;
#else
            img.DownloadCompleted += UnhookImageHandlers;
            img.DownloadFailed += ImageFailed;
            img.DecodeFailed += ImageFailed;
#endif
        }

        private void UnhookImageHandlers(object sender, EventArgs e)
        {
            var img = (BitmapImage) sender;
#if SILVERLIGHT
            img.ImageOpened -= UnhookImageHandlers;
            img.ImageFailed -= ImageFailed;
#else
            img.DownloadCompleted -= UnhookImageHandlers;
            img.DecodeFailed -= ImageFailed;
            img.DownloadFailed -= ImageFailed;
#endif
        }

        private void ImageFailed(object sender, EventArgs e)
        {
            UnhookImageHandlers(sender, e);

            // Try to substitute missing image for the failed image
            var missing = (MissingImage ?? DefaultMissingImage) as BitmapImage;
            if (null == missing) return; // we can't help
            var img = sender as BitmapImage;
            if (null == img) return;
            img.UriSource = missing.UriSource;
        }
        #endregion

        /// <summary>Conversion from image to filepath is not implemented.</summary>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}