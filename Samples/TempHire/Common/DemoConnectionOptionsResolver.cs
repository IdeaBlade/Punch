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

using System;
using Cocktail;

namespace Common
{
    /// <summary>
    /// Internal use for demo purposes. This class is only compiled for the Demo configuration.
    /// </summary>
    public class DemoConnectionOptionsResolver : IConnectionOptionsResolver
    {
        private static Guid? _fakeStoreSession;

        public static Guid FakeStoreSession
        {
            get { return (Guid)(_fakeStoreSession ?? (_fakeStoreSession = Guid.NewGuid())); }
        }

        /// <summary>
        /// Called by Cocktail to retrieve the ConnectionOptions having the specified name.
        /// </summary>
        /// <param name="name">The name of the ConnectonOptions to be retrieved.</param>
        /// <returns/>
        public ConnectionOptions GetConnectionOptions(string name)
        {
            // The demo build redirects all connections to an isolated fake store per user session
            return ConnectionOptions.Fake.WithCompositionContext(FakeStoreSession.ToString()).WithName(name);
        }
    }
}