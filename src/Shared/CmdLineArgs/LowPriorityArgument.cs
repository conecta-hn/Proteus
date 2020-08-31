/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading;

namespace TheXDS.Proteus.Cmd
{
    public class LowPriorityArgument : PriorityArgumentBase
    {
        public LowPriorityArgument() : base("baja", ThreadPriority.Lowest)
        {
        }

        public override char? ShortName => 'l';
    }
}