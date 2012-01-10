//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
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

using System.ComponentModel;

namespace IdeaBlade.Application.Framework.Core.DesignTimeSupport
{
    /// <summary>
    /// Determines if presently executing in a visual Design Tool or not.
    /// </summary>
    /// <remarks>
    /// Implementation is almost verbatim from Laurent Bugnion's blog post
    /// http://geekswithblogs.net/lbugnion/archive/2009/09/05/detecting-design-time-mode-in-wpf-and-silverlight.aspx
    /// Look there for a discussion of the code and its limitations.
    /// </remarks>
    public static class DesignModeProber
    {
        private static bool _isModeKnown;
        private static bool _isInDesignMode;

        /// <summary>
        /// Get whether presently running in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isModeKnown)
                {
                    _isModeKnown = true;
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            _isInDesignMode
                = (bool)DependencyPropertyDescriptor
                .FromProperty(prop, typeof(System.Windows.FrameworkElement))
                .Metadata.DefaultValue;
#endif
                }

                return _isInDesignMode;
            }
        }
    }
}