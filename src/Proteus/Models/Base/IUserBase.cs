/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que exponga 
    /// información de enlace de datos con el módulo de usuario, haciendo
    /// referencia al modelo <see cref="User"/> o a una clase derivada.
    /// </summary>
    public interface IUserBase
    {
        /// <summary>
        /// Obtiene o establece el Id del usuario enlazado referenciado por
        /// este objeto.
        /// </summary>
        string UserId { get; set; }
    }
}