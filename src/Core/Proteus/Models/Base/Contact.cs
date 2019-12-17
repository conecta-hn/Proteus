/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;

namespace TheXDS.Proteus.Models.Base
{
    public class Contact : Contact<Guid>
    {
    }

    public abstract class Contact<T> : Nameable<T>, IContact where T : IComparable<T>
    {
        public virtual List<Email> Emails { get; set; } = new List<Email>();
        public virtual List<Phone> Phones { get; set; } = new List<Phone>();
    }
}
