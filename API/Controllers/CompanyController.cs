using API.Contracts;
using API.DTOs.Accounts;
using API.DTOs.Companies;
using API.Models;
using API.Repositories;
using API.Utilities.Enums;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyController : ControllerBase
    {

        private readonly IAccountRepository _accountRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IRoleRepository _roleRepository;



        public CompanyController(IAccountRepository accountRepository,
            IVendorRepository vendorRepository, ICompanyRepository companyRepository, IRoleRepository roleRepository)
        {

            _accountRepository = accountRepository;
            _companyRepository = companyRepository;
            _vendorRepository = vendorRepository;
            _roleRepository = roleRepository;



        }

        [HttpPost("registerCompany")]
        public async Task<IActionResult> RegisterCompany([FromForm] RegisterCompanyDto registrationDto)
        {
            if (ModelState.IsValid)
            {
                using (var transactionScope = new TransactionScope())
                {
                    try
                    {
                        Account account = registrationDto;
                        Company company = registrationDto;
                        Vendor vendor = registrationDto;

                        byte[] photoBytes = null;

                        if (registrationDto.FotoCompany != null && registrationDto.FotoCompany.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                registrationDto.FotoCompany.CopyTo(memoryStream);
                                photoBytes = memoryStream.ToArray();
                            }

                            company.Foto = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid()}_{Path.GetFileName(registrationDto.FotoCompany.FileName)}";
                        }

                        if (registrationDto.ConfirmPassword != registrationDto.Password)
                        {
                            return BadRequest(new ResponseErrorHandler
                            {
                                Code = StatusCodes.Status400BadRequest,
                                Status = HttpStatusCode.BadRequest.ToString(),
                                Message = "Password and Confirm Password do not match."
                            });
                        }

                        _companyRepository.Create(company);

                        if (photoBytes != null)
                        {
                            string uploadPath = "Utilities/File/FotoCompany/";
                            string filePath = Path.Combine(uploadPath, company.Foto);

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                fileStream.Write(photoBytes, 0, photoBytes.Length);
                            }
                        }

                        vendor.Guid = company.Guid;
                        _vendorRepository.Create(vendor);

                        account.Guid = company.Guid;
                        account.RoleGuid = _roleRepository.GetDefaultGuid() ?? throw new Exception("Default role not found");
                        account.Password = HashHandler.HashPassword(registrationDto.Password);

                        _accountRepository.Create(account);

                        transactionScope.Complete();

                        return Ok(new ResponseOKHandler<string>("Registration successful, Waiting for Admin Approval"));
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new ResponseErrorHandler
                        {
                            Code = StatusCodes.Status400BadRequest,
                            Status = HttpStatusCode.BadRequest.ToString(),
                            Message = "Registration failed. " + ex.Message
                        });
                    }
                }
            }

            return BadRequest(new ResponseErrorHandler
            {
                Code = StatusCodes.Status400BadRequest,
                Status = HttpStatusCode.BadRequest.ToString(),
                Message = "Invalid request data."
            });
        }

        [HttpGet("allCompany-waitingList")]
        public IActionResult GetAllCompanyDetailsWaiting()
        {
            try
            {
                var companies = _companyRepository.GetAll();
                var accounts = _accountRepository.GetAll();
                var vendors = _vendorRepository.GetAll();

                var clientDetails = (from com in companies
                                     join ven in vendors on com.Guid equals ven.Guid
                                     join acc in accounts on com.Guid equals acc.Guid
                                     where acc.Status == StatusAccount.Requested
                                     select new CompanyDetailDto
                                     {
                                         CompanyGuid = com.Guid,
                                         NameCompany = com.Name,
                                         Address = com.Address,
                                         Email = com.Email,
                                         Foto = com.Foto,
                                         PhoneNumber = com.PhoneNumber,
                                         StatusAccount = acc.Status.ToString(),
                                         StatusVendor = ven.StatusVendor.ToString(),
                                         VendorGuid = ven.Guid,
                                         BidangUsaha = ven.BidangUsaha,
                                         JenisPerusahaan = ven.JenisPerusahaan,

                                     }).ToList();



                return Ok(new ResponseOKHandler<IEnumerable<CompanyDetailDto>>(clientDetails));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Failed to retrieve client details. " + ex.Message
                });
            }
        }

        [HttpGet("vendor-DetailsWaiting")]
        public IActionResult GetAllVendorWaiting()
        {
            try
            {
                var companies = _companyRepository.GetAll();
                var accounts = _accountRepository.GetAll();
                var vendors = _vendorRepository.GetAll();

                var clientDetails = (from com in companies
                                     join ven in vendors on com.Guid equals ven.Guid
                                     join acc in accounts on com.Guid equals acc.Guid
                                     where acc.Status == StatusAccount.Approved && ven.StatusVendor == StatusVendor.waiting
                                     select new CompanyDetailDto
                                     {
                                         CompanyGuid = com.Guid,
                                         NameCompany = com.Name,
                                         Address = com.Address,
                                         Email = com.Email,
                                         Foto = com.Foto,
                                         PhoneNumber = com.PhoneNumber,
                                         StatusAccount = acc.Status.ToString(),
                                         StatusVendor = ven.StatusVendor.ToString(),
                                         VendorGuid = ven.Guid,
                                         BidangUsaha = ven.BidangUsaha,
                                         JenisPerusahaan = ven.JenisPerusahaan,

                                     }).ToList();



                return Ok(new ResponseOKHandler<IEnumerable<CompanyDetailDto>>(clientDetails));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Failed to retrieve client details. " + ex.Message
                });
            }
        }

        [HttpGet("company-ApproveByAdmin")]
        public IActionResult GetCompanyApproveByAdmin()
        {
            try
            {

                var companies = _companyRepository.GetAll();
                var accounts = _accountRepository.GetAll();
                var vendors = _vendorRepository.GetAll();

                var clientDetails = (from com in companies
                                     join ven in vendors on com.Guid equals ven.Guid
                                     join acc in accounts on com.Guid equals acc.Guid
                                     where acc.Status == StatusAccount.Approved && ven.StatusVendor == StatusVendor.approvedByAdmin
                                     select new CompanyDetailDto
                                     {
                                         CompanyGuid = com.Guid,
                                         NameCompany = com.Name,
                                         Address = com.Address,
                                         Email = com.Email,
                                         Foto = com.Foto,
                                         PhoneNumber = com.PhoneNumber,
                                         StatusAccount = acc.Status.ToString(),
                                         StatusVendor = ven.StatusVendor.ToString(),
                                         VendorGuid = ven.Guid,
                                         BidangUsaha = ven.BidangUsaha,
                                         JenisPerusahaan = ven.JenisPerusahaan,

                                     }).ToList();

                return Ok(new ResponseOKHandler<IEnumerable<CompanyDetailDto>>(clientDetails));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = HttpStatusCode.BadRequest.ToString(),
                    Message = "Failed to retrieve client details. " + ex.Message
                });
            }
        }
    }

}