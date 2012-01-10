//====================================================================================================================
//Copyright (c) 2011 IdeaBlade
//====================================================================================================================
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//====================================================================================================================
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//the Software.
//====================================================================================================================
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
//OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================

using IdeaBlade.Validation;

namespace IdeaBlade.Application.Framework.Core.Verification
{
    /// <summary>Implement this interface on any DevForce entity in addition to or instead of the DevForce Verifiers to provide validation logic during a save. The
    /// framework will automatically call the Verify method in addition to all the DevForce Verifiers on the entity and aborts the save if any validation errors occur.</summary>
    /// <seealso cref="IVerifierResultNotificationService"></seealso>
    public interface ICustomVerifier
    {
        /// <summary>This method is called during a save to allow for custom validation of an entity.</summary>
        /// <param name="verifierResultCollection">Validation errors should be added to the provided collection.</param>
        void Verify(VerifierResultCollection verifierResultCollection);
    }
}
