using PointOfSale.Model;
using POS.DataAccessLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace POS.DataAccessLayer.IServices
{
    public interface IProductServices
    {
        public int CompanyId { get; set; }
        Task<IEnumerable<ProductsListViewModel>> GetProducts(SearchFilter filter);
        Task<bool> Insert(ProductViewModel model, string user);
        Task<bool> Update(ProductViewModel model, string user);
        Task<ProductViewModel> GetProductById(int id);
        Task<bool> Delete(int id);
    }
}
