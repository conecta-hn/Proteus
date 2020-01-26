/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.Proteus.Crud.Mappings.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using TheXDS.MCART.Attributes;
using static TheXDS.MCART.Objects;
using static TheXDS.MCART.ReflectionHelpers;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    /// Clase base que mapea la descripción de una propiedad con un control
    /// de edición apropiado para editar el valor de la misma.
    /// </summary>
    public abstract class PropertyMapper
    {
        private static readonly HashSet<PropertyMapper> _mappers = new HashSet<PropertyMapper>();

        /// <summary>
        /// Elementos de mapeo de propiedades cargados.
        /// </summary>
        public static IEnumerable<PropertyMapper> Mappers => _mappers;

        static PropertyMapper()
        {
            RescanMappers();
        }
        
        /// <summary>
        /// Vuelve a escanear todos los ensamblados en búsqueda de nuevas
        /// clases que hereden de <see cref="PropertyMapping"/>.
        /// </summary>
        public static void RescanMappers()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return;
            lock (_mappers)
            {
                try
                {
                    _mappers.Clear();
                    foreach (var j in FindAllObjects<PropertyMapper>().OrderBy(p=>p.GetAttr<PriorityAttribute>()?.Value ?? int.MaxValue))
                    {
                        _mappers.Add(j);
                    }
                }
                catch { /* Suprimir error. Simplemente no mapear propiedades. */ }
            }
        }

        /// <summary>
        /// Enumera los tipos de propiedad mapeados por este
        /// <see cref="PropertyMapper"/>.
        /// </summary>
        public virtual IEnumerable<Type> MapsTypes { get { yield break; } }

        /// <summary>
        /// Comprueba que este <see cref="PropertyMapper"/> sea capaz de
        /// mapear la propiedad especificada.
        /// </summary>
        /// <param name="property">Propiedad a comprobar.</param>
        /// <returns>
        /// <see langword="true"/> si este <see cref="PropertyMapper"/>
        /// puede mapear la propiedad, <see langword="false"/> en caso
        /// contrario.
        /// </returns>
        public virtual bool Maps(IPropertyDescription property)
        {
#if DEBUG
            if (!MethodBase.GetCurrentMethod().IsOverriden(this) && !MapsTypes.Any())
            {
                throw new Exception(
                    "Este mapper debe configurarse antes de usarse, invalidando " +
                    $"a la propiedad {nameof(MapsTypes)} o a este método.");
            }
#endif
            return MapsTypes.Contains(property.PropertyType);
        }

        /// <summary>
        /// Mapea la propiedad por medio de esta instancia.
        /// </summary>
        /// <param name="p">
        /// Propiedad a mapear.
        /// </param>
        /// <returns>
        /// Un objeto <see cref="IPropertyMapping"/> con controles de
        /// edición configurados para editar la propiedad en una página
        /// auto-generada de CRUD.
        /// </returns>
        public abstract IPropertyMapping Map(IPropertyDescription p);

        /// <summary>
        /// Obtiene un <see cref="PropertyMapping"/> que pueda utilizarse
        /// para generar un control de edición para la propiedad
        /// especificada.
        /// </summary>
        /// <param name="property">
        /// Propiedad a partir de la cual generar un control.
        /// </param>
        /// <returns>
        /// Un <see cref="PropertyMapping"/> que puede utilizarse para
        /// generar un control de edición para la propiedad especificada.
        /// </returns>
        public static IPropertyMapping GetMapping(IPropertyDescription property)
        {
            if (property is null) throw new ArgumentNullException(nameof(property));
            if (property.Hidden) return null;
            if (property.ReadOnly) return new ReadOnlyMapping(property);

            PropertyMapper m = null;
            lock (_mappers)
                m = _mappers.FirstOrDefault(p => p.Maps(property));

            return m?.Map(property) ?? new ReadOnlyMapping(property);
        }
    }
}