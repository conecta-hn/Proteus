/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que
    /// represente un descriptor de seguridad.
    /// </summary>
    public interface ISecurityDescriptor
    {
        /// <summary>
        /// Obtiene una referencia a la credencial para la cual este
        /// descriptor ha sido aplicado.
        /// </summary>
        IProteusCredential Parent { get; }
    }
}