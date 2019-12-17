/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.ViewModels.Base
{

    public abstract class PageModelViewModel<T> : PageViewModel where T : ModelBase
    {
        protected PageModelViewModel(ICloseable host) : base(host)
        {
        }

        protected PageModelViewModel(ICloseable host, bool closeable) : base(host, closeable)
        {
        }

        protected PageModelViewModel(ICloseable host, T entity) : base(host)
        {
            Entity = entity;
        }

        protected PageModelViewModel(ICloseable host, bool closeable, T entity) : base(host, closeable)
        {
            Entity = entity;
        }

        private T _entity;

        /// <summary>
        ///     Obtiene o establece el valor Entity.
        /// </summary>
        /// <value>El valor de Entity.</value>
        public T Entity
        {
            get => _entity;
            set => Change(ref _entity, value);
        }
    }
    /// <summary>
    ///     Clase base para todos los ViewModel que describan ventanas.
    /// </summary>
    public abstract class PageViewModel : ProteusViewModel, IPageViewModel
    {
        private bool _closeable = true;
        private string _title;

        /// <summary>
        ///     Obtiene el título de este ViewModel a mostrar en su
        ///     correspondiente contenedor visual.
        /// </summary>
        public virtual string Title
        {
            get => _title;
            set => Change(ref _title, value);
        }
        /// <summary>
        ///     Obtiene al contenedor visual cerrable de este ViewModel.
        /// </summary>
        public ICloseable Host { get; }
        /// <summary>
        ///     Obtiene un comando de cierre de este ViewModel.
        /// </summary>
        public SimpleCommand CloseCommand { get; }
        /// <summary>
        ///     Obtiene un valor que determina si este ViewModel puede ser
        ///     cerrado.
        /// </summary>
        public virtual bool Closeable
        {
            get => _closeable;
            protected set
            {
                if (Change(ref _closeable, value))
                    CloseCommand.SetCanExecute(value);
            }
        }
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="PageViewModel"/>.
        /// </summary>
        /// <param name="host">Huésped visual de este ViewModel.</param>
        /// <param name="closeable">
        ///     Valor predeterminado de capacidad de cierre de este VideModel.
        /// </param>
        protected PageViewModel(ICloseable host, bool closeable)
        {
            CloseCommand = new SimpleCommand(Close, Closeable);
            Closeable = closeable;
            Host = host;
        }
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="PageViewModel"/>.
        /// </summary>
        /// <param name="host">Huésped visual de este ViewModel.</param>
        protected PageViewModel(ICloseable host) : this(host, true) { }
        /// <summary>
        ///     Cierra este ViewModel manualmente.
        /// </summary>
        public virtual void Close()
        {
            if (Closeable) Host?.Close();
        }
    }
}