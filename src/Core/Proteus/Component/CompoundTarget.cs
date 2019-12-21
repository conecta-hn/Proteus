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
    public class CompoundReporter : IStatusReporter
    {
        public ICollection<IStatusReporter> Reporters { get; private set; } = new HashSet<IStatusReporter>();
        public static void Register(IEnumerable<IStatusReporter> reporters)
        {
            if (Proteus.CommonReporter is CompoundReporter cp)
            {
                foreach (var j in reporters.ExceptFor(cp).NotNull()) cp.Reporters.Add(j);
            }
            else
            {
                Proteus.CommonReporter = new CompoundReporter
                {
                    Reporters = new HashSet<IStatusReporter>(reporters.Concat(new[] { Proteus.CommonReporter }).NotNull().Where(p => p.GetType() != typeof(CompoundReporter)))
                };
            }

        }
        [Sugar]
        public static void Register(IStatusReporter target)
        {
            Register(new[] { target });
        }

        public void Done()
        {
            Parallel.ForEach(Reporters, j => j.Done());
        }

        public void Done(string text)
        {
            Parallel.ForEach(Reporters, j => j.Done(text));
        }

        public void UpdateStatus(double progress)
        {
            Parallel.ForEach(Reporters, j => j.UpdateStatus(progress));
        }

        public void UpdateStatus(double progress, string text)
        {
            Parallel.ForEach(Reporters, j => j.UpdateStatus(progress,text));
        }

        public void UpdateStatus(string text)
        {
            Parallel.ForEach(Reporters, j => j.UpdateStatus(text));
        }
    }
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
        }

        public void Critical(Exception ex)
        {
            Parallel.ForEach(Targets, j => j.Critical(ex));
        }

        public void Error(string message)
        {
            Parallel.ForEach(Targets, j => j.Error(message));
        }

        public void Info(string message)
        {
            Parallel.ForEach(Targets, j => j.Info(message));
        }

        public void Show(string message)
        {
            Parallel.ForEach(Targets, j => j.Show(message));
        }

        public void Show(string title, string message)
        {
            Parallel.ForEach(Targets, j => j.Show(title, message));
        }

        public void Stop(string message)
        {
            Parallel.ForEach(Targets, j => j.Stop(message));
        }

        public void Warning(string message)
        {
            Parallel.ForEach(Targets, j => j.Warning(message));
        }
    }
}