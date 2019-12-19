/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Inventario.Models
{
    public class Bodega : Nameable<int>
    {
        public virtual List<Batch> Batches { get; set; }
    }
}