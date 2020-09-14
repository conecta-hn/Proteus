namespace TheXDS.Proteus.Models
{
    public class GenericBatch : Batch
    {
        public override int Qty => InitialQty + CurrentQty;
        public int InitialQty { get; set; }
        public int CurrentQty { get; set; }

        public override string RebajarVenta(int qty, Factura f)
        {
            CurrentQty -= qty;
            return $"{qty} unidades de {Item.Name}";
        }

        public override Batch Split(int newQty)
        {
            CurrentQty -= newQty;

            return new GenericBatch
            {
                Item = Item,
                Lote = Lote,
                InitialQty = newQty                
            };
        }
    }
}