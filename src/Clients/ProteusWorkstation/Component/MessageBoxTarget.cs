/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Windows;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    public class MessageBoxTarget : IMessageTarget
    {
        public void Critical(string message)
        {
            MessageBox.Show(message, "Error crítico", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Critical(Exception ex)
        {
            MessageBox.Show(ex.Message, ex.GetType().NameOf(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void Info(string message)
        {
            MessageBox.Show(message, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Show(string message)
        {
            MessageBox.Show(message, "Mensaje");
        }

        public void Show(string title, string message)
        {
            MessageBox.Show(message, title);
        }

        public void Stop(string message)
        {
            MessageBox.Show(message, "Alto", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        public void Warning(string message)
        {
            MessageBox.Show(message, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}