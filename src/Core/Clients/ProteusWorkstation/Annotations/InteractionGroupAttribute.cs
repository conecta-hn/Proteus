/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Annotations
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InteractionGroupAttribute : TextAttribute
    {
        public InteractionGroupAttribute(string interactionGroup) : base(interactionGroup) {}
    }
}