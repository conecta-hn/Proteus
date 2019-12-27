/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Annotations
{
    /// <summary>
    ///     Marca un método de elemento de interacción como escencial, lo cual
    ///     lo mostrará en la lista de acceso rápido del módulo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EssentialAttribute : Attribute { }
}