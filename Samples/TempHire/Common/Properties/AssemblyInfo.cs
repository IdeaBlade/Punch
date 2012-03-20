using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SILVERLIGHT
[assembly: AssemblyTitle("Common.SL")]
#else
[assembly: AssemblyTitle("Common")]
#endif
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IdeaBlade")]
[assembly: AssemblyProduct("TempHire")]
[assembly: AssemblyCopyright("Copyright © 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if SILVERLIGHT
[assembly: InternalsVisibleTo("IdeaBlade.Core.SL, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b3f302890eb528"
                                                     + "1a7ab39b936ad9e0eded7c4a41abb440bead71ff5a31d51e865606b2a7e6d0b9dd0d92b113b9d1"
                                                     + "0fb13f01fb5d856e99c1e61777cf4772d29bad7e66ffb93fc5cbd63b395046c06ff57db6ecbeee"
                                                     + "4bdd6effc405878d65cfc4911708ed650da935d733fc5dc707f74910e025ac080543e01a6cc863"
                                                     + "b9f85ffc")]
#endif

[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Behaviors")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Converters")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Errors")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Factories")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Messages")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Security")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Toolbar")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Validation")]
[assembly: XmlnsDefinition("http://temphire.ideablade.com/common", "Common.Workspace")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6ee27d01-654e-43d5-8739-2741a79a1089")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
