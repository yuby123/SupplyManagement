using API.Contracts;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.DTOs.Accounts;
using API.DTOs.Tokens;
using System.Net;
using System.Security.Claims;
using System.Transactions;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly ITokenHandlers _tokenHandler;

        public AccountController(ICompanyRepository companyRepository, IAccountRepository accountRepository, IRoleRepository roleRepository, IVendorRepository vendorRepository, ITokenHandlers tokenHandler)
        {
            _companyRepository = companyRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _vendorRepository = vendorRepository;
            _tokenHandler = tokenHandler;
        }

        [Authorize]
        [HttpGet("GetClaims/{token}")]
        public IActionResult GetClaims(string token)
        {
            var claims = _tokenHandler.ExtractClaimsFromJwt(token);
            return Ok(new ResponseOKHandler<ClaimsDto>
            {
                Code = StatusCodes.Status200OK,
                Status = HttpStatusCode.OK.ToString(),
                Message = "Claims has been retrieved",
                Data = claims
            });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Invalid input!"
                    });
                }

                var user = _accountRepository.GetByCompanyEmail(request.Email);
                var company = _companyRepository.GetByCompanyEmail(request.Email);

                if (user == null || company == null || !HashHandler.VerifyPassword(request.Password, user.Password))
                {
                    return BadRequest(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status400BadRequest,
                        Status = HttpStatusCode.BadRequest.ToString(),
                        Message = "Account or Password is invalid!",
                    });
                }

                var account = _accountRepository.GetByGuid(company.Guid);
                var vendor = _vendorRepository.GetByGuid(company.Guid);

                var claims = new List<Claim>
                {
                    new Claim("Email", company.Email),
                    new Claim("Name", string.Concat(company.Name)),
                    new Claim("Foto", company.Foto ?? ""),
                    new Claim("StatusAccount", account.Status.ToString() ?? "")
                };

    
                if (vendor != null && vendor.StatusVendor != null)
                {
                    claims.Add(new Claim("StatusVendor", vendor.StatusVendor.ToString() ?? ""));
                }
                else
                {
                    claims.Add(new Claim("StatusVendor", "DefaultValueForNoVendor"));
                }

                var role = _roleRepository.GetByGuid(user.RoleGuid);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Name));
                }

                var companyGuidClaim = new Claim("CompanyGuid", company.Guid.ToString());
                claims.Add(companyGuidClaim);

                var generateToken = _tokenHandler.Generate(claims);

                return Ok(new ResponseOKHandler<object>("Login Success", new { Token = generateToken }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Error during login",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            try
            {
                var result = _accountRepository.GetByGuid(guid);

                if (result is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Id Not Found"
                    });
                }
                return Ok(new ResponseOKHandler<AccountDto>((AccountDto)result));
            }
            catch (ExceptionHandler ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to retrieve data",
                    Error = ex.Message
                });
            }
        }

        [HttpPut]
        public IActionResult Update(AccountDto accountDto)
        {
            try
            {
                var entity = _accountRepository.GetByGuid(accountDto.Guid);
                if (entity is null)
                {

                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Data Not Found"
                    });
                }
                Account toUpdate = accountDto;
                toUpdate.CreatedDate = entity.CreatedDate;
                toUpdate.Password = entity.Password;
                toUpdate.RoleGuid = entity.RoleGuid;

                _accountRepository.Update(toUpdate);
                return Ok(new ResponseOKHandler<string>("Data Updated"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to Update data",
                    Error = ex.Message
                });
            }
        }

    }
}
