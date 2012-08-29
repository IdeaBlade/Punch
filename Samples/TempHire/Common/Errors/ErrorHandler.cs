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

using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Cocktail;
using IdeaBlade.EntityModel;

namespace Common.Errors
{
    [Export(typeof (IErrorHandler))]
    public class ErrorHandler : IErrorHandler
    {
        private readonly IDialogManager _dialogManager;

        [ImportingConstructor]
        public ErrorHandler(IDialogManager dialogManager)
        {
            _dialogManager = dialogManager;
        }

        #region IErrorHandler Members

        public void HandleError(Exception ex)
        {
            string customMessage = null;
            if (ex is EntityManagerSaveException &&
                ((EntityManagerSaveException) ex).FailureType == PersistenceFailure.Concurrency)
                customMessage = "Another user has previously saved the current record.";

            if (ex is TaskCanceledException)
            {
                // Log and ignore
                LogFns.DebugWriteLine(ex.Message);
                return;
            }

            _dialogManager.ShowMessageAsync(customMessage ?? ex.Message, DialogButtons.Ok);
        }

        #endregion
    }
}