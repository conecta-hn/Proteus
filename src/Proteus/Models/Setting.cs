/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;

namespace TheXDS.Proteus.Models
{
    public class Setting : ModelBase<string>
    {
        public string Value { get; set; }
        public virtual ConfigRepository Parent { get; set; }

        public static implicit operator KeyValuePair<string, string>(Setting setting) => new KeyValuePair<string, string>(setting.Id, setting.Value);
        public static implicit operator Setting(KeyValuePair<string, string> value) => new Setting { Id = value.Key, Value = value.Value};
    }
}