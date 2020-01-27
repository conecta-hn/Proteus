using System.Collections.Generic;

namespace TheXDS.Proteus.Models.Base
{
    public interface IContact
    {
        List<Email> Emails { get; set; }
        List<Phone> Phones { get; set; }
    }
}