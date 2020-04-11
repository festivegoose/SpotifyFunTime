using System;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SpotifyFunTime.Application.Utilities;

namespace SpotifyFunTime.Application
{
    public class ContentCache : IContentCache
    {
        private const int CACHE_EXPIRATION_MINUTES = 10;

        private readonly IMemoryCache _cache;

        public ContentCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set(string key, object content)
        {
            var cacheKey = Base64Utility.Encode(key);
            var compressedContent = CompressContent(content);
            _cache.Set(cacheKey, compressedContent, TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES));
        }

        public bool TryGet<T>(string key, out T content)
        {
            var cacheKey = Base64Utility.Encode(key);

            if (!_cache.TryGetValue(cacheKey, out string compressedContent))
            {
                content = default(T);
                return false;
            }

            content = DecompressContent<T>(compressedContent);
            return true;
        }

        private string CompressContent(object content)
        {
            var serializedContent = JsonConvert.SerializeObject(content);
            var compressedContent = Base64Utility.Encode(serializedContent);

            return compressedContent;
        }

        private T DecompressContent<T>(string compressedContent)
        {
            var decodedContent = Base64Utility.Decode(compressedContent);
            var deserializedContent = JsonConvert.DeserializeObject<T>(decodedContent);

            return deserializedContent;
        }
    }

    public interface IContentCache
    {
        void Set(string key, object content);
        bool TryGet<T>(string key, out T content);
    }
}