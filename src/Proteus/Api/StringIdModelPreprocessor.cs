/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Establece un campo llave de <see cref="string"/> para una nueva
    /// entidad que no tenga establecido un Id.
    /// </summary>
    public class StringIdModelPreprocessor : AutoIdModelPreprocessor<string>
    {
        /// <summary>
        /// Obtiene un nuevo <see cref="string"/> para utilizarlo como Id
        /// de la nueva entidad.
        /// </summary>
        /// <returns>
        /// Un valor para utilizar como Id de la nueva entidad.
        /// </returns>
        protected override string GetNewValue()
        {
            return Guid.NewGuid().ToString();
        }
    }
}