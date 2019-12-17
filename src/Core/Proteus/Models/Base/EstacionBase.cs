/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    ///     Clase base para los modelos de datos que representan a un equipo
    ///     dentro del sistema.
    /// </summary>
    public abstract class EstacionBase : Nameable<string>
    {
        /// <summary>
        ///     Convierte este objeto a su representación como una cadena.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name} ({Id})";
    }
}
