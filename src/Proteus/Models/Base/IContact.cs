/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que exponga
    /// información de contacto.
    /// </summary>
    public interface IContact
    {
        /// <summary>
        /// Colección de direcciones de correo del contacto.
        /// </summary>
        List<Email> Emails { get; set; }

        /// <summary>
        /// Colección de números de teléfono del contacto.
        /// </summary>
        List<Phone> Phones { get; set; }
    }
}