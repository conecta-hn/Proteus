/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TheXDS.Proteus.Crud.Base
{
    internal abstract class ListBasePropertyDescriptor<TDescriptor, TItem> : ObjectPropertyDescriptor, IListBasePropertyDescription, IListBasePropertyDescriptor<TDescriptor,TItem> where TDescriptor : class, IDataPropertyDescriptor where TItem : ModelBase
    {
        private readonly List<Column> _columns = new List<Column>();
        public ListBasePropertyDescriptor(PropertyInfo property) : base(property)
        {
        }

        IEnumerable<Column> IListBasePropertyDescription.Columns => _columns;

        public TDescriptor Column(string header, string path)
        {
            _columns.Add(new Column(header, path));
            return this as TDescriptor;
        }

        public TDescriptor Column(string header, Expression<Func<TItem, object>> property)
        {
            _columns.Add(new Column(header, property.Name));
            return this as TDescriptor;
        }
    }

    internal class LinkPropertyDescriptor<T> : ListBasePropertyDescriptor<ILinkPropertyDescriptor<T>,T>, ILinkPropertyDescriptor<T>, ILinkPropertyDescription where T : ModelBase
    {
        public LinkPropertyDescriptor(PropertyInfo property) : base(property)
        {
            ((IObjectPropertyDescriptor)this).Selectable();
        }

        public Type Model => typeof(T);

        public ILinkPropertyDescriptor<T> DisplayMember(Expression<Func<T, object>> selector)
        {
            Displaymember(selector);
            return this;
        }

        public new ILinkPropertyDescriptor<T> DisplayMember(string path)
        {
            base.DisplayMember(path);
            return this;
        }

        private protected override IQueryable<ModelBase> GetFromSvc()
        {
            return Proteus.Infer(typeof(T)).All(typeof(T));
        }
    }
}