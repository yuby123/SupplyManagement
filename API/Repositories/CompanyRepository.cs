using API.Data;
using API.Contracts;
using API.Models;

namespace API.Repositories
{
    public class CompanyRepository : GeneralRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(SMDbContext context) : base(context) { }

        public Company GetAdminEmployee()
        {
            return _context.Companies.FirstOrDefault(e => e.Account.Role.Name == "admin");
        }

        public Company GetByCompanyEmail(string companyEmail)
        {
            return _context.Companies.FirstOrDefault(company => company.Email == companyEmail);
        }
    }
}
