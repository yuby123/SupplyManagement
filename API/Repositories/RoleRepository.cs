using API.Contracts;
using API.Data;
using API.Models;

namespace API.Repositories
{
    public class RoleRepository : GeneralRepository<Role>, IRoleRepository
    {
        public RoleRepository(SMDbContext context) : base(context) { }

        public Guid? GetDefaultGuid()
        {
            // Mengambil role dengan nama "user" dari database
            return _context.Set<Role>().FirstOrDefault(r => r.Name == "vendor")?.Guid;
        }

    }
}
