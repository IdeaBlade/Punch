//  ====================================================================================================================
//    Copyright (c) 2012 IdeaBlade
//  ====================================================================================================================
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//    WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//    OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//    OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//  ====================================================================================================================
//    USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
//    http://cocktail.ideablade.com/licensing
//  ====================================================================================================================

using System;

namespace Cocktail
{
    /// <summary>
    /// Represents a command in a message or dialog box.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDialogUICommand<T> : IUICommand
    {
        /// <summary>
        /// The dialog result associated with this command.
        /// </summary>
        T DialogResult { get; }
    
        /// <summary>
        /// Event triggerd when the UI command is being invoked.
        /// </summary>
        new event EventHandler<DialogUICommandInvokedArgs> Invoked;
    }
}