/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq.Expressions;
using TheXDS.MCART.Component;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Cmd
{
    public abstract class ProteusArgument : Argument
    {
        protected void ShowWarning(Exception ex) => ShowWarning($"Se ha producido una excepción de tipo {ex.GetType().NameOf()}{Environment.NewLine}{Environment.NewLine}{ex.Message}");
        protected void ShowWarning(string body) => ShowWarning($"Advertencia: {LongName}", body);

        protected void ShowWarning(string header, string body)
        {
            Show(p => p.Warning, header, body);
        }

        protected void InvalidArg()
        {
            ShowWarning($"El valor '{Value}' provisto para este argumento no es válido.");
        }
        protected void Show(Expression<Func<IMessageTarget, Action<string>>> method, string header, string body) => Show(method, header, body, null);

        protected void Show(Expression<Func<IMessageTarget, Action<string>>> method, string header, string body, Action<Alerta>? interaction)
        {
            if (Proteus.AlertTarget is { } a)
            {
                a.Alert(header, body, interaction!);
            }
            else if (Proteus.MessageTarget is { } m)
            {
                TheXDS.MCART.ReflectionHelpers.GetMethod(method).Invoke(m, new[] { $"{header.OrNull($"{{0}}{Environment.NewLine}{Environment.NewLine}")}{body}" });
            }
        }
    }
}