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

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SILVERLIGHT
[assembly: AssemblyTitle("Caliburn.Micro.Extensions.SL")]
#else
[assembly: AssemblyTitle("Caliburn.Micro.Extensions.Desktop")]
#endif
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IdeaBlade")]
[assembly: AssemblyProduct("IdeaBlade Application Framework")]
[assembly: AssemblyCopyright("Copyright © IdeaBlade 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguageAttribute("en")]

[assembly: XmlnsDefinition("http://www.ideablade.com", "Caliburn.Micro.Extensions")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
#if SILVERLIGHT
[assembly: Guid("8b14639a-659b-4fb1-bd8b-76e598617d51")]
#else
[assembly: Guid("d6183ef2-0daf-4b3e-b689-b81bee954828")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("2.0.5.81")]

