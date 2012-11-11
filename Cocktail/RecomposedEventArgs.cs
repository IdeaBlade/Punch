using System;
using System.Reflection;

namespace Cocktail
{
    public class RecomposedEventArgs : EventArgs
    {
        // Methods
        public RecomposedEventArgs()
        {
        }

        public RecomposedEventArgs(Assembly assembly = null, bool shouldRecompose = true)
        {
            this.Assembly = assembly;
            this.ShouldRecompose = shouldRecompose;
        }

        // Properties
        public Assembly Assembly { get; private set; }

        public bool AssemblyLoaded
        {
            get
            {
                return (this.Assembly != null);
            }
        }

        public bool HasError
        {
            get
            {
                return false;
            }
        }

        public bool ShouldRecompose { get; private set; }
    }

 

}
