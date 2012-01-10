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
using System.Resources;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

#if SILVERLIGHT
[assembly: AssemblyTitle("IB.Application.Framework.Core.SL")]
#else
[assembly: AssemblyTitle("IB.Application.Framework.Core.Desktop")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IdeaBlade")]
[assembly: AssemblyProduct("IdeaBlade Application Framework")]
[assembly: AssemblyCopyright("Copyright © IdeaBlade 2011")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

#if SILVERLIGHT
[assembly: InternalsVisibleTo("Core.Tests.SL, PublicKey=00240000048000009400000006020000002400005253413100040000010001000711e4133e5919"
                                                     + "6b877f67b04088121b1d6c3a25cb8791422c56986774125d0b805da768e11c38bbe2a395a98611"
                                                     + "5a66ef56c993ca31bef5d998cd98d53ba73a4c5b69fbbf197267b0b6c9ab232f8ff83794df4c7f"
                                                     + "e3ec9ce25e749ba984ed28135cca779fef9aca4e421831d13a6e8d2b3cb321002d5d3511895476"
                                                     + "a6a04dc8")]
[assembly: InternalsVisibleTo("Caliburn.Micro.Extensions.SL, PublicKey=00240000048000009400000006020000002400005253413100040000010001000711e4133e5919"
                                                     + "6b877f67b04088121b1d6c3a25cb8791422c56986774125d0b805da768e11c38bbe2a395a98611"
                                                     + "5a66ef56c993ca31bef5d998cd98d53ba73a4c5b69fbbf197267b0b6c9ab232f8ff83794df4c7f"
                                                     + "e3ec9ce25e749ba984ed28135cca779fef9aca4e421831d13a6e8d2b3cb321002d5d3511895476"
                                                     + "a6a04dc8")]
[assembly: InternalsVisibleTo("IdeaBlade.Core.SL, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b3f302890eb528"
                                                     + "1a7ab39b936ad9e0eded7c4a41abb440bead71ff5a31d51e865606b2a7e6d0b9dd0d92b113b9d1"
                                                     + "0fb13f01fb5d856e99c1e61777cf4772d29bad7e66ffb93fc5cbd63b395046c06ff57db6ecbeee"
                                                     + "4bdd6effc405878d65cfc4911708ed650da935d733fc5dc707f74910e025ac080543e01a6cc863"
                                                     + "b9f85ffc")]
#else
[assembly: InternalsVisibleTo("Caliburn.Micro.Extensions.Desktop, PublicKey=00240000048000009400000006020000002400005253413100040000010001000711e4133e5919"
                                                     + "6b877f67b04088121b1d6c3a25cb8791422c56986774125d0b805da768e11c38bbe2a395a98611"
                                                     + "5a66ef56c993ca31bef5d998cd98d53ba73a4c5b69fbbf197267b0b6c9ab232f8ff83794df4c7f"
                                                     + "e3ec9ce25e749ba984ed28135cca779fef9aca4e421831d13a6e8d2b3cb321002d5d3511895476"
                                                     + "a6a04dc8")]
[assembly: InternalsVisibleTo("IdeaBlade.Core, PublicKey=0024000004800000940000000602000000240000525341310004000001000100b3f302890eb528"
                                                     + "1a7ab39b936ad9e0eded7c4a41abb440bead71ff5a31d51e865606b2a7e6d0b9dd0d92b113b9d1"
                                                     + "0fb13f01fb5d856e99c1e61777cf4772d29bad7e66ffb93fc5cbd63b395046c06ff57db6ecbeee"
                                                     + "4bdd6effc405878d65cfc4911708ed650da935d733fc5dc707f74910e025ac080543e01a6cc863"
                                                     + "b9f85ffc")]
#endif

[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.ApplicationState")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.Authentication")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.Collections.Generic")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.Composition")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.DesignTimeSupport")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.Persistence")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.Sync")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.Verification")]
[assembly: XmlnsDefinition("http://www.ideablade.com", "IdeaBlade.Application.Framework.Core.ViewModel")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

#if SILVERLIGHT
[assembly: Guid("8b383663-ad4a-4f35-a574-dcdca85c7450")]
#else
[assembly: Guid("ac99aa4d-698a-4399-a48c-e6189c661df1")]
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