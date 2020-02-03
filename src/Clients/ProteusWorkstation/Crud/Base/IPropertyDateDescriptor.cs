/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Crud.Base
{
    public interface IPropertyDateDescriptor : IPropertyNumberDescriptor<DateTime>
    {
        /// <summary>
        /// Indica que el campo de fecha incluirá el componente de hora.
        /// </summary>
        /// <returns></returns>
        IPropertyDateDescriptor WithTime();

    }
}