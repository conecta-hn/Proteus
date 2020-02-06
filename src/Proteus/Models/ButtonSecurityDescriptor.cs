/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/


using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class ButtonSecurityDescriptor : SecurityDescriptor
    {
        public bool Visible { get; set; }
        public bool Available { get; set; }
        public bool Elevatable { get; set; }
        public override string ToString()
        {
            return $"Descriptor de seguridad de función {Id}";
        }
    }

}
