
using API.Models;
using API.Contracts;
using API.Data;

namespace API.Repositories
{
    public class AccountRepository : GeneralRepository<Account>, IAccountRepository
    {
        private readonly SMDbContext _context;

        public AccountRepository(SMDbContext context) : base(context)
        {
            _context = context;
        }

        public Account GetByCompanyEmail(string companyEmail)
        {
            var account = _context.Accounts
                .Join(
                    _context.Companies,
                    account => account.Guid,
                    company => company.Guid,
                    (account, company) => new
                    {
                        Account = account,
                        Company = company
                    }
                )

                .Where(joinResult => joinResult.Company.Email == companyEmail)
                .Select(joinResult => joinResult.Account)
                .FirstOrDefault();

            return account;
        }
    }
}
