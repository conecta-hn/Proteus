/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    public class CompoundTarget : IMessageTarget
    {
        public static void Register(IEnumerable<IMessageTarget> targets)
        {
            if (Proteus.MessageTarget is CompoundTarget cp)
            {
                foreach (var j in targets.ExceptFor(cp).NotNull()) cp.Targets.Add(j);
            }
            else
            {
                Proteus.MessageTarget = new CompoundTarget
                {
                    Targets = new HashSet<IMessageTarget>(targets.Concat(new[] { Proteus.MessageTarget }).NotNull().Where(p => p.GetType() != typeof(CompoundTarget)))
                };
            }
            
        }
        [Sugar] public static void Register(IMessageTarget target)
        {
            Register(new[] { target });
        }

        public ICollection<IMessageTarget> Targets { get; private set; } = new HashSet<IMessageTarget>();

        public void Critical(string message)
        {
            Parallel.ForEach(Targets, j => j.Critical(message));
            //foreach(var j in Targets)j.Critical(message);
        }

        public void Critical(Exception ex)
        {
            Parallel.ForEach(Targets, j => j.Critical(ex));
            //foreach (var j in Targets) j.Critical(ex);
        }

        public void Error(string message)
        {
            Parallel.ForEach(Targets, j => j.Error(message));
            //foreach (var j in Targets) j.Error(message);
        }

        public void Info(string message)
        {
            Parallel.ForEach(Targets, j => j.Info(message));
            //foreach (var j in Targets) j.Info(message);
        }

        public void Show(string message)
        {
            Parallel.ForEach(Targets, j => j.Show(message));
            //foreach (var j in Targets) j.Show(message);
        }

        public void Show(string title, string message)
        {
            Parallel.ForEach(Targets, j => j.Show(title, message));
            //foreach (var j in Targets) j.Show(title, message);
        }

        public void Stop(string message)
        {
            Parallel.ForEach(Targets, j => j.Stop(message));
            //foreach (var j in Targets) j.Stop(message);
        }

        public void Warning(string message)
        {
            Parallel.ForEach(Targets, j => j.Warning(message));
            //foreach (var j in Targets) j.Warning(message);
        }
    }
}