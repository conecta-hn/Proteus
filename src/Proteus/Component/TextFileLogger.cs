/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Misc;

namespace TheXDS.Proteus.Component
{
    public class TextFileLogger : IMessageTarget, ILogTarget, IStatusReporter
    {
        private static readonly object _lockObj = new object();
        private bool _broken = false;

        public TextFileLogger() : this(null)
        {
        }

        public TextFileLogger(string? logFile)
        {
            LogFile = logFile ?? $@"{Proteus.Settings?.PluginsDir ?? "."}\{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
        }

        public string LogFile { get; }

        public bool IsBusy { get; private set; }

        public void Log(string text)
        {
            if (_broken) return;
            lock (_lockObj)
            {
                try
                {
                    System.IO.File.AppendAllText(LogFile, $"{DateTime.Now}: {text}\n");
                }
                catch (Exception ex)
                {
                    Proteus.AlertTarget?.Alert($"{GetType().NameOf()}: La ruta '{LogFile}' no se pudo escribir: {ex.Message}", a =>
                    {
                        _broken = false;
                        a.Dismiss();
                    });
                    _broken = true;
                }
            }
        }

        public void Critical(string message)
        {
            Log($"(X) {message}");
        }
        public void Critical(Exception ex)
        {
            Log($"(X) {ex.Dump()}");
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
        public void Done()
        {
            Log("Operación completada exitosamente.");
            IsBusy = false;
        }

        public void Done(string text)
        {
            Log($"Operación finalizada: {text}");
            IsBusy = false;
        }

        public void UpdateStatus(double progress)
        {
            IsBusy = true;
            Log($"Se está realizando una operación interna ({progress:0.0}%)");
        }

        public void UpdateStatus(double progress, string text)
        {
            IsBusy = true;
            Log($"{text} ({progress:0.0}%)");
        }

        public void UpdateStatus(string text)
        {
            IsBusy = true;
            Log($"{text}");
        }
    }
}