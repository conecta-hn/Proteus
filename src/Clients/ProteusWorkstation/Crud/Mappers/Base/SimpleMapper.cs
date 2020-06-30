/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud.Base;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Crud.Mappers.Base
{
    public abstract class SimpleMapper<T> : PropertyMapper
    {
        public override IEnumerable<Type> MapsTypes
        {
            get {
                yield return typeof(T);
                if (typeof(T).IsValueType) yield return typeof(Nullable<>).MakeGenericType(typeof(T));
            }
        }
    }
    public abstract class SimpleMapper<T, TMapping> : SimpleMapper<T> where TMapping : IPropertyMapping
    {
        public override IPropertyMapping Map(IEntityViewModel parentVm, IPropertyDescription p)
        {
            return typeof(TMapping).New<IPropertyMapping>(p);
        }
    }
}