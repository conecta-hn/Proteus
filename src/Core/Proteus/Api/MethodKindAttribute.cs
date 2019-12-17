using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodKindAttribute : Attribute, IValueAttribute<SecurityFlags>
    {
        public SecurityFlags Value { get; }
        public MethodKindAttribute(SecurityFlags kind)
        {
            Value = kind;
        }
    }
}
