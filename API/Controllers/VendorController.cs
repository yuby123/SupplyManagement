using API.Contracts;
using API.DTOs.Vendors;
using API.Models;
using API.Utilities.Handler;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorController(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

   
        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _vendorRepository.GetAll();

            if (!result.Any())
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Data Vendor Not Found"
                });
            }

            var data = result.Select(x => (VendorDto)x);

            return Ok(new ResponseOKHandler<IEnumerable<VendorDto>>(data));
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
            var result = _vendorRepository.GetByGuid(guid);

            if (result is null)
            {
                return NotFound(new ResponseErrorHandler
                {
                    Code = StatusCodes.Status404NotFound,
                    Status = HttpStatusCode.NotFound.ToString(),
                    Message = "Vendor Data with Specific GUID Not Found"
                });
            }

            return Ok(new ResponseOKHandler<VendorDto>((VendorDto)result));
        }

        [HttpPut]
        public IActionResult Update(VendorDto vendorDto)
        {
            try
            {
                var entity = _vendorRepository.GetByGuid(vendorDto.Guid);
                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Vendor with Specific GUID Not Found"
                    });
                }

                Vendor toUpdate = vendorDto;
                toUpdate.CreatedDate = entity.CreatedDate;
                _vendorRepository.Update(toUpdate);


                return Ok(new ResponseOKHandler<string>("Data Has Been Update !"));
            }
            catch (ExceptionHandler ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to update data",
                    Error = ex.Message
                });
            }
        }

        [HttpPut("PutStatusVendorByAdmin")]
        public IActionResult UpdateVendor(UpdateVendorDto vendorDto)
        {
            try
            {
                var entity = _vendorRepository.GetByGuid(vendorDto.Guid);
                if (entity is null)
                {
                    return NotFound(new ResponseErrorHandler
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = HttpStatusCode.NotFound.ToString(),
                        Message = "Vendor with Specific GUID Not Found"
                    });
                }

                Vendor toUpdate = vendorDto;
                toUpdate.BidangUsaha = entity.BidangUsaha;
                toUpdate.JenisPerusahaan = entity.JenisPerusahaan;
                toUpdate.CreatedDate = entity.CreatedDate;

                _vendorRepository.Update(toUpdate);

                return Ok(new ResponseOKHandler<string>("Data Has Been Update !"));
            }
            catch (ExceptionHandler ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Failed to update data",
                    Error = ex.Message
                });
            }
        }
    }
}
