/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// Clase base para un <see cref="PageViewModel"/> con capacidad de
    /// administrar a una entidad de datos.
    /// </summary>
    /// <typeparam name="T">Modelo de la entidad a administrar.</typeparam>
    public abstract class PageModelViewModel<T> : PageViewModel where T : ModelBase
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageModelViewModel{T}"/>.
        /// </summary>
        /// <param name="host">Host visual de esta página.</param>
        protected PageModelViewModel(ICloseable host) : base(host)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageModelViewModel{T}"/>.
        /// </summary>
        /// <param name="host">Host visual de esta página.</param>
        /// <param name="closeable">
        /// Establece la posibilidad de cerrar la página.
        /// </param>
        protected PageModelViewModel(ICloseable host, bool closeable) : base(host, closeable)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageModelViewModel{T}"/>.
        /// </summary>
        /// <param name="host">Host visual de esta página.</param>
        /// <param name="entity">Entidad a administrar.</param>
        protected PageModelViewModel(ICloseable host, T? entity) : base(host)
        {
            Entity = entity;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PageModelViewModel{T}"/>.
        /// </summary>
        /// <param name="host">Host visual de esta página.</param>
        /// <param name="closeable">
        /// Establece la posibilidad de cerrar la página.
        /// </param>
        /// <param name="entity">Entidad a administrar.</param>
        protected PageModelViewModel(ICloseable host, bool closeable, T? entity) : base(host, closeable)
        {
            Entity = entity;
        }

        private T? _entity;

        /// <summary>
        /// Obtiene o establece el valor Entity.
        /// </summary>
        /// <value>El valor de Entity.</value>
        public T? Entity
        {
            get => _entity;
            set => Change(ref _entity, value);
        }
    }
}