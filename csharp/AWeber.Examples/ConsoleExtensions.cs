using System;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;

namespace AWeber.Examples
{
    public static class ConsoleExtensions
    {
        public static void WriteResponse(this IConsole console, ConsoleColor foreground, string format, params object[] args)
        {
            console.ForegroundColor = foreground;
            console.Out.WriteLine(format, args);
            Console.ResetColor();
        }

        public static void WriteError(this IConsole console, string format, params object[] args)
        {
            console.ForegroundColor = ConsoleColor.Red;
            console.Error.WriteLine(format, args);
            Console.ResetColor();
        }

        public static void WriteJson<T>(this IConsole console, string message, T obj)
        {
            console.WriteResponse(ConsoleColor.Yellow, "{0}: {1}", message, JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}