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

#if !LIGHT
using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
#endif

namespace Cocktail
{
    /// <summary>
    /// Marker interface to make a ViewModel discoverable. 
    /// 
    /// This interface is leveraged by the Development Harness to automatically
    /// generate a list of views that can be launched.
    /// </summary>
    [InterfaceExport(typeof(IDiscoverableViewModel))]
    public interface IDiscoverableViewModel : IHideObjectMembers
    {
    }
}