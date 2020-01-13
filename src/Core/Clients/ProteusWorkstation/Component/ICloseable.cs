/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Define una serie de métodos a implementar por una clase que permita
    /// cerrar un contenedor visual o un elemento de UI.
    /// </summary>
    public interface ICloseable
    {
        /// <summary>
        /// Cierra el elemento activo.
        /// </summary>
        void Close();
    }
}