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

using System.ComponentModel.Composition;
using System.Text;
using Cocktail;
using DomainModel;
using Security;

namespace DomainServices.SampleData
{
    /// <summary>
    /// Security sample data. Will be loaded TempHireEntities into the same fake store
    /// </summary>
    [Export(typeof(ISampleDataProvider<TempHireEntities>))]
    public class SecuritySampleDataProvider : ISampleDataProvider<TempHireEntities>
    {
        public void AddSampleData(TempHireEntities manager)
        {
            var user = new User
                           {
                               Id = CombGuid.NewGuid(),
                               Username = "Admin"
                           };
            user.SetPassword("password");
            manager.AttachEntity(user);
        }
    }
}