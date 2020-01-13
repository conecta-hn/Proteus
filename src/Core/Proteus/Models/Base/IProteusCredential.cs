/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using System.Collections.Generic;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que describa
    /// una credencial de seguridad de Proteus.
    /// </summary>
    public interface IProteusCredential : INameable
    {
        /// <summary>
        /// Obtiene el Id de la credencial.
        /// </summary>
        string Id { get; }
        
        /// <summary>
        /// Obtiene un valor que indica el comportamiento de seguridad
        /// predeterminado a utilizar para presentar módulos.
        /// </summary>
        SecurityBehavior? ModuleBehavior { get; }

        /// <summary>
        /// Obtiene un valor que indica el comportamiento de seguridad
        /// predeterminado a utilizar para presentar botones.
        /// </summary>
        SecurityBehavior? ButtonBehavior { get; }

        /// <summary>
        /// Enumera todos los descriptores de seguridad asociados a esta
        /// credencial.
        /// </summary>
        IEnumerable<ISecurityDescriptor> Descriptors { get; }

        /// <summary>
        /// Obtiene un valor que indica las operaciones de seguridad
        /// otorgadas de forma predeterminada a la credencial.
        /// </summary>
        SecurityFlags DefaultGranted { get; }

        /// <summary>
        /// Obtiene un valor que indica las operaciones de seguridad
        /// denegadas de forma predeterminada a la credencial.
        /// </summary>
        SecurityFlags DefaultRevoked { get; }
    }
}