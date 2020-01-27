/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;

namespace TheXDS.Proteus.Component
{
    public class LogcatMessageTarget : IMessageTarget
    {
        public struct LogcatEntry
        {
            public enum EntryType:byte
            {
                Message,
                Info,
                Warning,
                Stop,
                Error,
                Critical
            }
            public string Kind { get; }
            public string Content { get; }
            public DateTime Timestamp { get; }
            public LogcatEntry(EntryType kind, string content)
            {
                Kind = kind.ToString();
                Content = content;
                Timestamp = DateTime.Now;
            }
        }

        private List<LogcatEntry> _entries = new List<LogcatEntry>();

        public IEnumerable<LogcatEntry> Entries => _entries.AsReadOnly();

        public void Critical(string message)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Critical, message));
        }

        public void Critical(Exception ex)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Critical, ex.Message));
        }

        public void Error(string message)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Error, message));
        }

        public void Info(string message)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Info, message));
        }

        public void Show(string message)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Message, message));
        }

        public void Show(string title, string message)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Message, $"{title}: {message}"));
        }

        public void Stop(string message)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Stop, message));
        }

        public void Warning(string message)
        {
            _entries.Add(new LogcatEntry(LogcatEntry.EntryType.Warning, message));
        }
    }
}
