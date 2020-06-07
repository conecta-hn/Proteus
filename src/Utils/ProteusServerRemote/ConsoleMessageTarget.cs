/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Reflection;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    public class ConsoleMessageTarget
    {
        private static readonly ConsoleColor _default = ConsoleColor.Gray;
        public void Critical(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"(X) {message}");
            Console.ForegroundColor = _default;
        }

        public void Critical(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"(X) {ex.GetType().Name}");
            Console.WriteLine(ex.Message);
            Console.WriteLine(new string('-', Console.BufferWidth));
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine(new string('=', Console.BufferWidth));
            Console.WriteLine();

            if (!(ex.InnerException is null)) Critical(ex.InnerException);
            switch (ex)
            {
                case AggregateException aex:
                    foreach (var j in aex.InnerExceptions) Critical(j);
                    break;
                case ReflectionTypeLoadException rex:
                    foreach (var j in rex.LoaderExceptions?.NotNull() ?? Array.Empty<Exception>()) Critical(j);
                    break;
            }
            Console.ForegroundColor = _default;
        }

        public void Done()
        {
            Console.ForegroundColor = _default;
        }

        public void Done(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(text);
            Console.ForegroundColor = _default;

        }

        public void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("(X) ");
            Console.ForegroundColor = _default;
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("(i) ");
            Console.ForegroundColor = _default;
            Console.WriteLine(message);
        }

        public void Show(string message)
        {
            Console.ForegroundColor = _default;
            Console.WriteLine(message);
        }

        public void Show(string title, string message)
        {
            Console.ForegroundColor = _default;
            Console.WriteLine($"{title}: {message}");
        }

        public void Stop(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write("[!] ");
            Console.ForegroundColor = _default;
            Console.WriteLine(message);
        }

        public void UpdateStatus(double progress)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{progress:0.#}%");
            Console.ForegroundColor = _default;
        }

        public void UpdateStatus(double progress, string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{text}... {progress:0.#}%");
            Console.ForegroundColor = _default;
        }

        public void UpdateStatus(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{text}...");
            Console.ForegroundColor = _default;
        }

        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("/!\\ ");
            Console.ForegroundColor = _default;
            Console.WriteLine(message);
        }
    }
}