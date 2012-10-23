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

using Caliburn.Micro;

namespace Cocktail
{
    /// <summary>A collection of extension methods to interact with the dialog host.</summary>
    public static class DialogHostFns
    {
        /// <summary>Returns a reference to the dialog host if the provided ViewModel is currently hosted in the dialog host.</summary>
        /// <param name="source">The hosted ViewModel.</param>
        /// <returns>Null if the ViewModel is not currently hosted in a dialog host, otherwise a reference to the current dialog host.</returns>
        public static IDialogHost DialogHost(this IChild source)
        {
            return source.Parent as IDialogHost;
        }
    }
}