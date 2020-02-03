/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class AddressArea : ModelBase<long>, IAddressArea
    {
        public string City { get; set; }
        public string Province { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }

}