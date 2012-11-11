using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;

namespace Cocktail
{
    public class CompositionHost
    {
        private static CompositionHost __instance;
        private static readonly object __lockObject;
        public static event EventHandler<RecomposedEventArgs> Recomposed;
        public static CompositionHost Instance
        {
            get
            {
                if (__instance == null)
                {
                    lock (__lockObject)
                    {
                        __instance = new CompositionHost();
                    }
                }
                return __instance;
            }
        }

        private CompositionContainer _mainContainer;
        public CompositionContainer Container
        {
            get
            {
                return this._mainContainer;
            }
            set
            {
                this._mainContainer = value;
            }
        }

        static CompositionHost()
        {
            __lockObject = new object();
        }


        public CompositionHost()
        {
            Load();
        }

        private void Load()
        {
            var catalog = new AggregateCatalog();
            _mainContainer = new CompositionContainer(catalog);
        }
    }
}
