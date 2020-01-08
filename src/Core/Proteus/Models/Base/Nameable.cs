/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Models.Base
{
    public abstract class Nameable<T> : ModelBase<T>, INameable where T : IComparable<T>
    {
        /// <summary>
        ///     Obtiene el nombre del elemento.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Convierte este elemento en su representación como cadena.
        /// </summary>
        /// <returns>
        ///     El nombre de este objeto.
        /// </returns>
        public override string ToString() => Name;
    }
}