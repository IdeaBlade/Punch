#if (!NETFX_CORE)
using System.ComponentModel.Composition;
#else
using System.Composition;
#endif

namespace Cocktail.Tests.Helpers
{
    [Export]
    public class GenericExport<T>
    {
         
    }

    [Export("ExportWithTypeAndContract", typeof(GenericExportWithTypeAndContractName<>))]
    public class GenericExportWithTypeAndContractName<T>
    {
        
    }
}