using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.IO;
using YamlDotNet.Serialization;
using Xunit.Abstractions;

namespace UsefullCode
{
    public class YamlToJson
    {
        private readonly ITestOutputHelper output;

        public YamlToJson(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Main()
        {
            // convert string/file to YAML object
            var r = new StringReader(@"
scalar: a scalar
sequence:
  - one
  - two
");
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(r);

            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);

            output.WriteLine(json);
        }
    }
}
