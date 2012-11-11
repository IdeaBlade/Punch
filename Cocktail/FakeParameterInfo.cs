using System;
using System.Reflection;

namespace Cocktail
{
    /// <summary>
    /// This class allows us to fake a ParameterInfo object. We use this to create a ReflectionImportDefinition
    /// </summary>
    public class FakeParameterInfo : ParameterInfo
    {
        private readonly Type _type;

        /// <summary>
        /// Creates a new ParameterInfo object with our type.
        /// </summary>
        /// <param name="type">The type we want the ParameterInfo object to expose</param>
        public FakeParameterInfo(Type type)
        {
            _type = type;
        }

        public override Type ParameterType
        {
            get { return _type; }
        }
    }
}
