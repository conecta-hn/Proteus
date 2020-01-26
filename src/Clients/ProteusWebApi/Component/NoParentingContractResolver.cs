/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    public class NoParentingContractResolver : DefaultContractResolver
    {
        static readonly string[] _ignore = { "parent", "isnew", "idtype","idasobject", "stringid", "isdeleted" , "idtype" };

        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (_ignore.Contains(property.PropertyName.ToLower()))
                property.Ignored = true;

            return property;
        }
    }
}