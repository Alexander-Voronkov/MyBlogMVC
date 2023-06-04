using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyBlog.Extentions
{
    public static class SessionExtensions
    {
        public static T? Get<T>(this ISession session, string key)
        {
            string? value = session.GetString(key);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
            };

            return value == null 
                ? default : 
                JsonSerializer.Deserialize<T>(value, options);
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
            };

            session.SetString(key, JsonSerializer.Serialize(value, options));
        }

        public static void Remove<T>(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
}
