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
using System.Collections.Generic;
using IdeaBlade.EntityModel;

namespace Cocktail
{
    /// <summary>
    /// Encapsulates an optionally asynchronous operation that can be executed sequentially or in parallel
    /// as part of a larger list of operations. It hides the technical details of coroutines
    /// </summary>
    [Obsolete("Functionality is part of DevForce now. See new Coroutine.Start and Couroutine.StartParallel overloads.")]
    public class CoroutineCommand
    {
        private readonly Func<INotifyCompleted> _command;

        /// <summary>Initializes a new command.</summary>
        public CoroutineCommand(Func<INotifyCompleted> command)
        {
            _command = command;
        }

        /// <summary>Executes the encapsulated action.</summary>
        public INotifyCompleted Execute()
        {
            return _command();
        }
    }

    /// <summary>
    /// Provides extension methods to execute a collection of CoroutineCommands
    /// </summary>
    [Obsolete("Functionality is part of DevForce now. See new Coroutine.Start and Couroutine.StartParallel overloads.")]
    public static class CommandFns
    {
        private static IEnumerable<INotifyCompleted> CoroutineAction(this IEnumerable<CoroutineCommand> commands)
        {
            IEnumerator<CoroutineCommand> enumerator = commands.GetEnumerator();

            while (enumerator.MoveNext())
                yield return enumerator.Current.Execute();
        }

        /// <summary>Executes a list of CoroutineCommands sequentially. Waiting for each command to finish before executing the next command.</summary>
        [Obsolete("Functionality is part of DevForce now. See new Coroutine.Start and Couroutine.StartParallel overloads.")]
        public static CoroutineOperation Execute(this IEnumerable<CoroutineCommand> commands)
        {
            return Coroutine.Start(commands.CoroutineAction);
        }

        /// <summary>Executes a list of CoroutineCommands in parallel.</summary>
        [Obsolete("Functionality is part of DevForce now. See new Coroutine.Start and Couroutine.StartParallel overloads.")]
        public static CoroutineOperation ExecuteParallel(this IEnumerable<CoroutineCommand> commands)
        {
            return Coroutine.StartParallel(commands.CoroutineAction);
        }
    }
}