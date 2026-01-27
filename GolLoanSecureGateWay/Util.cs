using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Caching;

namespace GoldLoanSecureGateWay
{
    public class Util
    {
        MemoryCache tokenCache = MemoryCache.Default;
        #region TokenValidation
        public  string ValidateToken(string token)
        {
            try
            {
                //string authorizationHeader = token.Length > 10
                //? token.Remove(token.Length - 28)
                //: string.Empty; 
                string authorizationHeader = token;
                string cacheKey = "token_" + token;
                string key = authorizationHeader.Substring(0, Math.Min(authorizationHeader.Length, 64));
                string ts = Base64Decode(authorizationHeader.Substring(64));
                string hash = Sha256("100939@777#314J");
                if (key != hash)
                {
                    return "Token Mismatch.";
                }
                //if (tokenCache.Contains(cacheKey))
                //{
                //    return "Duplicate Token."; // Token already used
                //}
                if (IsWithin30Seconds(ts))
                {
                    tokenCache.Add(cacheKey, true, DateTimeOffset.Now.AddMinutes(3));
                    return "Valid";
                }
                else
                {
                    return "Token Expired.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }


        }
        public static string Base64Decode(string base64EncodedString)
        {
            byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedString);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string Sha256(string plainText)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // Convert the hash bytes to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }
        public static bool IsWithin30Seconds(string dateTimeString)
        {
            string TimeOut = "300"; //It is not advised to hardcode the time.

            if (DateTime.TryParseExact(dateTimeString, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
            {

                TimeSpan difference = DateTime.Now - parsedDateTime;

                return difference.TotalSeconds < Convert.ToInt32(TimeOut);
            }
            else
            {

                return false;
            }
        }
        #endregion
    }
}
