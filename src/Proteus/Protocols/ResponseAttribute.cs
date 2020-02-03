/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Protocols
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ResponseAttribute : Attribute, IValueAttribute<Response>
    {
        public Response Value { get; }
        public ResponseAttribute(Response value) => Value = value;
    }
}