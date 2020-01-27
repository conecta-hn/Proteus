/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class LoginToken : ModelBase<string>, IVoidable
    {
        public virtual User Login { get; set; }
        public DateTime? Void { get; set; }
        public string ComputerName { get; set; }
        public bool OnceOnly { get; set; }
    }
}