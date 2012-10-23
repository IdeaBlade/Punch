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

using IdeaBlade.Core;
using IdeaBlade.Core.Composition;
using IdeaBlade.Validation;

namespace Cocktail
{
    /// <summary>Implement this interface to be notified of validation errors during a save. The framework automatically performs validation before saving changed
    /// entities. If any validation errors occur, the save is aborted and any implementation of IValidationErrorNotification is notified of the error(s).</summary>
    /// <example>
    /// 	<code title="Example" description="In this example, the implementation of IValidationErrorNotification publishes a message to the UI EventAggregator for consumption by any view model and processing of the validation error." lang="CS">
    /// // Create this implementation as a singleton.
    /// [PartCreationPolicy(CreationPolicy.Shared)]
    /// public class ValidationErrorProcessor : IValidationErrorNotification
    /// {
    ///     // Let the UI know that a validation error occurred.
    ///     public OnValidationError(VerifierResultCollection validationErrors)
    ///     {
    ///         EventFns.Publish(new HandleValidationErrors(validationErrors));
    ///     }
    /// }</code>
    /// </example>
    [InterfaceExport(typeof(IValidationErrorNotification))]
    public interface IValidationErrorNotification : IHideObjectMembers
    {
        /// <summary>Method called by the framework if validation errors occurred during the save.</summary>
        /// <param name="validationErrors">Collection containing all validation errors.</param>
        void OnValidationError(VerifierResultCollection validationErrors);
    }
}
