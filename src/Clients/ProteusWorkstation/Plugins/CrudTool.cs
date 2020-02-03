/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using System.Linq;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Clase base para una herramienta persnalizada de Crud.
    /// </summary>
    public abstract class CrudTool : WpfPlugin
    {
        /// <summary>
        /// Define las vistas de Crud en las cuales la herramienta será
        /// visible.
        /// </summary>
        public CrudToolVisibility Visibility { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CrudTool"/>.
        /// </summary>
        /// <param name="visibility">
        /// Vistas de Crud en las cuales la herramienta será visible.
        /// </param>
        protected CrudTool(CrudToolVisibility visibility)
        {
            Visibility = visibility;
        }

        /// <summary>
        /// Obtiene un valor que determina si este <see cref="CrudTool"/>
        /// estará disponible dada la colección de modelos de la ventana Crud.
        /// </summary>
        /// <param name="models">Modelos de la ventana de Crud.</param>
        /// <returns>
        /// <see langword="true"/> si este <see cref="CrudTool"/> está
        /// disponible dada la colección de modelos de la ventana Crud,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual bool Available(IEnumerable<Type> models) => true;

        /// <summary>
        /// Obtiene un valor que determina si este <see cref="CrudTool"/>
        /// estará disponible dado el modelo principal de la ventana Crud.
        /// </summary>
        /// <param name="model">Modelo principal de la ventana de Crud.</param>
        /// <returns>
        /// <see langword="true"/> si este <see cref="CrudTool"/> está
        /// disponible dado el modelo principal de la ventana Crud,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual bool Available(Type model) => Available(new[] { model });

        /// <summary>
        /// Obtiene la colección de <see cref="Launcher"/> a presentar en la
        /// ventana de Crud.
        /// </summary>
        /// <param name="models">Modelos de la ventana de Crud.</param>
        /// <param name="vm">
        /// Instancia del <see cref="ICrudViewModel"/> que gestiona el
        /// comportamiento de la ventana de Crud.
        /// </param>
        /// <returns>
        /// Una enumeración de <see cref="Launcher"/> a presentar en las
        /// distintas vistas de la ventana de Crud.
        /// </returns>
        public abstract IEnumerable<Launcher> GetLaunchers(IEnumerable<Type> models, ICrudViewModel vm);

        /// <summary>
        /// Obtiene la colección de <see cref="Launcher"/> a presentar en la
        /// ventana de Crud.
        /// </summary>
        /// <param name="model">Modelo principal de la ventana de Crud.</param>
        /// <param name="vm">
        /// Instancia del <see cref="ICrudViewModel"/> que gestiona el
        /// comportamiento de la ventana de Crud.
        /// </param>
        /// <returns>
        /// Una enumeración de <see cref="Launcher"/> a presentar en las
        /// distintas vistas de la ventana de Crud.
        /// </returns>
        public virtual IEnumerable<Launcher> GetLaunchers(Type model, ICrudViewModel vm) => GetLaunchers(new[] { model }, vm);
    }

    /// <summary>
    /// Clase base para una herramienta personalizada de Crud que define el
    /// modelo de datos para el cual se aplica.
    /// </summary>
    /// <typeparam name="T">
    /// Modelo de datos, clase base o interfaz para el cual exponer las
    /// acciones de este <see cref="CrudTool{T}"/>.
    /// </typeparam>
    public abstract class CrudTool<T> : CrudTool
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="CrudTool{T}"/>.
        /// </summary>
        /// <param name="visibility">
        /// Vistas de Crud en las cuales la herramienta será visible.
        /// </param>
        protected CrudTool(CrudToolVisibility visibility) : base(visibility)
        {
        }

        /// <summary>
        /// Obtiene un valor que determina si este <see cref="CrudTool"/>
        /// estará disponible dada la colección de modelos de la ventana Crud.
        /// </summary>
        /// <param name="models">Modelos de la ventana de Crud.</param>
        /// <returns>
        /// <see langword="true"/> si este <see cref="CrudTool"/> está
        /// disponible dada la colección de modelos de la ventana Crud,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public override bool Available(IEnumerable<Type> models)
        {
            return models.Any(p => p.Implements<T>());
        }
    }
}