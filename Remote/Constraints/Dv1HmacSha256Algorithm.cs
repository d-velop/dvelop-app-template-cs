using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Dvelop.Remote.Constraints
{
    public static class Dv1HmacSha256Algorithm
    {
        public static async Task<string> CalculateSignature(HttpRequest request, string appSecret)
        {
            request.EnableBuffering();
            var reader = new StreamReader(request.Body, Encoding.UTF8, false, 1024*128, true);
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            string signatureHeaders = request.Headers["x-dv-signature-headers"];
            if (signatureHeaders == null)
            {
                return null;
            }

            var headers = signatureHeaders.Split(',');
            Array.Sort(headers, string.Compare);
            var enumerable = headers.Select(header => $"{header}:{request.Headers[header]}".ToLowerInvariant());
            
            var normalizedHeaders = string.Join('\n', enumerable.ToArray());

            var httpVerb = request.Method;
            string resourcePath = request.Path;
            var queryString = request.QueryString.Value.TrimStart('?');
            
            var payload = Sha256(body);

            var normalizedRequest = $"{httpVerb}\n{resourcePath}\n{queryString}\n{normalizedHeaders}\n\n{payload}";
            var requestHash = Sha256(normalizedRequest);

            var signatureHash = HmacSha256(Convert.FromBase64String(appSecret), requestHash);

            return signatureHash;
        }

        private static string Sha256(string input)
        {
            return BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
        }

        private static string HmacSha256(byte[] key, string input)
        {
            var sha = new HMACSHA256(key);
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLower();
        }
    }
}