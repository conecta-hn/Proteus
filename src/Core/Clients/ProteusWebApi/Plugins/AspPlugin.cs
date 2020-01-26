/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.MCART;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.Proteus.Controllers.Base;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Clase base para todos los plugins de Pruteus que contengan
    /// controladores de Web API cargables en Runtime.
    /// </summary>
    public class ProteusAspModule : Plugin
    {
        /// <summary>
        /// Explora la aplicación actual en busca de controladores cargables
        /// desde Plugins de MCART.
        /// </summary>
        /// <returns>
        /// Una enumeración de todos los controladores encontrados.
        /// </returns>
        public virtual IEnumerable<Type> ExploreControllers()
        {
            return Objects.GetTypes<ProteusWebController>(true);
        }
    }
}