/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

# nullable enable

using System;
using System.Reflection;

namespace TheXDS.Proteus.Component
{
    public class TextFileLogger : IMessageTarget
    {
        private static readonly object _lockObj = new object();
        public TextFileLogger() : this(null)
        {
        }
        public TextFileLogger(string? logFile)
        {
            LogFile = logFile ?? $@"{Proteus.Settings?.PluginsDir ?? "."}\{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
        } 
        private string LogFile { get; }
        public void Log(string text)
        {
            lock (_lockObj)
            {
                System.IO.File.AppendAllText(LogFile, $"{DateTime.Now}: {text}\n");
            }
        }

        public void Critical(string message)
        {
            Log($"(X) {message}");
        }
        public void Critical(Exception ex)
        {
            Log($"(X) {ex.GetType().Name}\n{ex.Message}\n{ex.StackTrace}");
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
        public void Error(string message)
        {
            Log($"(X) {message}");
        }
        public void Info(string message)
        {
            Log($"(i) {message}");
        }
        public void Show(string message)
        {
            Log($"message");
        }
        public void Show(string title, string message)
        {
            Log($"{title}: {message}");
        }
        public void Stop(string message)
        {
            Log($"[!] {message}");
        }
        public void Warning(string message)
        {
            Log($"/!\\ {message}");
        }
    }
}