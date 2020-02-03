/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    public class TitledText<T> : ModelBase<T>, ITitledText where T : IComparable<T>
    {
        public string Header { get; set; }
        public string Body { get; set; }
    }
}
