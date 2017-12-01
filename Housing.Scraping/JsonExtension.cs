using Newtonsoft.Json.Linq;
using System;

namespace Housing.Scraping
{
    public static class JsonExtension
    {
        public static int GetInt32(this JToken token, object key)
        {
            var result = GetNullableInt32(token, key);

            if (!result.HasValue)
                throw new InvalidCastException();
            else
                return result.Value;
        }
        public static int? GetNullableInt32(this JToken token, object key)
        {
            var value = token[key]?.Value<string>();
            if (value == null)
                return null;

            value = value.Replace(".", "");

            if (int.TryParse(value, out int result))
                return result;
            else
                return null;
        }
    }
}
