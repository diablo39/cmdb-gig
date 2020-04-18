using CmdbGig.DataTransformation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CmdbGig.DataTransformation
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];

            var result = new CmdbData();
            var files = Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories);

            GetRecords<VLan>(files, "vlans.yaml").ToList().ForEach(e => result.Vlans.Add(e));

            GetRecords<CmdbEnvironment>(files, "env.yaml").ToList().ForEach(e => result.Env.Add(e));
            
            GetRecords<Machine>(files, "machines.yaml").ToList().ForEach(e => result.Machines.Add(e));

            var serializer = GetJsonSerializer();

            serializer.Serialize(Console.Out, result);

            Console.WriteLine();
        }

        private static IDeserializer GetDeserializer()
        {
            return new YamlDotNet.Serialization.DeserializerBuilder().WithNamingConvention(HyphenatedNamingConvention.Instance).Build();
        }

        private static ISerializer GetJsonSerializer()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .JsonCompatible()
                .Build();
            return serializer;
        }

        private static IEnumerable<T> GetRecords<T>(string[] files, string filePattern)
        {
            IDeserializer deserializer = GetDeserializer();

            var vlanFiles = files.Where(e => e.EndsWith(filePattern));


            foreach (var vlanFile in vlanFiles)
            {
                var vlanFileContent = File.ReadAllText(vlanFile);
                var vlans = deserializer.Deserialize<List<T>>(vlanFileContent);

                for (int i = 0; i < vlans.Count; i++)
                {
                    yield return vlans[i];
                }

            }
        }
    }
}
