/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    ///     Clase base para modelos que describan un rol operativo de sistema
    ///     dentro de un módulo.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de campo llave a utilizar para el modelo subyacente.
    /// </typeparam>
    public abstract class User<T> : ModelBase<T>, IUserBase where T : IComparable<T>
    {
        /// <summary>
        ///     Obtiene o establece el Id del usuario enlazado.
        /// </summary>
        public string UserId { get; set; }

        public override string ToString()
        {
            return Proteus.LogonService.Get<User>(UserId)?.Name;
        }
    }
}