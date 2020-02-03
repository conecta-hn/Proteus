/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Widgets
{
    /// <summary>
    /// Define una serie de métodos a implementar por una clase que sea el
    /// host visual de una página del sistema Proteus.
    /// </summary>
    public interface IPageVisualHost : IPage
    {
        /// <summary>
        /// Activa la página solicitada.
        /// </summary>
        /// <param name="page">
        /// Página a activar.
        /// </param>
        void Activate(IPage page);
    }

    /// <summary>
    /// Define una serie de métodos a implementar por una clase con
    /// funcionalidad de cierre que sea el host visual de páginas del
    /// sistema Proteus.
    /// </summary>
    public interface IPageRootVisualHost : IPageVisualHost
    {
        /// <summary>
        /// Cierra forzosamente la aplicación.
        /// </summary>
        void ForceClose();
    }
}