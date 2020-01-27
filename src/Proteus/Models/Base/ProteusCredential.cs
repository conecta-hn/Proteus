/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheXDS.Proteus.Api;

namespace TheXDS.Proteus.Models.Base
{
    public class ProteusCredential : ModelBase<string>, IProteusCredential, IProteusRwCredential
    {
        [Required]
        public string Name { get; set; }
        public SecurityBehavior? ModuleBehavior { get; set; }
        public SecurityBehavior? ButtonBehavior { get; set; }
        [Required]
        public SecurityFlags DefaultGranted { get; set; }
        [Required]
        public SecurityFlags DefaultRevoked { get; set; }
        public virtual List<SecurityDescriptor> Descriptors { get; set; } = new List<SecurityDescriptor>();

        IEnumerable<ISecurityDescriptor> IProteusCredential.Descriptors => Descriptors.AsReadOnly();
        ICollection<SecurityDescriptor> IProteusRwCredential.Descriptors => Descriptors;
    }
}