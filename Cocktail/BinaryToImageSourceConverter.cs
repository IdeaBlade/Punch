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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cocktail
{
    /// <summary>Converts a byte array of image data to an image source.</summary>
    /// <remarks>This implementation only tolerates jpeg and pngs</remarks>
    public class BinaryToImageSourceConverter : IValueConverter
    {
        /// <summary>
        /// Register this instance with <see cref="ValueConverterConventionRegistry"/>
        /// </summary>
        public void RegisterConvention()
        {
            ValueConverterConventionRegistry.RegisterConvention(this, Image.SourceProperty, typeof(byte[]));
        }

        /// <summary>Converts a byte array of image data to an image source.</summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(ImageSource))
                throw new InvalidOperationException(StringResources.TargetTypeMustBeImageSource);

            return GetImageFromBytes(value as byte[]);
        }

        /// <summary>Conversion from image to byte array is not implemented.</summary>
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static object GetImageFromBytes(byte[] binaryImageData)
        {
            if (null == binaryImageData || 0 == binaryImageData.Length)
                return DependencyProperty.UnsetValue;
            try
            {
                ThrowIfInvalidImageData(binaryImageData);
                var img = new BitmapImage();
#if SILVERLIGHT
                using (var stream = new MemoryStream(binaryImageData))
                {
                    // ToDo: consider substituting a "Missing Image" instead
                    // See PathToImageConverter
                    img.SetSource(stream);
                }
#else
                img.BeginInit();
                img.StreamSource = new MemoryStream(binaryImageData);
                img.EndInit();
#endif
                return img;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        private static void ThrowIfInvalidImageData(byte[] binaryImageData)
        {
            if (!IsJpegOrPngBinaryImage(binaryImageData))
            {
                // ToDo: consider substituting a "Missing Image" instead of throwing
                // See PathToImageConverter
                throw new InvalidOperationException(StringResources.InvalidBinaryImageData);
            }
        }

        private static bool IsJpegOrPngBinaryImage(byte[] binaryImageData)
        {
            if (binaryImageData.Length > 2)
            {
                if (binaryImageData[0].Equals(0XFF) && binaryImageData[1].Equals(0XD8))
                {
                    return true;
                }



                if (binaryImageData[0].Equals(0X89) && binaryImageData[1].Equals(0X50))
                {
                    return true;
                }
            }
            return false;
        }
    }
}