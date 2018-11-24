using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SnakeHost
{
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T value)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}