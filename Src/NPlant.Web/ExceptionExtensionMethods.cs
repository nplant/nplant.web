using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

/// <summary>
/// Utility method for dealing with exceptions 
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1050:DeclareTypesInNamespaces", Justification = "Easier to use if global")]
public static class ExceptionExtensionMethods
{
    /// <summary>
    /// Utility method for inspectingthe exception graph to determine if there are any falling knives.
    /// http://vasters.com/clemensv/2012/09/06/Are+You+Catching+Falling+Knives.aspx
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static bool IsFatal(this Exception exception)
    {
        while (exception != null)
        {
            if (exception as OutOfMemoryException != null && exception as InsufficientMemoryException == null || exception as ThreadAbortException != null ||
                exception as AccessViolationException != null || exception as SEHException != null || exception as StackOverflowException != null)
            {
                return true;
            }

            if (exception as TypeInitializationException == null && exception as TargetInvocationException == null)
            {
                break;
            }

            exception = exception.InnerException;
        }

        return false;
    }
}