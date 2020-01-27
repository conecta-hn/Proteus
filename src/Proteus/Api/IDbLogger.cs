using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Api
{
    public interface IDbLogger
    {
        Task<Result> Log(DbChangeTracker ct, IProteusCredential credential);
    }
}
