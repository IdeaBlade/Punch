using System;
using System.ComponentModel.Composition;

namespace Cocktail
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = false), MetadataAttribute]
    public class InterfaceExportAttribute : InheritedExportAttribute
    {
        // Methods
        public InterfaceExportAttribute(Type exportedType)
            : base(exportedType)
        {
        }
    }


}
