/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Extensions;
using System.Collections;
using static TheXDS.MCART.Objects;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Contiene métodos de extensión que proveen de atajos de
    /// configuración para descriptores de propiedades de CRUD.
    /// </summary>
    public static class PropertyDescriptorShortcuts
    {
        #region Atajos de descripción de propiedad

        /// <summary>
        /// Atajo que configura una propiedad como un nombre.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="label">
        /// Etiqueta opcional a mostrar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor AsName(this IPropertyDescriptor p, string label)
        {
            return p.Label(label).Important().NotEmpty();
        }

        /// <summary>
        /// Atajo que configura una propiedad como un nombre.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor AsName(this IPropertyDescriptor p)
        {
            return p.AsName("Nombre descriptivo");
        }

        /// <summary>
        /// Muestra una propiedad únicamente en la vista de detalles.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor OnlyInDetails(this IPropertyDescriptor p)
        {
            p.Hidden();
            return p.ShowInDetails();
        }

        /// <summary>
        /// Muestra una propiedad únicamente en la vista de detalles.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="label">
        /// Etiqueta opcional a mostrar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor OnlyInDetails(this IPropertyDescriptor p, string label)
        {
            p.Hidden();
            return p.ShowInDetails().Label(label);
        }

        /// <summary>
        /// Atajo que configura una propiedad como un nombre.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="label">
        /// Etiqueta opcional a mostrar.
        /// </param>
        /// <param name="icon">Ícono opcional a mostrar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor AsName(this IPropertyDescriptor p, string label, string icon)
        {
            return p.Label(label).Icon(icon).Important();
        }

        /// <summary>
        /// Establece la propiedad como importante, ejecutando
        /// <see cref="IPropertyDescriptor.WatermarkAlwaysVisible()"/>,
        /// <see cref="IPropertyDescriptor.AsListColumn()"/> y 
        /// <see cref="IPropertyDescriptor.ShowInDetails()"/>
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor Important(this IPropertyDescriptor p)
        {
            return p.WatermarkAlwaysVisible()
                .ShowInDetails()
                .AsListColumn();
        }

        /// <summary>
        /// Establece la propiedad como importante, ejecutando
        /// <see cref="IPropertyDescriptor.WatermarkAlwaysVisible()"/>,
        /// <see cref="IPropertyDescriptor.AsListColumn()"/> y 
        /// <see cref="IPropertyDescriptor.ShowInDetails()"/>
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="label">Etiqueta a establecer.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor Important(this IPropertyDescriptor p, string label)
        {
            return p.WatermarkAlwaysVisible()
                .ShowInDetails()
                .AsListColumn()
                .Label(label);
        }

        /// <summary>
        /// Atajo que configura una propiedad de campo llave.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="label">Etiqueta a mostrar.</param>
        /// <param name="icon">Ícono opcional a mostrar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor Id(this IPropertyDescriptor p, string label, string icon)
        {
            return p.Label(label).Icon(icon).Important().Required();
        }

        /// <summary>
        /// Atajo que configura una propiedad de campo llave.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="label">Etiqueta a mostrar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor Id(this IPropertyDescriptor p, string label)
        {
            return p.Id(label, "🗝");
        }

        /// <summary>
        /// Atajo que configura una propiedad de campo llave.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor Id(this IPropertyDescriptor p)
        {
            return p.Id("Id", "🗝");
        }

        /// <summary>
        /// Marca una lista como únicamente creable, lo cual permite hacer
        /// Binding de orígen de lista personalizado.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IListPropertyDescriptor<T> CreatableOnly<T>(this IListPropertyDescriptor<T> p) where T : ModelBase
        {
            return (IListPropertyDescriptor<T>)p.Source(null).Creatable();
        }

        /// <summary>
        /// Marca un campo para no ser una cadena nula.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor NotEmpty(this IPropertyDescriptor p)
        {
            return p.Required().Validator(CheckNotEmpty);
        }
        
        /// <summary>
        /// Marca un campo para no ser una cadena nula.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor NotNull(this IPropertyDescriptor p)
        {
            return p.Required().Validator(CheckNotNull);
        }

        /// <summary>
        /// Marca una colección para indicar que debe contener al menos un elemento.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor NotEmpty<T>(this IListPropertyDescriptor<T> p) where T : ModelBase
        {
            return p.Required().Validator(CheckListNotEmpty);
        }

        /// <summary>
        /// Permite agregar múltiples funciones de validación a un mismo
        /// campo.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="validations">
        /// Funciones de validación a concatenar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor Validations(this IPropertyDescriptor p, params Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>>[] validations)
        {
            return p.Validator((e, pr) => validations.SelectMany(q => q?.Invoke(e, pr)));
        }

        /// <summary>
        /// Permite agregar múltiples funciones de validación a un mismo
        /// campo.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="validations">
        /// Funciones de validación a concatenar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor Validations<T>(this IPropertyDescriptor p, params Func<T, PropertyInfo, IEnumerable<ValidationError>>[] validations) where T: ModelBase, new()
        {
            return p.Validator<T>((e, pr) => validations.SelectMany(q => q?.Invoke(e, pr)));
        }

        /// <summary>
        /// Marca un campo como columna de una lista.
        /// </summary>
        /// <param name="p">Propiedad a configurar.</param>
        /// <param name="format">Formato a aplicar al campo.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IPropertyDescriptor AsListColumn(this IPropertyDescriptor p, string format)
        {
            return p.AsListColumn().Format(format);
        }

        /// <summary>
        /// Indica que una lista debe contener controles para la creación
        /// de nuevas entidades de la clase base especificada.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo base de los modelos que deben estar disponibles para la
        /// creación.
        /// </typeparam>
        /// <param name="p">Propiedad a configurar.</param>
        /// <returns>
        /// La misma instancia que <paramref name="p"/>.
        /// </returns>
        [Sugar]
        public static IDataPropertyDescriptor Creatable<T>(this IDataPropertyDescriptor p) where T : ModelBase
        {
            return p.Creatable(GetTypes<T>(true));
        }

        public static IPropertyNumberDescriptor<decimal> Positive(this IPropertyNumberDescriptor<decimal> p)
        {
            return p.Range(0.01m, decimal.MaxValue);
        }

        public static IPropertyNumberDescriptor<short> Positive(this IPropertyNumberDescriptor<short> p)
        {
            return p.Range(1, short.MaxValue);
        }
        public static IPropertyNumberDescriptor<int> Positive(this IPropertyNumberDescriptor<int> p)
        {
            return p.Range(1, int.MaxValue);
        }
        public static IPropertyNumberDescriptor<long> Positive(this IPropertyNumberDescriptor<long> p)
        {
            return p.Range(1L, long.MaxValue);
        }
        public static IPropertyNumberDescriptor<float> Positive(this IPropertyNumberDescriptor<float> p)
        {
            return p.Range(float.Epsilon, float.MaxValue);
        }
        public static IPropertyNumberDescriptor<double> Positive(this IPropertyNumberDescriptor<double> p)
        {
            return p.Range(double.Epsilon, double.MaxValue);
        }

        private static IEnumerable<ValidationError> CheckNotZero(ModelBase m, PropertyInfo prop)
        {
            var v = prop.GetValue(m);
            if (v is null || v.Equals(prop.PropertyType.Default()))
            {
                yield return new NullValidationError(prop);
            }
        }

        public static IPropertyNumberDescriptor<decimal> NonZero(this IPropertyNumberDescriptor<decimal> p)
        {
            p.Validations(CheckNotZero);
            return p;
        }

        public static IPropertyNumberDescriptor<short> NonZero(this IPropertyNumberDescriptor<short> p)
        {
            p.Validations(CheckNotZero);
            return p;
        }
        public static IPropertyNumberDescriptor<int> NonZero(this IPropertyNumberDescriptor<int> p)
        {
            p.Validations(CheckNotZero);
            return p;
        }
        public static IPropertyNumberDescriptor<long> NonZero(this IPropertyNumberDescriptor<long> p)
        {
            p.Validations(CheckNotZero);
            return p;
        }
        public static IPropertyNumberDescriptor<float> NonZero(this IPropertyNumberDescriptor<float> p)
        {
            p.Validations(CheckNotZero);
            return p;
        }
        public static IPropertyNumberDescriptor<double> NonZero(this IPropertyNumberDescriptor<double> p)
        {
            p.Validations(CheckNotZero);
            return p;
        }

        public static IPropertyDateDescriptor Timestamp(this IPropertyDateDescriptor p)
        {
            p.Default(DateTime.Now).Label("Fecha de creación");
            return p.WithTime();
        }

        #endregion

        #region Bulks de descripción de modelos

        /// <summary>
        /// Describe rápidamente las propiedades de dirección del modelo
        /// actual.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo a describir.
        /// </typeparam>
        /// <param name="descriptor">
        /// Instancia del descriptor de modelos.
        /// </param>
        [Sugar]
        public static void DescribeAddress<T>(this CrudDescriptor<T> descriptor) where T : ModelBase, IAddressable, new()
        {
            descriptor.TextProperty(p => p.Address).TextKind(TextKind.Big).Label("Dirección").Icon("🏢").Required().Validator(CheckAddress).ShowInDetails();
            descriptor.Property(p => p.City).Label("Cuidad").Icon("🏙").NotEmpty().ShowInDetails();
            descriptor.Property(p => p.Province).Label("Provincia/Departamento").Icon("🚩").NotEmpty().ShowInDetails();
            descriptor.Property(p => p.Zip).Label("Código Zip").Icon("📮").Nullable().ShowInDetails();
            descriptor.Property(p => p.Country).Label("País").Icon("🏳").NotEmpty().ShowInDetails();
        }

        /// <summary>
        /// Describe rápidamente las propiedades de contacto del modelo
        /// actual.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo a describir.
        /// </typeparam>
        /// <param name="descriptor">
        /// Instancia del descriptor de modelos.
        /// </param>
        [Sugar]
        public static void DescribeContact<T>(this CrudDescriptor<T> descriptor) where T : ModelBase, IContact, new()
        {
            descriptor.ListProperty(p => p.Emails)
                .Editable()
                .Creatable()
                .Label("Correos de contacto")
                .Icon("📧")
                .ShowInDetails()
                .Required();

            descriptor.ListProperty(p => p.Phones)
                .Editable()
                .Creatable()
                .Label("Teléfonos")
                .Icon("📞")
                .ShowInDetails()
                .Required();
        }

        /// <summary>
        /// Describe rápidamente las propiedades de información de equipo
        /// del modelo actual.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo a describir.
        /// </typeparam>
        /// <param name="descriptor">
        /// Instancia del descriptor de modelos.
        /// </param>
        [Sugar]
        public static void DescribeEstacion<T>(this CrudDescriptor<T> descriptor) where T : EstacionBase, new()
        {
            descriptor.Property(p => p.Id).Id("Nombre del equipo").Default(Environment.MachineName);
            descriptor.Property(p => p.Name).AsName().Default($"Equipo de {Environment.UserName}");
        }

        /// <summary>
        /// Describe una entidad que ofrece representaciones de valores 
        /// absolutos o relativos.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo a describir.
        /// </typeparam>
        /// <param name="descriptor">
        /// Instancia del descriptor de modelos.
        /// </param>
        [Sugar]
        public static void DescribeValuable<T>(this CrudDescriptor<T> descriptor) where T: ModelBase, IValuable, new()
        {
            descriptor.NumericProperty(p => p.StaticValue)
                .Range(decimal.Zero, decimal.MaxValue)
                .Nullable()
                .RadioSelectable()
                .Label("Valor estático");

            descriptor.NumericProperty(p => p.PercentValue)
                .Range(0f, 1f)
                .Nullable()
                .RadioSelectable()
                .Label("Valor porcentual");
        }

        #endregion

        #region Validaciones personalizadas

        /// <summary>
        /// Valida una dirección.
        /// </summary>
        /// <param name="entity">Entidad a validar.</param>
        /// <param name="prop">Referencia a la propiedad a validar.</param>
        /// <returns>
        /// Una colección de errores de validación si existen problemas, o
        /// una colección vacía si la entidad ha superado todas las
        /// validaciones.
        /// </returns>
        public static IEnumerable<ValidationError> CheckAddress(ModelBase entity, PropertyInfo prop)
        {
            var m = entity as IAddressable ?? throw new InvalidOperationException();
            if (m.Address.IsEmpty()) yield return new ValidationError(prop, "Se necesita una dirección");
            if (m.Address.CountChars(' ', ',') < 3) yield return new ValidationError(prop, "Esa no parece ser una dirección válida.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entidad a validar.</param>
        /// <param name="prop">Referencia a la propiedad a validar.</param>
        /// <returns>
        /// Una colección de errores de validación si existen problemas, o
        /// una colección vacía si la entidad ha superado todas las
        /// validaciones.
        /// </returns>
        public static IEnumerable<ValidationError> CheckNotEmpty(ModelBase entity, PropertyInfo prop)
        {
            if (prop.GetValue(entity)?.ToString()?.IsEmpty() ?? true) yield return new ValidationError(prop, "Este campo es requerido.");
        }

        /// <summary>
        /// Validación que comprueba que el objeto no sea <see langword="null"/>.
        /// </summary>
        /// <param name="entity">Entidad a validar.</param>
        /// <param name="prop">Referencia a la propiedad a validar.</param>
        /// <returns>
        /// Una colección de errores de validación si existen problemas, o
        /// una colección vacía si la entidad ha superado todas las
        /// validaciones.
        /// </returns>
        public static IEnumerable<ValidationError> CheckNotNull(ModelBase entity, PropertyInfo prop)
        {
            if (prop.GetValue(entity) is null) yield return new NullValidationError(prop);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entidad a validar.</param>
        /// <param name="prop">Referencia a la propiedad a validar.</param>
        /// <returns>
        /// Una colección de errores de validación si existen problemas, o
        /// una colección vacía si la entidad ha superado todas las
        /// validaciones.
        /// </returns>
        public static IEnumerable<ValidationError> CheckListNotEmpty(ModelBase entity, PropertyInfo prop)
        {
            if (!(prop.GetValue(entity) is IEnumerable c)) yield break;
            if (!c.ToGeneric().Any()) yield return new ValidationError(prop, "Se necesita al menos un elemento en esta colección.");
        }

        #endregion
    }
}