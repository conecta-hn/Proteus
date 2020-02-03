/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.Widgets
{
    /// <summary>
    /// Define una serie de miembros a implementar por una clase que 
    /// funcione como una página huésped de Proteus.
    /// </summary>
    public interface IPage : ICloseable
    {
        /// <summary>
        /// Obtiene o establece una referencia al hospedaje de páginas de
        /// Proteus.
        /// </summary>
        IPageHost PageHost { get; set; }
        /// <summary>
        /// Activa esta página de Proteus.
        /// </summary>
        void Activate();
    }    
}