/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    ///     Indica el orígen de la propiedad mapeada.
    /// </summary>
    public enum PropertyLocation : byte
    {
        /// <summary>
        ///     El campo de orígen de datos es un modelo.
        /// </summary>
        Model,
        /// <summary>
        ///     El campo de orígen de datos es el ViewModel.
        /// </summary>
        ViewModel
    }
}