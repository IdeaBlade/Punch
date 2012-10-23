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
using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace Cocktail
{
    /// <summary>
    /// A convention for applying a <see cref="Converter"/> to a binding when
    /// the binding matches the convention's <see cref="Filter"/>
    /// </summary>
    /// <remarks>
    /// See <see cref="PathToImageSourceConverter"/> for an example.
    /// These conventions should be registered with the
    /// <see cref="ValueConverterConventionRegistry"/>.
    /// </remarks>
    public class ValueConverterConvention
    {
        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="filter"></param>
        public ValueConverterConvention(IValueConverter converter, Func<DependencyProperty, PropertyInfo, bool> filter)
        {
            if (null == converter) throw new ArgumentNullException("converter");
            if (null == filter) throw new ArgumentNullException("filter");
            Converter = converter;
            Filter = filter;
        }

        /// <summary>
        /// <see cref="IValueConverter"/> to use when the binding passes the <see cref="Filter"/>
        /// </summary>
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Filter function returns true if the binding is appropriate for this <see cref="Converter"/>.
        /// </summary>
        /// <remarks>
        /// Binding appropriateness determined by the binding property and the data property.
        /// See <see cref="PathToImageSourceConverter"/> for an example.
        /// </remarks>
        public Func<DependencyProperty, PropertyInfo, bool> Filter { get; set; }
 
    }
}
