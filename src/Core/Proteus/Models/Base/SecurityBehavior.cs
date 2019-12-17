/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/


using System;

namespace TheXDS.Proteus.Models.Base
{
    [Flags]
    public enum SecurityBehavior
    {
        Locked,
        Visible,
        Enabled,
        Unlocked,
    }
}