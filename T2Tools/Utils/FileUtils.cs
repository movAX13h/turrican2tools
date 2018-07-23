using System.IO;
using System.Runtime.Serialization.Json;

namespace T2Tools.Utils
{
    public class FileUtils
    {
        internal static T ObjectFromJsonFile<T>(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(stream);
                return instance;
            }
        }

        internal static void ObjectToJsonFile<T>(T obj, string filename)
        {
            if (File.Exists(filename)) File.Delete(filename);

            using (Stream stream = File.OpenWrite(filename))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, obj);
            }
        }
    }
}
