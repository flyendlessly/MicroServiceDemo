using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;
namespace OrderApi.Interface
{
    public interface IProductAPI
    {
        //[Headers()]
        [Post("/api/Values")]
        Task<IEnumerable<string>> GetProduct();
        //Task<ResponseBody> GetProduct()
    }
}
