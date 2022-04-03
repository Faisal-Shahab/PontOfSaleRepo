using POS.DataAccessLayer.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.DataAccessLayer.IServices
{
    public interface IDropdownsServices
    {
        int LanguageId { get; set; }
        int CompanyId { get; set; }
        Task<List<SelectListViewModel>> CategoriesDropdown();
        Task<List<SelectListViewModel>> CompaniesDropdown();
        Task<List<SelectListViewModel>> PaymentTypesDropdown();
        Task<List<ProductListViewModel>> ProductsDropdown(string value);
        Task<List<SelectListViewModel>> CustomersDropdown(string value);
        Task<List<SelectListViewModel>> SubscriptionsDropdown(string value);
        Task<List<SelectListViewModel>> RolesDropdown();
    }
}
