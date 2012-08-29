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

namespace Common.Workspace
{
    public class Workspace : IWorkspace
    {
        private readonly bool _default;
        private readonly int _sequence;

        public Workspace(string displayName, bool @default, int sequence, Type viewModelType)
        {
            DisplayName = displayName;
            ViewModelType = viewModelType;
            _default = @default;
            _sequence = sequence;
        }

        #region IWorkspace Members

        public string DisplayName { get; set; }
        public Type ViewModelType { get; private set; }

        public bool IsDefault
        {
            get { return _default; }
        }

        public int Sequence
        {
            get { return _sequence; }
        }

        #endregion
    }
}