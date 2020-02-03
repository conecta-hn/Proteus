/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    public class Person : Contact, ITimestamp
    {
        public string Address { get; set; }

        public DateTime Birth { get; set; } = DateTime.Today - TimeSpan.FromDays(365.25 * 18);

        public DateTime Timestamp { get; set; } = DateTime.Now;

    }
    public abstract class Person<T> : Contact<T>, ITimestamp where T : IComparable<T>
    {
        public string Address { get; set; }

        public DateTime Birth { get; set; } = DateTime.Today - TimeSpan.FromDays(365.25 * 18);

        public DateTime Timestamp { get; set; } = DateTime.Now;

    }
}
