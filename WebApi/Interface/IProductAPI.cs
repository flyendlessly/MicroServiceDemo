using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;
namespace OrderApi.Interface
{
    /// <summary>
    /// Rfit应用：商品API调用
    /// </summary>
    public interface IProductAPI
    {

        //[Headers()] 请求头设置
        [Get("/api/Values")]
        Task<IEnumerable<string>> GetProduct();


        [Get("/api/Values/{id}")]
        Task<string> GetProduct([AliasAs("id")] int id);
    }
}
