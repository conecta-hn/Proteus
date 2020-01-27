/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.PluginSupport.Legacy;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Plugin de parchado rápido de objetos que contengan Quirks que no
    /// puedan ser solucionados en el código fuente de los mismos.
    /// </summary>
    public abstract class Patch : Plugin
    {
        /// <summary>
        /// Comprueba que este parche pueda ser aplicado a un objeto del
        /// tipo especificado.
        /// </summary>
        /// <param name="type">
        /// Tipo de objeto a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si este parche puede ser aplicado a un
        /// objeto del tipo especificado, <see langword="false"/> en caso
        /// contrario.
        /// </returns>
        public abstract bool Patches(Type type);

        /// <summary>
        /// Comprueba que este parche pueda ser aplicado al objeto 
        /// especificado.
        /// </summary>
        /// <param name="obj">
        /// Objeto a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si este parche puede ser aplicado al
        /// objeto especificado, <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual bool Patches(object obj) => Patches(obj.GetType());

        /// <summary>
        /// Aplica este parche al objeto.
        /// </summary>
        /// <param name="o">
        /// Objeto a parchar.
        /// </param>
        public abstract void Apply(object o);
    }

    /// <summary>
    /// Plugin de parchado rápido de objetos que contengan Quirks que no
    /// puedan ser solucionados en el código fuente de los mismos.
    /// </summary>
    public abstract class Patch<T> : Patch
    {
        /// <summary>
        /// Aplica este parche al objeto.
        /// </summary>
        /// <param name="o">
        /// Objeto a parchar.
        /// </param>
        public override sealed void Apply(object o)
        {
            if (o is T p) Apply(p);
        }

        /// <summary>
        /// Aplica este parche al objeto.
        /// </summary>
        /// <param name="obj">
        /// Objeto a parchar.
        /// </param>
        public abstract void Apply(T obj);

        /// <summary>
        /// Comprueba que este parche pueda ser aplicado al objeto 
        /// especificado.
        /// </summary>
        /// <param name="type">
        /// Tipo de objeto a comprobar.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si este parche puede ser aplicado al
        /// objeto del tipo especificado, <see langword="false"/> en caso
        /// contrario.
        /// </returns>
        public override bool Patches(Type type)
        {
            return type.Implements(typeof(T));
        }
    }
}