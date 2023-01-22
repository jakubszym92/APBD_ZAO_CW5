using APBD_ZAO_CW5.Models;
using System.Threading.Tasks;

namespace APBD_ZAO_CW5.DAL
{
    public interface IDbService
    {
        Task<int> AddProductToWarehouseAsync(ProductWarehouse productWarehouse);
    }
}
