/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Protocols
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
    public sealed class CommandAttribute : Attribute, IValueAttribute<Command>
    {
        public Command Value { get; }
        public CommandAttribute(Command value) => Value = value;
    }
}