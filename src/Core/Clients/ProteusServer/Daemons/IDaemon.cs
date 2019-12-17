/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Daemons
{
    public interface IDaemon
    {
        bool CanRun { get; }
        byte Schedule { get; }
        TimeSpan ScheduleTime { get; }

        void Run();
    }
}