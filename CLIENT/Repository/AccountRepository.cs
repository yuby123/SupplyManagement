using API.DTOs.Accounts;

using API.DTOs.Tokens;
using API.Models;
using API.Utilities.Handler;
using CLIENT.Contract;
using CLIENT.Models;
using CLIENT.Repository;
using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace CLIENT.Repository
{
    public class AccountRepository : GeneralRepository<AccountDto, Guid>, IAccountRepository
    {
        public AccountRepository(string request = "Account/") : base(request)
        {

        }

        public async Task<ResponseOKHandler<ClaimsDto>> GetClaimsAsync(string token)
        {
            var requestUrl = "GetClaims/";

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.GetAsync(request + requestUrl + token);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var claims = JsonConvert.DeserializeObject<ResponseOKHandler<ClaimsDto>>(content);
                return claims;
            }
            else
            {
                throw new Exception("Failed to retrieve claims.");
            }
        }

       

        public async Task<object> Login(LoginDto login)
        {
            string jsonEntity = JsonConvert.SerializeObject(login);
            StringContent content = new StringContent(jsonEntity, Encoding.UTF8, "application/json");

            using (var response = await httpClient.PostAsync($"{request}login", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    // Respons status adalah 200 OK, proses respons seperti biasa
                    response.EnsureSuccessStatusCode();
                    string apiResponse = await response.Content.ReadAsStringAsync();

                    if (apiResponse != null)
                    {
                        var entityVM = JsonConvert.DeserializeObject<ResponseOKHandler<TokenDto>>(apiResponse);
                        return entityVM;
                    }
                    else
                    {
                        // Handle respons lainnya jika tidak ada token di dalam respons OK
                        return new ResponseErrorHandler
                        {
                            Code = StatusCodes.Status500InternalServerError,
                            Status = HttpStatusCode.InternalServerError.ToString(),
                            Message = "Terjadi kesalahan server. Silakan coba lagi nanti.",
                            Error = null
                        };
                    }
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Respons status adalah 400 Bad Request, baca pesan kesalahan validasi
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(apiResponse);
                    dynamic dynamicResponse = JsonConvert.DeserializeObject(apiResponse);

                    // Handle pesan kesalahan validasi di sini
                    if (dynamicResponse != null)
                    {
                        var errors = dynamicResponse.error.ToObject<List<string>>() ?? "";
                        var errorString = string.Join(", ", errors);

                        // Mengembalikan objek ResponseErrorHandler dengan kesalahan validasi
                        try
                        {
                            var errorResponse = new ResponseErrorHandler
                            {
                                Code = dynamicResponse.code,
                                Status = dynamicResponse.status,
                                Message = dynamicResponse.message,
                                Error = errorString
                            };
                            return errorResponse;
                        }
                        catch (Exception ex)
                        {
                            // Tampilkan pesan pengecualian ke konsol atau log
                            Console.WriteLine("Exception: " + ex.Message);
                        }
                    }
                }

                // Handle respons lainnya seperti sebelumnya
                return new ResponseErrorHandler
                {
                    Code = StatusCodes.Status500InternalServerError,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = "Terjadi kesalahan server. Silakan coba lagi nanti.",
                    Error = null
                };
            }
        }
    }
}
