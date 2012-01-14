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
using System.Windows.Controls;
using System.Windows.Data;

namespace Cocktail
{

    /// <summary>
    /// Adds stock Cocktail ValueConverter conventions to the
    /// <see cref="Caliburn.Micro.ConventionManager"/>.
    /// </summary>
    public class ValueConverterConventions
    {
        /// <summary>
        /// Add stock Cocktail ValueConverter conventions to the
        /// <see cref="Caliburn.Micro.ConventionManager"/>.
        /// </summary>
        public static void AddConventions()
        {
            var currentApplyValueConverter = Caliburn.Micro.ConventionManager.ApplyValueConverter;
            Caliburn.Micro.ConventionManager.ApplyValueConverter = (binding, bindableProperty, property) =>
            {
                // Apply prior rules first
                currentApplyValueConverter(binding, bindableProperty, property);
                if (null != binding.Converter) return; // already assigned

                if (bindableProperty == Image.SourceProperty)
                {
                    if (typeof(string).IsAssignableFrom(property.PropertyType))
                    {
                        binding.Converter = PathToImageSourceConverter;
                    }
                    else if (typeof(byte[]).IsAssignableFrom(property.PropertyType))
                    {
                        binding.Converter = BinaryToImageSourceConverter;
                    }
                }
            };
        }

        /// <summary>Converter that converts a string source path to an image source.</summary>
        public static IValueConverter PathToImageSourceConverter = new PathToImageSourceConverter();

        /// <summary>Converter that converts a byte array of image data to an image source.</summary>
        public static IValueConverter BinaryToImageSourceConverter = new BinaryToImageSourceConverter();
    }

    //TODO: Breakout into own files within Cocktail.UI
}
