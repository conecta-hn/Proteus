/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe una dirección de correo.
    /// </summary>
    public class Email : ModelBase<long>
    {
        /// <summary>
        /// Obtiene o establece la dirección de correo de esta instancia.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="Email"/>.
        /// </summary>
        public Email()
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="Email"/>.
        /// </summary>
        /// <param name="address">
        /// Dirección de correo a establecer.
        /// </param>
        public Email(string address)
        {
            Address = address;
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="string"/> en un
        /// <see cref="Email"/>.
        /// </summary>
        /// <param name="address">
        /// Cadena a convertir.
        /// </param>
        public static implicit operator Email(string address) => new Email(address);

        /// <summary>
        /// Convierte implícitamente un <see cref="Email"/> en un
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="email">
        /// <see cref="Email"/> desde el cual obtener la dirección de
        /// correo electrónico.
        /// </param>
        public static implicit operator string(Email email) => email.Address;

        /// <summary>
        /// Obtiene el nombre de dominio asociado a esta dirección de
        /// correo.
        /// </summary>
        public string Domain => Part(1);

        /// <summary>
        /// Obtiene el nombre de usuario asociado a esta dirección de
        /// correo.
        /// </summary>
        public string UserName => Part(0);

        private string Part(int index)
        {
            if (Address.IsEmpty()) return string.Empty;
            var a = Address.Split('@');
            return a.Length != 2 ? string.Empty : a[index];
        }

        public override string ToString()
        {
            return Address;
        }
    }
}