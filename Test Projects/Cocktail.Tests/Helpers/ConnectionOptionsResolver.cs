// ====================================================================================================================
//  Copyright (c) 2012 IdeaBlade
// ====================================================================================================================
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//  WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//  OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//  OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// ====================================================================================================================
//  USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//  http://cocktail.ideablade.com/licensing
// ====================================================================================================================

using System.Collections.Generic;

namespace Cocktail.Tests.Helpers
{
    public class ConnectionOptionsResolver : IConnectionOptionsResolver
    {
        private static readonly Dictionary<string, ConnectionOptions> ConnectionOptions =
            new Dictionary<string, ConnectionOptions>();

        #region IConnectionOptionsResolver Members

        public ConnectionOptions GetConnectionOptions(string name)
        {
            if (!ConnectionOptions.ContainsKey(name))
                return null;

            return ConnectionOptions[name];
        }

        #endregion

        public static void Add(ConnectionOptions connectionOptions)
        {
            if (ConnectionOptions.ContainsKey(connectionOptions.Name))
                ConnectionOptions.Remove(connectionOptions.Name);

            ConnectionOptions.Add(connectionOptions.Name, connectionOptions);
        }
    }
}