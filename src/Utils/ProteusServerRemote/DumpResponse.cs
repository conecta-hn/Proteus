/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Component
{
    public enum DumpResponse : byte
    {
        Critical,
        Done,
        Error,
        Info,
        Stop,
        Report,
        Warning,
        Message = 35
    }
}