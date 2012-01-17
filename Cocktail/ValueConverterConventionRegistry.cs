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
    /// as it is in the <see cref="FrameworkBootstrapper"/>.
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
        /// Register the convention for a <see cref="IValueConverter"/> with the <see cref="GetConventions"/>
        /// with a <see cref="ValueConverterConvention.Filter"/> that matches the bindings
        /// <cref param="bindableProperty"/> and <cref param="dataPropertyType"/> exactly.
        /// </summary>
        /// <param name="converter">The converter instance to register</param>
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

            Conventions.Add(
                new ValueConverterConvention(converter, (bindProperty, propertyInfo) =>
                                                        bindProperty == bindableProperty &&
                                                        dataPropertyType.IsAssignableFrom(propertyInfo.PropertyType))
                );
        }

        /// <summary>
        /// Get the registered <see cref="ValueConverterConvention"/>s.
        /// </summary>
        public static IEnumerable<ValueConverterConvention> GetConventions
        {
            get { return Conventions; }
        }

        /// <summary>
        /// Clear all registered conventions.
        /// </summary>
        public static void ClearRegistry() { Conventions.Clear(); }
    }
}