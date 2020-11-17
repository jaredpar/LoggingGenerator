// © Microsoft Corporation. All rights reserved.

// This is an example showing how we can arrange to have strongly typed logging APIs.
//
// The point of this exercise is to create a logging model which:
//
//     * Is delightful for service developers
//     * Eliminates string formatting
//     * Eliminates memory allocations
//     * Enables output in a dense binary format
//     * Enables more effective auditing of log data
//
// Use is pretty simple. A service developer creates an interface type which lists all of the log messages that the assembly can produce.
// Once this is done, a new type is generated automatically which the developer uses to interactively with an ILogger instance. 
//
// This R9LoggingGenerator project uses C# 9.0 source generators. This is magic voodoo invoked at compile time. This code is
// responsible for finding types annotated with the [LoggerExtensions] attribute and automatically generating the strongly-typed
// logging methods.
//
// IMPLEMENTATION TODO
//    * Ensure that the user's logging interface type isn't generic
//    * Ensure that logging methods aren't generic
//    * Ensure that logging methods return void
//    * Finish implementation of exception support
//    * Transpose doc comments from source interface to the generated type to improve IntelliSense experience.
//    * Enforce that two logging methods don't have the same event id
//    * Change namespaces and assembly names for inclusion in .NET
//    * Add nuget packaging voodoo

namespace LoggingExample
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Debug;
    using R9Logging;

    // interface defined by each assembly listing all the logger messages the assembly produces
    [LoggerExtensions]
    interface ILoggerExtensions
    {
        [LoggerMessage(0, LogLevel.Critical, "Could not open socket to `{hostName}`")]
        void CouldNotOpenSocket(string hostName);
    }

    /* Here is an example of the code generated from the above

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    namespace LoggingExample
    {
        static class LoggerExtensions
        {
            private readonly struct __CouldNotOpenSocketStruct__ : IReadOnlyList<KeyValuePair<string, object>>
            {
                private readonly string hostName;

                public __CouldNotOpenSocketStruct__(string hostName)
                {
                    this.hostName = hostName;
                }

                public string Format() => $"Could not open socket to `{hostName}`";

                public int Count => 1;

                public KeyValuePair<string, object> this[int index]
                {
                    get
                    {
                        switch (index)
                        {
                            case 0:
                                return new KeyValuePair<string, object>(nameof(hostName), hostName);

                            default:
                                throw new ArgumentOutOfRangeException(nameof(index));
                        }
                    }
                }

                public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
                {
                    yield return new KeyValuePair<string, object>(nameof(hostName), hostName);
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            private static readonly EventId __CouldNotOpenSocketEventId__ = new(0, nameof(CouldNotOpenSocket));

            public static void CouldNotOpenSocket(this ILogger logger, string hostName)
            {
                if (logger.IsEnabled((LogLevel)5))
                {
                    var s = new __CouldNotOpenSocketStruct__(hostName);
                    logger.Log((LogLevel)5, __CouldNotOpenSocketEventId__, s, null, (s, _) => s.Format());
                }
            }

            public static ILoggerExtensions Wrap(this ILogger logger) => new __Wrapper__(logger);
        
            private sealed class __Wrapper__ : ILoggerExtensions
            {
                private readonly ILogger __logger;
                public __Wrapper__(ILogger logger) => __logger = logger;
                public void CouldNotOpenSocket(string hostName) =>  __logger.CouldNotOpenSocket(hostName);
            }
        }
    }

*/

    class Program
    {
        static void Main()
        {
            using var provider = new DebugLoggerProvider();
            var logger = provider.CreateLogger("LoggingExample");

            // Approach #1: Extension method
            logger.CouldNotOpenSocket("microsoft.com");

            // Approach #2: wrapper Type
            var d = logger.Wrap();
            d.CouldNotOpenSocket("microsoft.com");
        }
    }
}
