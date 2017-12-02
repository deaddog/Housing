using Housing.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Housing.Services.GoogleDirections
{
    public class CachedGoogleDirectionsRepository : IGoogleDirectionsRepository
    {
        private static readonly MD5 _md5 = MD5.Create();
        private readonly IGoogleDirectionsRepository _baseApi;
        private readonly string _cachePath;

        public CachedGoogleDirectionsRepository(IGoogleDirectionsRepository baseApi, string cachePath)
        {
            _baseApi = baseApi ?? throw new ArgumentNullException(nameof(baseApi));
            _cachePath = cachePath ?? throw new ArgumentNullException(nameof(cachePath));
        }

        public async Task<Distance> GetDistance(string origin, string destination)
        {
            var filepath = GetDirectionsFilepath(origin, destination);

            if (File.Exists(filepath))
                return JsonConvert.DeserializeObject<Distance>(File.ReadAllText(filepath));
            else
            {
                var response = await _baseApi.GetDistance(origin, destination);

                File.WriteAllText(filepath, JsonConvert.SerializeObject(response, Formatting.Indented));

                return response;
            }
        }

        private static string GetHashString(string input)
        {
            if (input == null || input.Length == 0)
                return null;

            StringBuilder sb = new StringBuilder();

            byte[] hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (var b in hash)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }
        private string GetDirectionsFilepath(string origin, string destination)
        {
            return Path.Combine(_cachePath, GetHashString(origin) + "_" + GetHashString(destination) + ".json");
        }
    }
}
