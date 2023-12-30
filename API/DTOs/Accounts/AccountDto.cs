using API.Models;
using API.Utilities.Enums;

namespace API.DTOs.Accounts;
public class AccountDto
{
    public Guid Guid { get; set; }
    public StatusAccount Status { get; set; }


    public static explicit operator AccountDto(Account account)
    {
        return new AccountDto
        {
            Guid = account.Guid, 
            Status = account.Status,
        };
    }


    public static implicit operator Account(AccountDto accountDto)
    {
        return new Account
        {
            Guid = accountDto.Guid,
            Status = accountDto.Status,
            ModifiedDate = DateTime.Now
        };
    }
}
