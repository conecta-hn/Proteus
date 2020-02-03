/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que pueda
    /// describir a una entidad que puede ser marcada como borrada sin
    /// eliminar la información físicamente de la base de datos.
    /// </summary>
    public interface ISoftDeletable
    {
        /// <summary>
        /// Obtiene o establece un valor que indica si la entidad ha sido
        /// marcada como eliminada.
        /// </summary>
        bool IsDeleted { get; set; }
    }
}