using System;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SnakeHost.Storage
{
    public class JsonStorage<T> where T : new()
    {
        public JsonStorage([NotNull] string fileName)
        {
            _fileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public void Write(T value)
        {
            var storageJson = JsonConvert.SerializeObject(value, Formatting.Indented);
            File.WriteAllText(_fileName, storageJson);
        }

        public T Read()
        {
            if (!File.Exists(_fileName))
            {
                return new T();
            }

            var storageJson = File.ReadAllText(_fileName);
            return JsonConvert.DeserializeObject<T>(storageJson);
        }

        private readonly string _fileName;
    }
}