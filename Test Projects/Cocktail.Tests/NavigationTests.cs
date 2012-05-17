// ====================================================================================================================
//   Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//   WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//   OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//   OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//   USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//   http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using Caliburn.Micro;
using Cocktail.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Cocktail.Tests
{
    [TestClass]
    public class NavigationTests : CocktailTestBase
    {
        [TestMethod]
        public void ShouldBeIncomplete()
        {
            var navigation = new NavigateResult<object>(new Conductor<object>(), () => null);

            Assert.IsFalse(navigation.IsCompleted);
            Assert.IsFalse(navigation.HasError);
            Assert.IsFalse(navigation.CompletedSuccessfully);
            Assert.IsFalse(navigation.Cancelled);
            Assert.IsNull(navigation.Error);
        }
    }
}