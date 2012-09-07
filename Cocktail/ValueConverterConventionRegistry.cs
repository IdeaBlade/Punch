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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>
    /// A static registry of <see cref="ValueConverterConvention"/>s that are
    /// added to the Caliburn <see cref="ConventionManager"/>.
    /// </summary>
    /// <remarks>
    /// Registered conventions become part of the Caliburn.Micro conventions when 
    /// <see cref="AddConventionsToConventionManager"/> is called
    /// as it is in the <see cref="CocktailMefBootstrapper"/>.
    /// <para>
    /// You can continue adding conventions after adding the ConventionRegistry 
    /// to the Caliburn conventions.
    /// </para>
    /// </remarks>
    public static class ValueConverterConventionRegistry
    {
        /// <summary>
        /// Add Cocktail ValueConverter conventions to the Caliburn <see cref="ConventionManager"/>.
        /// </summary>
        /// <remarks>Harmless to call twice because only adds once.</remarks>
        public static void AddConventionsToConventionManager()
        {
            if (HaveAddedToConventionManager) return;
            HaveAddedToConventionManager = true;

            var oldApplyValueConverter = ConventionManager.ApplyValueConverter;
            ConventionManager.ApplyValueConverter = 
                (binding, bindableProperty, property) =>
                {
                    // Apply prior rules first
                    oldApplyValueConverter(binding, bindableProperty, property);
                    if (null != binding.Converter) return; // already assigned

                    // Find matching ValueConverterConvention and apply it.
                    // Last matching convention wins.
                    var convention =
                        Conventions.LastOrDefault(c => c.Filter(bindableProperty, property));
                    if (null != convention) binding.Converter = convention.Converter;

                };
        }

        /// <summary>
        /// Get if the conventions have already been added to the <see cref="ConventionManager"/>
        /// </summary>
        internal static bool HaveAddedToConventionManager { get; set; }

        private static readonly List<ValueConverterConvention> Conventions = 
            new List<ValueConverterConvention>();

        /// <summary>
        /// Creates a <see cref="ValueConverterConvention"/> and adds it to the registry.
        /// Creates a convention for a <see cref="IValueConverter"/> with a <see cref="ValueConverterConvention.Filter"/> 
        /// that matches the binding's
        /// <cref param="bindableProperty"/> and <cref param="dataPropertyType"/> exactly.
        /// </summary>
        /// <param name="converter">The converter instance returned by the <see cref="ValueConverterConvention"/>.</param>
        /// <param name="bindableProperty">The binding property to which this converter applies.</param>
        /// <param name="dataPropertyType">The type of the data property that this converter can convert.</param>
        /// <remarks>
        /// See <see cref="PathToImageSourceConverter"/> for an example.
        /// </remarks>
        public static void RegisterConvention(IValueConverter converter, DependencyProperty bindableProperty, Type dataPropertyType)
        {
            if (null == bindableProperty) throw new ArgumentNullException("bindableProperty");
            if (null == dataPropertyType) throw new ArgumentNullException("dataPropertyType");
            if (null == converter) throw new ArgumentNullException("converter");

            RegisterConvention(
                new ValueConverterConvention(converter, (bindProperty, propertyInfo) =>
                                                        bindProperty == bindableProperty &&
                                                        dataPropertyType.IsAssignableFrom(propertyInfo.PropertyType))
                );
        }

        /// <summary>
        /// Add a <see cref="ValueConverterConvention"/> to the registry
        /// </summary>
        public static void RegisterConvention(ValueConverterConvention convention)
        {
            if (null == convention) throw new ArgumentNullException("convention");
            Conventions.Add(convention);
        }

        /// <summary>
        /// Get the registered <see cref="ValueConverterConvention"/>s.
        /// </summary>
        public static ICollection<ValueConverterConvention> GetConventions
        {
            get { return Conventions; }
        }

    }
}