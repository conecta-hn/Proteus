/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Reflection;

namespace TheXDS.Proteus.Component
{
    public class ConsoleMessageTarget : IMessageTarget, IStatusReporter
    {
        private static readonly object _lockObj = new object();

        private const ConsoleColor _default = ConsoleColor.Gray;
        public void Critical(string message)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"(X) {message}");
                Console.ForegroundColor = _default;
            }
        }

        public void Critical(Exception ex)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"(X) {ex.GetType().Name}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(new string('-', Console.BufferWidth));
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(new string('=', Console.BufferWidth));
                Console.WriteLine();
                Console.ForegroundColor = _default;
            }

            if (!(ex.InnerException is null)) Critical(ex.InnerException);
            switch (ex)
            {
                case AggregateException aex:
                    foreach (var j in aex.InnerExceptions) Critical(j);
                    break;
                case ReflectionTypeLoadException rex:
                    foreach (var j in rex.LoaderExceptions) Critical(j);
                    break;
            }
        }

        public void Done()
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = _default;
            }
        }

        public void Done(string text)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine(text);
                Console.ForegroundColor = _default;
            }
        }

        public void Error(string message)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("(X) ");
                Console.ForegroundColor = _default;
                Console.WriteLine(message);
            }
        }

        public void Info(string message)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("(i) ");
                Console.ForegroundColor = _default;
                Console.WriteLine(message);
            }
        }

        public void Show(string message)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = _default;
                Console.WriteLine(message);
            }
        }

        public void Show(string title, string message)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = _default;
                Console.WriteLine($"{title}: {message}");
            }
        }

        public void Stop(string message)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("[!] ");
                Console.ForegroundColor = _default;
                Console.WriteLine(message);
            }
        }

        public void UpdateStatus(double progress)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{progress:0.#}%");
                Console.ForegroundColor = _default;
            }
        }

        public void UpdateStatus(double progress, string text)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{text}... {progress:0.#}%");
                Console.ForegroundColor = _default;
            }
        }

        public void UpdateStatus(string text)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{text}...");
                Console.ForegroundColor = _default;
            }
        }

        public void Warning(string message)
        {
            lock (_lockObj)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("/!\\ ");
                Console.ForegroundColor = _default;
                Console.WriteLine(message);
            }
        }
    }
}