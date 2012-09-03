using System;
using System.Composition;
using System.Threading.Tasks;

namespace NavSample
{
    [Export]
    [Shared]
    public class ErrorHandler
    {
         public void Handle(Exception exception)
         {
             if (exception is TaskCanceledException)
                 return;        // Just move on

             // Crash the app for now
             throw exception;
         }
    }
}