/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Crud.Base
{
    public interface IPropertyDateDescription : IPropertyNumberDescription<DateTime>
    {
        bool WithTime { get; }
    }
}