using System.Collections.Generic;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    ///     Define una serie de métodos a implementar por una clase que hospede
    ///     páginas del sistema Proteus.
    /// </summary>
    public interface IPageHost
    {
        /// <summary>
        ///     Colección de páginas hospedadas en este 
        ///     <see cref="IPageHost"/>.
        /// </summary>
        ICollection<IPage> Pages { get; }
        /// <summary>
        ///     Cierra una página de Proteus.
        /// </summary>
        /// <param name="page"></param>
        void ClosePage(IPage page);
        /// <summary>
        ///     Abre y hospeda la página de Proteus en este
        ///     <see cref="IPageHost"/>.
        /// </summary>
        /// <param name="page">Página a hospedar.</param>
        void OpenPage(IPage page);
        /// <summary>
        ///     Abre y hospeda una nueva página de Proteus en este
        ///     <see cref="IPageHost"/>.
        /// </summary>
        /// <typeparam name="TPage">Tipo de página a hospedar.</typeparam>
        void OpenPage<TPage>() where TPage : IPage, new();
        /// <summary>
        ///     Activa la página de Proteus contenida en este
        ///     <see cref="IPageHost"/>.
        /// </summary>
        /// <param name="page">Página a activar.</param>
        void SwitchTo(IPage page);
        /// <summary>
        ///     Avtica una página del tipo especificado contenida dentro de
        ///     este <see cref="IPageHost"/>.
        /// </summary>
        /// <typeparam name="TPage">Tipo de página a activar.</typeparam>
        void SwitchTo<TPage>() where TPage : IPage;
    }
}