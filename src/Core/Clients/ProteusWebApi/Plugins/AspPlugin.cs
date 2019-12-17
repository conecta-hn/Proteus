using TheXDS.Proteus.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.PluginSupport;

namespace TheXDS.Proteus.Plugins
{
    public class ProteusAspModule : Plugin
    {
        public virtual IEnumerable<Type> ExploreControllers()
        {
            return TheXDS.MCART.Objects.GetTypes<ProteusWebController>(true);
        }
    }
}
