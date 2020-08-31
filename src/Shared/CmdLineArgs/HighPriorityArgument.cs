/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading;

namespace TheXDS.Proteus.Cmd
{
    public class HighPriorityArgument : PriorityArgumentBase
    {
        public HighPriorityArgument() : base("alta", ThreadPriority.Highest)
        {
        }
    }
}