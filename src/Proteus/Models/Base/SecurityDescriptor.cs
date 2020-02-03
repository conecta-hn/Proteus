/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

namespace TheXDS.Proteus.Models.Base
{
    public abstract class SecurityDescriptor : ModelBase<string>, ISecurityDescriptor
    {
        public virtual ProteusCredential Parent { get; set; }

        IProteusCredential ISecurityDescriptor.Parent => Parent;
    }
}
