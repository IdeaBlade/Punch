//====================================================================================================================
//Copyright (c) 2012 IdeaBlade
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

using System;

namespace Cocktail
{
    /// <summary>Represents a user's response to a dialog or message box.</summary>
    public enum DialogResult
    {
        /// <summary>Nothing. This means that the dialog or message box continues running.</summary>
        None,
        /// <summary>The user clicked the Ok button.</summary>
        Ok,
        /// <summary>The user clicked the Cancel button.</summary>
        Cancel,
        /// <summary>The user clicked the Abort button.</summary>
        Abort,
        /// <summary>The user clicked the Retry button.</summary>
        Retry,
        /// <summary>The user clicked the Ignore button.</summary>
        Ignore,
        /// <summary>The user clicked the Yes button.</summary>
        Yes,
        /// <summary>The user clicked the No button.</summary>
        No
    };

    /// <summary>An interface allowing a hosted custom view model to participate in the dialog host lifecycle.</summary>
    public interface IDialogHostDelegate
    {
        /// <summary>A property to indicate to the dialog host, whether the hosted view model is complete. This property controls whether the Ok button is enabled or
        /// disabled.</summary>
        bool IsComplete { get; }

        /// <summary>The current user response to a dialog or message box.</summary>
        /// <remarks>The dialog host will set the DialogResult upon a user clicking the respective button.</remarks>
        DialogResult DialogResult { get; set; }

        /// <summary>Event to notify the dialog host, that the <see cref="IsComplete"/> property has changed.</summary>
        event EventHandler CompleteChanged;
    }
}