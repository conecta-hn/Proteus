using System.Linq;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Conecta.Models;
using static TheXDS.MCART.Types.Extensions.EnumerableExtensions;

namespace TheXDS.Proteus.Conecta.ViewModels
{
    /*
    public class LoteViewModel : ViewModel<Lote>
    {
        public int VendidosCount
        {
            get
            {
                if (!(Entity?.Items is { } i)) return 0;
                var c = 0;
                foreach (IPagable j in i.Select(p => p.MenudeoParent).NotNull())
                {
                    if (j.Debe == 0m) c++;
                }
                return c;
            }
        }

        public int CreditosCount
        {
            get
            {
                if (!(Entity?.Items is { } i)) return 0;
                var c = 0;
                foreach (IPagable j in i.Select(p => p.MenudeoParent).NotNull())
                {
                    if (j.Debe > 0m) c++;
                }
                return c;
            }
        }

        public int ExistenciasCount
        {
            get
            {
                if (!(Entity?.Items is { } i)) return 0;
                var c = 0;
                foreach (var j in i)
                {
                    if (j.MenudeoParent is null) c++;
                }
                return c;
            }
        }

        public LoteViewModel()
        {
            RegisterPropertyChangeBroadcast(
                nameof(Lote.Items),
                nameof(VendidosCount),
                nameof(CreditosCount),
                nameof(ExistenciasCount));
        }
    }
    */
}
