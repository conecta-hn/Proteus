/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading.Tasks;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Plugins
{
    public abstract class Tool : WpfPlugin
    {
        public virtual Task PostLoadAsync() => Task.CompletedTask;
        public virtual Task PostLoginAsync() => Task.CompletedTask;
    }
}