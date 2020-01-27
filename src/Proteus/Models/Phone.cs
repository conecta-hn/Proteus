/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Attributes;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class Phone : ModelBase<long>
    {
        /// <summary>
        /// Determina el tipo de teléfono.
        /// </summary>
        public enum PhoneTypes
        {
            Fijo,
            Internacional,
            Fax,
            [Name("Móvil")] Movil,
            PBX,
            Skype,
            VoIP,
            [Name("Extensión")] Extension
        }

        public PhoneTypes PhoneType { get; set; } 
        public string Number { get; set; }

        public Phone()
        {
        }

        public Phone(string phone):this(PhoneTypes.Fijo, phone)
        {
        }

        public Phone(PhoneTypes type, string phone)
        {
            PhoneType = type;
            Number = phone;
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="string"/> en un
        /// <see cref="Email"/>.
        /// </summary>
        /// <param name="number">
        /// Cadena a convertir.
        /// </param>
        public static implicit operator Phone(string number) => new Phone(number);

        /// <summary>
        /// Convierte implícitamente un <see cref="Email"/> en un
        /// <see cref="string"/>.
        /// </summary>
        /// <param name="email">
        /// <see cref="Email"/> desde el cual obtener la dirección de
        /// correo electrónico.
        /// </param>
        public static implicit operator string(Phone email) => email.Number;

        public override string ToString()
        {
            return Number;
        }

    }
}