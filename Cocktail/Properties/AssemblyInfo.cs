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

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if SILVERLIGHT
[assembly: AssemblyTitle("Cocktail.SL")]
#else
[assembly: AssemblyTitle("Cocktail")]
#endif
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IdeaBlade")]
[assembly: AssemblyProduct("Cocktail")]
[assembly: AssemblyCopyright("Copyright © IdeaBlade 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguageAttribute("en")]

[assembly: InternalsVisibleTo("Caliburn.Micro, PublicKey=0024000004800000940000000602000000240000525341310004000001000100a5f0e53ba56f57"
                                                     + "28067f02a18dcee7f9fc52dc8ff49a74407ef2a26b6c6d3eaa1c4701bd9f4c8fee660aa8b13c2a"
                                                     + "1de97ebfe3204bcc12cc61e069ebbe3d3732cfaab87502fc26efa5475caf108fa49e0f397b7d2a"
                                                     + "feb2349d72fcc11906f25c100db2d9167f55c998f0020ffec532dc1166e5a9eab8e961b3f711a1"
                                                     + "abff6ebe")]

#if SILVERLIGHT
[assembly: InternalsVisibleTo("Cocktail.Tests.SL, PublicKey=00240000048000009400000006020000002400005253413100040000010001000711e4133e5919"
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
[assembly: InternalsVisibleTo("Cocktail.Tests, PublicKey=00240000048000009400000006020000002400005253413100040000010001000711e4133e5919"
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

[assembly: XmlnsDefinition("http://cocktail.ideablade.com", "Cocktail")]


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
#if SILVERLIGHT
[assembly: Guid("0EE7DDF4-04AB-4048-B7C9-5B69DAEE5430")]
#else
[assembly: Guid("869C0A7C-3B83-44C0-8836-360EA78C9D6E")]
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
[assembly: AssemblyVersion("1.0.0.918")]

