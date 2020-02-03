/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Component.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Indica que una propiedad no se cableará automáticamente por el método
    /// <see cref="M:TheXDS.Proteus.Models.Base.ModelBase.AutoHook" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NoAutoHookAttribute : Attribute { }
}