using Microsoft.EntityFrameworkCore;
using POS.DataAccessLayer.Models.Company;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace POS.DataAccessLayer.Services
{

    public class CompanyServices
    {
        private readonly AppDbContext _appDbContext;
        public CompanyServices(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<CompanyModel>> GetCompanies()
        {
            return await _appDbContext.Companies.ToListAsync();
        }

        public async Task<CompanyModel> GetCompanyById(int id)
        {
            return await _appDbContext.Companies.FirstOrDefaultAsync(x => x.CompanyId == id);
        }

        public async Task<bool> CreateCompany(CompanyModel model)
        {
            await _appDbContext.Companies.AddAsync(model);
            return await SaveChangesAsync();
        }



        public async Task<bool> SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
