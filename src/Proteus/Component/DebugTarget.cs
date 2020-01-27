/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

#if DEBUG

using System;
using System.Diagnostics;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    public class DebugTarget : IMessageTarget
    {
        public void Critical(string message)
        {
            Debug.Print($"(X) Error crítico: {message}");
        }

        public void Critical(Exception ex)
        {
            Debug.Print($"(X) {ex.GetType().NameOf()}: {ex.Message}");
        }

        public void Error(string message)
        {
            Debug.Print($"(X) Error: {message}");
        }

        public void Info(string message)
        {
            Debug.Print($"(i) Información: {message}");
        }

        public void Show(string message)
        {
            Debug.Print(message);
        }

        public void Show(string title, string message)
        {
            Debug.Print($"{title}: {message}");
        }

        public void Stop(string message)
        {
            Debug.Print($"(!) {message}. Alto.");
        }

        public void Warning(string message)
        {
            Debug.Print($"/!\\ Advertencia: {message}");
        }
    }
}

#endif