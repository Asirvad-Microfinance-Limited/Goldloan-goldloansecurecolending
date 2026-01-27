using GoldLoanSecureGateWay.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace GoldLoanSecureGateWay.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]

    /// CREATED BY : 100939-JITHIN U R
    /// DATE : 07-AUG-2025
    /// DESCRIPTION : ROUNTING TO BACKEND APIS IS ASSISTED BY THIS API.
    public class SecureController : ControllerBase
    {
        private readonly AESEncryptDecrypt aESEncryptDecrypt;
        private readonly PublicApiHandler aPublicApiHandler;
        private readonly Util util;
        public SecureController(AESEncryptDecrypt aESEncryptDecrypt, PublicApiHandler aPublicApiHandler,Util util) 
        { 
            this.aESEncryptDecrypt = aESEncryptDecrypt;
            this.aPublicApiHandler = aPublicApiHandler;
            this.util = util;
        }
        #region POST METHOD
        [HttpPost]
        public IActionResult PostSecureData([FromBody]RequestData request)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues authToken))
                {
                    if (Request.Query.Count == 0)
                    {

                        string isTrue = util.ValidateToken((authToken.ToString().Replace("Bearer ", "")));
                        if (isTrue == "Valid")
                        {

                            request.requestToken = aESEncryptDecrypt.DecodeBase64(request.requestToken);
                            string resJson = aESEncryptDecrypt.DecryptStringAES(request.requestJson).ToString();
                            request.endPoint = aESEncryptDecrypt.DecryptStringAES(request.endPoint);
                            request.baseUrl = aESEncryptDecrypt.DecryptStringAES(request.baseUrl);

                            var response = aPublicApiHandler.PostDataToApiWithAuthorization(resJson, request.requestToken, request.endPoint, request.baseUrl).Result;

                            return Ok(new
                            {
                                secureResponse = aESEncryptDecrypt.EncryptStringAES(response),
                                hashString = aESEncryptDecrypt.Sha256(response.ToString()),
                                txn = aESEncryptDecrypt.EncryptStringAES($"SUCCESS~{DateTime.Now.ToString()}")
                            });
                        }
                        else
                        {
                            return new ObjectResult(new { secureResponse = "FAIL", Message = isTrue }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                    else
                    {
                        return new ObjectResult(new { secureResponse = "FAIL", Message = "Invalid Params" }) { StatusCode = StatusCodes.Status400BadRequest };
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GET METHOD
        [HttpPost]
        public IActionResult GetSecureData([FromBody] RequestData request)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues authToken))
                {
                    if (Request.Query.Count == 0)
                    {

                        string isTrue = util.ValidateToken((authToken.ToString().Replace("Bearer ", "")));
                        if (isTrue == "Valid")
                        {

                            request.requestToken = aESEncryptDecrypt.DecodeBase64(request.requestToken);
                            request.requestJson = aESEncryptDecrypt.DecryptStringAES(request.requestJson);
                            request.endPoint = aESEncryptDecrypt.DecryptStringAES(request.endPoint);
                            request.baseUrl = aESEncryptDecrypt.DecryptStringAES(request.baseUrl);

                            var response = aPublicApiHandler.GetDataFromApiWithAuthorization(request.requestToken, request.endPoint, request.baseUrl).Result;

                            return Ok(new
                            {
                                secureResponse = aESEncryptDecrypt.EncryptStringAES(response),
                                hashString = aESEncryptDecrypt.Sha256(response.ToString()),
                                txn = aESEncryptDecrypt.EncryptStringAES($"SUCCESS~{DateTime.Now.ToString()}")

                            });
                        }
                        else
                        {
                            return new ObjectResult(new { secureResponse = "FAIL", Message = isTrue }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                    else
                    {
                        return new ObjectResult(new { secureResponse = "FAIL", Message = "Invalid Params" }) { StatusCode = StatusCodes.Status400BadRequest };
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region PUT METHOD
        [HttpPost]
        public IActionResult PutSecureData([FromBody] RequestData request)
        {
            try
            {
                if (Request.Headers.TryGetValue("Authorization", out StringValues authToken))
                {
                    if (Request.Query.Count == 0)
                    {
                        string isTrue = util.ValidateToken((authToken.ToString().Replace("Bearer ", "")));
                        if (isTrue == "Valid")
                        {

                            request.requestToken = aESEncryptDecrypt.DecodeBase64(request.requestToken);
                            string resJson = aESEncryptDecrypt.DecryptStringAES(request.requestJson).ToString();
                            request.endPoint = aESEncryptDecrypt.DecryptStringAES(request.endPoint);
                            request.baseUrl = aESEncryptDecrypt.DecryptStringAES(request.baseUrl);

                            var response = aPublicApiHandler.PutDataToApiWithAuthorization(resJson, request.requestToken, request.endPoint, request.baseUrl).Result;

                            return Ok(new
                            {
                                secureResponse = aESEncryptDecrypt.EncryptStringAES(response),
                                hashString = aESEncryptDecrypt.Sha256(response.ToString()),
                                txn = aESEncryptDecrypt.EncryptStringAES($"SUCCESS~{DateTime.Now.ToString()}")
                            });
                        }
                        else
                        {
                            return new ObjectResult(new { secureResponse = "FAIL", Message = isTrue }) { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                    else
                    {
                        return new ObjectResult(new { secureResponse = "FAIL", Message = "Invalid Params" }) { StatusCode = StatusCodes.Status400BadRequest };
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

    }
}
