/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/


using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class ModuleSecurityDescriptor : SecurityDescriptor
    {
        public bool Loaded { get; set; }
        public bool Accesible { get; set; }

        public override string ToString()
        {
            return $"Descriptor de seguridad de módulo {Id}";
        }
    }
}
