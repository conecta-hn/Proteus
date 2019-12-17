/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;

namespace TheXDS.Proteus.Models
{
    public class WebSession : TimestampModel<Guid>, IVoidable
    {
        public string Username { get; set; }
        public DateTime? Void { get; set; }
    }
}
