using System.IO;
using System.Text.Json;

namespace SE2.Utils
{
    public class JsonSave
    {
        public static void Save<T>(T obj, string file) => File.WriteAllText(file, JsonSerializer.Serialize(obj));
        public static T Read<T>(string file) => JsonSerializer.Deserialize<T>(File.ReadAllText(file));
    }
}
