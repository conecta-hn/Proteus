/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.Reflection;

namespace TheXDS.Proteus.Crud.Base
{
    internal class ListPropertyDescriptor<T> : ListBasePropertyDescriptor<IListPropertyDescriptor<T>,T>, IListPropertyDescription, IListPropertyDescriptor<T> where T : ModelBase
    {
        public ListPropertyDescriptor(PropertyInfo property) : base(property)
        {
            Required();
        }

        public bool Editable { get; private set; }

        IListPropertyDescriptor<T> IListPropertyDescriptor<T>.Editable()
        {
            Editable = true;
            return this;
        }
    }
}