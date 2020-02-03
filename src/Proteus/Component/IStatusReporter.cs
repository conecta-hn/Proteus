/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Define una serie de métodos a implementar por una ventana con una barra de estado y progreso.
    /// </summary>
    public interface IStatusReporter
    {
        /// <summary>
        /// Cambia la barra al estado de "Listo".
        /// </summary>
        void Done();
        /// <summary>
        /// Cambia la barra al estado de listo, mostrando un mensaje.
        /// </summary>
        /// <param name="text">Mensaje a mostrar.</param>
        void Done(string text);
        /// <summary>
        /// Actualiza el estado de la barra de progreso.
        /// </summary>
        /// <param name="progress">Valor de progreso.</param>
        void UpdateStatus(double progress);
        /// <summary>
        /// Actualiza el estado de la barra de progreso.
        /// </summary>
        /// <param name="progress">Valor de progreso.</param>
        /// <param name="text">Mensaje de estado.</param>
        void UpdateStatus(double progress, string text);
        /// <summary>
        /// </summary>
        /// <param name="text">Mensaje de estado.</param>
        void UpdateStatus(string text);
    }
}