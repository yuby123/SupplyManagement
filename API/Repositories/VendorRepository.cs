using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories
{
    public class VendorRepository : GeneralRepository<Vendor>, IVendorRepository
    {
        public VendorRepository(SMDbContext context) : base(context) { }

    }
}
