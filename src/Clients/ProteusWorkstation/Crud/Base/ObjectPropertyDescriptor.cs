/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using TheXDS.MCART.Types.Extensions;
using System.Linq.Expressions;
using static TheXDS.MCART.ReflectionHelpers;
using TheXDS.MCART.Types;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.Crud.Base
{
    internal class ObjectPropertyDescriptor : CrudPropertyDescriptor, IObjectPropertyDescription, IObjectPropertyDescriptor
    {

        private IQueryable<ModelBase> _source;

        private bool _selectable;

        private Func<object, CrudViewModelBase?, ObservableListWrap<ModelBase>>? _vmSource;

        public BindingBase DisplayMemberBinding { get; protected internal set; }

        public string DisplayMemberPath { get; protected internal set; }

        public ObjectPropertyDescriptor(PropertyInfo property) : base(property) { }

        public IQueryable<ModelBase> Source => _source ?? (Selectable ? GetFromSvc() : null);

        public bool Creatable { get; private set; }

        public bool Selectable => _selectable || !Creatable;

        public IEnumerable<Type> ChildModels { get; private set; }

        public bool UseVmSource { get; private set; }

        ObservableListWrap<ModelBase>? IDataPropertyDescription.VmSource(object obj) => _vmSource?.Invoke(obj, null);
        ObservableListWrap<ModelBase>? IDataPropertyDescription.VmSource(object obj, CrudViewModelBase elementVm) => _vmSource?.Invoke(obj, elementVm);

        private protected virtual IQueryable<ModelBase> GetFromSvc()
        {
            var t = Property.PropertyType.ResolveCollectionType();
            return Proteus.InferService(t)?.All(t) 
                ?? Proteus.InferBaseService(t)?.AllBase(t);
        }

        IDataPropertyDescriptor IDataPropertyDescriptor.Creatable()
        {
            ChildModels = new[] { Property.PropertyType };
            Creatable = true;
            return this;
        }
        IDataPropertyDescriptor IDataPropertyDescriptor.Source(IQueryable<ModelBase> source)
        {
            _source = source;
            return this;
        }
        IObjectPropertyDescriptor IObjectPropertyDescriptor.Selectable()
        {
            _selectable = true;
            return this;
        }
        IDataPropertyDescriptor IDataPropertyDescriptor.Creatable(IEnumerable<Type> models)
        {
            var l = models.ToList();

            foreach(var j in l)
            {
                if (!j.Implements<ModelBase>())
                    throw new InvalidOperationException($"El tipo {j.FullName} no es un modelo de datos.");

                if (!j.IsInstantiable())
                    throw new InvalidOperationException($"El tipo {j.FullName} no es instanciable.");

                if (!Property.PropertyType.ResolveCollectionType().IsAssignableFrom(j))
                    throw new InvalidOperationException($"El tipo {j.FullName} no es asignable a {Property.Name}.");
            }

            ChildModels = l;
            Creatable = true;
            return this;
        }

        public IDataPropertyDescription DisplayMember(Expression<Func<ModelBase, object>> selector)
        {
            return Displaymember(selector);
        }

        private protected IDataPropertyDescription Displaymember<T>(Expression<Func<T, object>> selector)
        {
            var p = GetMember(selector) as PropertyInfo
                ?? throw new MemberAccessException("El miembro a seleccionar debe ser una propiedad.");

            if (p.GetMethod.IsStatic && p.GetValue(null) is DependencyProperty dp)
            {
                DisplayMemberBinding = new Binding()
                {
                    Path = new PropertyPath(dp)
                };
                DisplayMemberPath = dp.Name;
            }
            else if (!p.GetMethod.IsStatic)
            {
                DisplayMemberBinding = new Binding(p.Name);
                DisplayMemberPath = p.Name;
            }
            else
            {
                throw new MemberAccessException("la propiedad estática debe devolver una propiedad de dependencia.");
            }
            return this;
        }

        public IDataPropertyDescription DisplayMember(string path)
        {
            DisplayMemberBinding = new Binding(path);
            DisplayMemberPath = path;
            return this;
        }

        public IDataPropertyDescriptor VmSource<T>(Func<T, CrudViewModelBase?, ObservableListWrap<ModelBase>> source) where T : ViewModelBase
        {
            _vmSource = (o,e) => source.Invoke((T)o, e);
            UseVmSource = true;
            return this;
        }

        public IDataPropertyDescriptor VmSource<T>(Func<T, ObservableListWrap<ModelBase>> source) where T : ViewModelBase
        {
            return VmSource<T>((o, _) => source(o));
        }
    }
}