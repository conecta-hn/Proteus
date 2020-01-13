/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Marca una propiedad para ser excluida de la generación de
    /// ViewModels dinámicos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ExcludeFromVmFactoryAttribute : Attribute { }

    /// <summary>
    /// Clase base para todos los modelos de datos de Proteus.
    /// </summary>
    public abstract class ModelBase : ISoftDeletable
    {
        internal static Type ResolveModelType(Type t)
        {
            return t.Assembly.IsDynamic ? ResolveModelType(t.BaseType) : t;
        }

        internal IEnumerable<PropertyInfo> Properties => GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite);

        internal void SetFrom(ModelBase data)
        {
            if (GetType() != data.GetType()) throw new InvalidCastException();
            foreach(var j in Properties) j.SetValue(this, j.GetValue(data));            
        }

        internal abstract Type IdType { get; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la entidad ha sido
        /// marcada como borrada de la base de datos.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Obtiene el Id de la entidad como una cadena.
        /// </summary>
        public abstract string StringId { get; }
        
        /// <summary>
        /// Obtiene el Id de la entidad como un objeto.
        /// </summary>
        public abstract object IdAsObject { get; }

        /// <summary>
        /// Obtiene o establece el valor del campo llave principal de esta
        /// entidad.
        /// </summary>
        [ExcludeFromVmFactory]
        public object Id { get; set; }

        /// <summary>
        /// Determina si el modelo corresponde a una nueva entidad.
        /// </summary>
        public abstract bool IsNew { get; }

        /// <summary>
        /// Obtiene un valor que indica si esta entidad contiene un Id.
        /// </summary>
        public abstract bool HasId { get; }

        /// <summary>
        /// Comprueba si existen cambios sin guardar en la base de datos
        /// para esta entidad.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> si existen cambios pendientes de
        /// guardar, <see langword="false" /> en caso contrario.
        /// </returns>
        public bool ChangesPending()
        {
            return Proteus.Infer(GetType().ResolveToDefinedType()).ChangesPending(this);
        }

        /// <summary>
        /// Deshace los cambios pendientes de guardado realizados sobre
        /// esta entidad.
        /// </summary>
        public void Rollback()
        {
            Proteus.Infer(GetType()).Rollback(this);
        }

        /// <summary>
        /// Convierte una entidad en su representación como una cadena.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this switch
            {
                INameable nameable => nameable.Name,
                IDescriptible descriptible => descriptible.Description!,
                _ => $"{(!GetType().Assembly.IsDynamic ? GetType().NameOf() : GetType().BaseType.NameOf())} {StringId}",
            };
        }
    }

    /// <summary>
    /// Clase base para los modelos de datos de Proteus que determina el tipo
    /// de campo llave a utilizar.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de campo llave principal a utilizar.
    /// </typeparam>
    public abstract class ModelBase<T> : ModelBase, IModelBase<T> where T : IComparable<T>
    {
        internal override sealed Type IdType => typeof(T);

        /// <summary>
        /// Obtiene o establece el valor del campo llave principal de esta
        /// entidad.
        /// </summary>
        [Key]
        public new T Id { get; set; }

        /// <summary>
        /// Obtiene el Id de la entidad como una cadena.
        /// </summary>
        public override sealed string StringId => Id?.ToString() ?? string.Empty;

        /// <summary>
        /// Obtiene el Id de la entidad como un objeto.
        /// </summary>
        public override sealed object IdAsObject => Id;

        /// <summary>
        /// Determina si el modelo corresponde a una nueva entidad.
        /// </summary>
        public override sealed bool IsNew
        {
            get
            {
                var t = GetType().ResolveCollectionType().ResolveToDefinedType();
                return !(HasId && Proteus.InferService(t).Exists(this));
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si esta entidad contiene un Id.
        /// </summary>
        public override sealed bool HasId => !(Id?.Equals(default(T)) ?? true);
    }
}
