﻿/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/


using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class ModuleSecurityDescriptor : SecurityDescriptor
    {
        public bool Loaded { get; set; }
        public bool Accesible { get; set; }
    }
}
