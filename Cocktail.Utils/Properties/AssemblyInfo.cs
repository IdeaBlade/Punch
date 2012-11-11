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
using System.Resources;
using System.Runtime.InteropServices;

#if !NETFX_CORE
using System.Windows.Markup;
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

#if SILVERLIGHT
[assembly: AssemblyTitle("Cocktail.Utils.SL")]
#elif NETFX_CORE
[assembly: AssemblyTitle("Cocktail.Utils.WinRT")]
#else
[assembly: AssemblyTitle("Cocktail.Utils")]
#endif

[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("IdeaBlade")]
[assembly: AssemblyProduct("Cocktail")]
[assembly: AssemblyCopyright("Copyright © IdeaBlade 2012")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("en")]

#if !NETFX_CORE
[assembly: XmlnsDefinition("http://cocktail.ideablade.com", "Cocktail")]
#endif


// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

#if SILVERLIGHT
[assembly: Guid("8317E417-7D94-4519-B745-65A029B7D5B3")]
#elif !NETFX_CORE
[assembly: Guid("DA34E3F7-F87D-47B5-B11E-7CE00E957DB8")]
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

[assembly: AssemblyVersion("2.0.5")]
