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
using IdeaBlade.Core;

namespace Cocktail
{
    /// <summary>
    /// Represents a command in the UI.
    /// </summary>
    public interface IUICommand : IHideObjectMembers
    {
        /// <summary>
        /// Gets the optional label of the UI command.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Specifies whether this command is the default command.
        /// </summary>
        bool IsDefaultCommand { get; }

        /// <summary>
        /// Specifies whether this command is used to cancel an operation.
        /// </summary>
        bool IsCancelCommand { get; }

        /// <summary>
        /// Enables or disables the UI command.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Returns true if the command invocation was cancelled.
        /// </summary>
        bool WasCancelled { get; }

        /// <summary>
        /// Event triggerd when the UI command is being invoked.
        /// </summary>
        event EventHandler<UICommandInvokedArgs> Invoked;
    }
}