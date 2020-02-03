/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.ComponentModel.DataAnnotations;

namespace TheXDS.Proteus.Crud.Base
{

    /// <summary>
    /// Enumera los distintos modos de nulidad de los campos.
    /// </summary>
    public enum NullMode
    {
        /// <summary>
        /// Inferir, basado en la prescencia del atributo
        /// <see cref="RequiredAttribute"/> del campo a configurar.
        /// </summary>
        Infer,
        /// <summary>
        /// Campo requerido.
        /// </summary>
        Required,
        /// <summary>
        /// Campo opcional desactivable.
        /// </summary>
        Nullable,
        /// <summary>
        /// Campo opcional seleccionable con RadioButton.
        /// </summary>
        Radio
    }
}