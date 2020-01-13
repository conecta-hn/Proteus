/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Define una serie de miembros a implementar por un objeto que pueda
    /// volcar algún tipo de información sobre sí mismo.
    /// </summary>
    public interface IDumpInfo
    {
        /// <summary>
        /// Inicia el volcado de información de esta instancia.
        /// </summary>
        void Dump();
    }
}