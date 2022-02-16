using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SE2.Utils
{
    class YamlSave
    {
        public static void Save<T>(T obj, string file) => File.WriteAllText(file, new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Serialize(obj));
        public static T Read<T>(string file) => new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance) .Build().Deserialize<T>(File.ReadAllText(file));
    }
}
