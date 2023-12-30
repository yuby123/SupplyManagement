using API.DTOs.Accounts;
using API.DTOs.Tokens;
using API.Utilities.Handler;



namespace CLIENT.Contract
{
    public interface IAccountRepository : IRepository<AccountDto, Guid>
    {
        Task<ResponseOKHandler<ClaimsDto>> GetClaimsAsync(string token);
        Task<object> Login(LoginDto login);
        
    }
}
