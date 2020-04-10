using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace UsefullCode
{
    public class SampleData
    {
        [Fact]
        public void GenerateSample()
        {
            var generator = new SampleDataGeneror();

            var vlans = new[]
            {
                new VLan("192.168.11.1/27"){ Env = "DEV", Name="001" }, // 00
                new VLan("192.168.12.1/27"){ Env = "DEV", Name="002" }, // 01
                new VLan("192.168.13.1/27"){ Env = "DEV", Name="003" }, // 02
                new VLan("192.168.14.1/27"){ Env = "DEV", Name="004" }, // 03
                new VLan("192.168.15.1/27"){ Env = "DEV", Name="005" }, // 04
                new VLan("192.168.16.1/27"){ Env = "DEV", Name="006" },
                new VLan("192.168.21.1/27"){ Env = "TST", Name="011" },
                new VLan("192.168.22.1/27"){ Env = "TST", Name="012" },
                new VLan("192.168.23.1/27"){ Env = "TST", Name="013" },
                new VLan("192.168.24.1/27"){ Env = "TST", Name="014" },
                new VLan("192.168.25.1/27"){ Env = "TST", Name="015" },
                new VLan("192.168.26.1/27"){ Env = "TST", Name="016" },
                new VLan("192.168.31.1/27"){ Env = "PRD", Name="021" },
                new VLan("192.168.32.1/27"){ Env = "PRD", Name="022" },
                new VLan("192.168.33.1/27"){ Env = "PRD", Name="023" },
                new VLan("192.168.34.1/27"){ Env = "PRD", Name="024" },
                new VLan("192.168.35.1/27"){ Env = "PRD", Name="025" },
                new VLan("192.168.36.1/27"){ Env = "PRD", Name="026" }
            };

            generator.AddVlans(vlans);

            generator.AddEnvironment("DEV", "Development");
            generator.AddEnvironment("TST", "Development");
            generator.AddEnvironment("PRD", "Development");

            generator.AddMachines(4, "DEV-APP-SERVERS", "DEV", vlans[0..2], "Linux", "Ubuntu 18.10", 4, 32);

            generator.AddMachines(8, "TST-APP-SERVERS", "TST", vlans[6..8], "Linux", "Ubuntu 18.10", 8, 32);

            generator.AddMachines(20, "PRD-APP-SERVERS", "PRD", vlans[12..14], "Linux", "Ubuntu 18.10", 8, 32);



            generator.Generate(@"C:\sencha\cmdb\samples\generated");
        }
    }

    public class SampleDataGeneror
    {
        List<Environment> _environments = new List<Environment>();
        List<VLan> _vlans = new List<VLan>();

        List<Machine> _machines = new List<Machine>();

        public void AddEnvironment(string name, string description = "")
        {
            _environments.Add(new Environment { Name = name, Description = description });
        }

        public void AddVlan(VLan vlan)
        {
            _vlans.Add(vlan);
        }

        public void AddVlans(VLan[] vlans)
        {
            _vlans.AddRange(vlans);
        }

        public void AddMachines(int count, string name, string env, VLan[] vlans, string operatingSystemClass, string operatingSystem, int vcpu, int memory)
        {
            for (int i = 0; i < count; i++)
            {

                var machine = new Machine
                {
                    Name = name + i.ToString("00"),
                    Env = env,
                    operatingSystem = operatingSystem,
                    operatingSystemClass = operatingSystemClass,
                    vcpu = vcpu,
                    memory = memory
                };

                if(vlans != null)
                {
                    for (int j = 0; j < vlans.Length; j++)
                    {
                        var networkInterface = vlans[j].CreateNetworkInterface();
                        networkInterface.Name = networkInterface.Name.Substring(0, networkInterface.Name .Length - 2) + (j*10+2).ToString();
                        machine.NetworkInterfaces.Add(networkInterface);
                    }
                }

                _machines.Add(machine);
            }
        }

        public void Generate(string path)
        {
            if(!path.EndsWith("/") || !path.EndsWith("\\"))
            {
                path = path + Path.DirectorySeparatorChar;
            }

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            SaveData(_environments, path, "env.yaml");

            SaveData(_vlans, path, "vlans.yaml");
            SaveData(_machines, path, "machines.yaml");
        }

        private ISerializer GetSerializer()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
            return serializer;
        }

        private void SaveData<T>(T data, string path, string fileName)
        {
            var serializer = GetSerializer();

            var yaml = serializer.Serialize(data);

            var filePath = Path.Combine(path, fileName);

            File.WriteAllText(filePath, yaml, Encoding.UTF8);
        }




    }
}
