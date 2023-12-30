using API.Models;

namespace API.Contracts
{
    public interface ICompanyRepository : IGeneralRepository<Company>
    {
        Company GetByCompanyEmail(string companyEmail);
        Company GetAdminEmployee();
    }
}
