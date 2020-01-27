/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// Define una serie d emiembros a implementar por un tipo que defina a
    /// un ViewModel de página de Proteus.
    /// </summary>
    public interface IPageViewModel
    {
        /// <summary>
        /// Obtiene un valor que determina si esta página puede ser cerrada.
        /// </summary>
        bool Closeable { get; }

        /// <summary>
        /// Obtiene una referencia al comando de cierre de esta página.
        /// </summary>
        SimpleCommand CloseCommand { get; }

        /// <summary>
        /// Obtiene al Host visual de esta página.
        /// </summary>
        ICloseable Host { get; }

        /// <summary>
        /// Obtiene el título de esta página.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Solicita el cierre de esta página.
        /// </summary>
        void Close();
    }
}