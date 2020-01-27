using TheXDS.Proteus.Widgets;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// Clase base para todos los ViewModel que puedan hospedar páginas
    /// visuales.
    /// </summary>
    public abstract class PageHostViewModel : PageViewModel, IPageHost
    {
        private readonly IPageVisualHost _visualHost;
        private readonly ObservableCollection<IPage> _pages = new ObservableCollection<IPage>();
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        /// <param name="host">Objeto Host visual de este ViewModel.</param>
        protected PageHostViewModel(IPageVisualHost host) : this(host, true)
        {
        }
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        /// <param name="host">Objeto Host visual de este ViewModel.</param>
        /// <param name="closeable">
        /// Estado predeterminado de cierre de este ViewModel.
        /// </param>
        protected PageHostViewModel(IPageVisualHost host, bool closeable) : base(host, closeable)
        {
            _visualHost = host;
            _pages.CollectionChanged += Pages_CollectionChanged;
        }

        private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Notify(nameof(Pages));
        }

        /// <summary>
        /// Enumera a las páginas abiertas hospedadas en este
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        public ICollection<IPage> Pages => _pages;
        /// <summary>
        /// Abre y hospeda una página en este
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        /// <param name="page">Página a hospedar.</param>
        public void OpenPage(IPage page)
        {
            page.PageHost = this;
            _pages.Add(page);
            SwitchTo(page);
        }
        /// <summary>
        /// Instancia, abre y hospeda una página en este
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        /// <typeparam name="TPage">Tipo de página a hospedar.</typeparam>
        public void OpenPage<TPage>() where TPage : IPage, new()
        {
            OpenPage(new TPage());
        }
        /// <summary>
        /// Cierra una página hospedada en este
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        /// <param name="page">Página a cerrar.</param>
        public void ClosePage(IPage page)
        {
            _pages.Remove(page);
        }
        /// <summary>
        /// Activa una página hospedada en este
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        /// <param name="page">Página a activar.</param>
        public void SwitchTo(IPage page)
        {
            _visualHost?.Activate(page);
        }
        /// <summary>
        /// Activa una página hospedada en este
        /// <see cref="PageHostViewModel"/>.
        /// </summary>
        /// <typeparam name="TPage">Tipo de página a activar.</typeparam>
        public void SwitchTo<TPage>() where TPage : IPage
        {
            SwitchTo(Pages.OfType<TPage>().FirstOrDefault());
        }
    }
}