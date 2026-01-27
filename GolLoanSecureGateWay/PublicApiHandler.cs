
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GoldLoanSecureGateWay
{
    public class PublicApiHandler
    {
        private readonly HttpClient _httpClient;
        public PublicApiHandler(IConfiguration configuration)
        {
            var baseUrl = configuration["BaseUrlGoldLoan"];
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
        #region Base64 Encode and Decode
        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedString)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedString);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        #endregion

        #region CALL POST METHOD
        public async Task<string> PostDataToApiWithAuthorization(string data, string authToken, string endPoint,string baseUrl)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var content = new StringContent(data, Encoding.UTF8, "application/json");

            //HttpResponseMessage response = await _httpClient.PostAsync(new Uri(_httpClient.BaseAddress, endPoint), content);
            HttpResponseMessage response = await _httpClient.PostAsync(endPoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                string c = await response.Content.ReadAsStringAsync();
                return c;
            }
        }
        #endregion

        #region CALL GET METHOD
        public async Task<string> GetDataFromApiWithAuthorization(string authToken, string endPoint, string baseUrl)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            HttpResponseMessage response = await _httpClient.GetAsync(endPoint);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return errorContent;
            }
        }
        #endregion

        #region CALL PUT METHOD
        public async Task<string> PutDataToApiWithAuthorization(string data,string authToken,string endPoint, string baseUrl)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(endPoint, content);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                return errorContent;
            }
        }

        #endregion

    }
}
