using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UsefullCode.DataModel;
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

            var vlans = new List<VLan>
            {
                new VLan("192.168.11.0/27"){ Env = "DEV", Vlan="001" }, // 00
                new VLan("192.168.12.0/27"){ Env = "DEV", Vlan="002" }, // 01
                new VLan("192.168.13.0/27"){ Env = "DEV", Vlan="003" }, // 02
                new VLan("192.168.14.0/27"){ Env = "DEV", Vlan="004" }, // 03
                new VLan("192.168.15.0/27"){ Env = "DEV", Vlan="005" }, // 04
                new VLan("192.168.16.0/27"){ Env = "DEV", Vlan="006" }, // 05
                new VLan("192.168.21.0/27"){ Env = "TST", Vlan="011" }, // 06
                new VLan("192.168.22.0/27"){ Env = "TST", Vlan="012" },
                new VLan("192.168.23.0/27"){ Env = "TST", Vlan="013" },
                new VLan("192.168.24.0/27"){ Env = "TST", Vlan="014" },
                new VLan("192.168.25.0/27"){ Env = "TST", Vlan="015" },
                new VLan("192.168.26.0/27"){ Env = "TST", Vlan="016" },
                new VLan("192.168.31.0/27"){ Env = "PRD", Vlan="021" },
                new VLan("192.168.32.0/27"){ Env = "PRD", Vlan="022" },
                new VLan("192.168.33.0/27"){ Env = "PRD", Vlan="023" },
                new VLan("192.168.34.0/27"){ Env = "PRD", Vlan="024" },
                new VLan("192.168.35.0/27"){ Env = "PRD", Vlan="025" },
                new VLan("192.168.36.0/27"){ Env = "PRD", Vlan="026" }
            };

            for (int i = 0; i < 150; i++)
            {

                vlans.Add(new VLan($"192.168.{i + 100}.0/27") { Env = "WTF", Vlan = (100 + i).ToString("000") });
            }

            generator.AddVlans(vlans);

            generator.AddEnvironment("DEV", "Development");
            generator.AddEnvironment("TST", "Test");
            generator.AddEnvironment("PRD", "Production");
            generator.AddEnvironment("WTF", "Experimental");

            generator.AddMachines(30, "DEV-APP-SERVERS", "DEV", vlans.ToArray()[0..1], "Linux", "Ubuntu", 4, 32);
            generator.AddMachines(30, "DEV-DBS-SERVERS", "DEV", vlans.ToArray()[1..2], "Linux", "Ubuntu", 4, 32);
            generator.AddMachines(30, "DEV-RED-SERVERS", "DEV", vlans.ToArray()[2..3], "Linux", "Ubuntu", 4, 32);
            generator.AddMachines(30, "DEV-ANS-SERVERS", "DEV", vlans.ToArray()[3..4], "Linux", "Ubuntu", 4, 32);
            generator.AddMachines(30, "DEV-IDX-SERVERS", "DEV", vlans.ToArray()[4..5], "Linux", "Ubuntu", 4, 32);
            generator.AddMachines(30, "DEV-BBC-SERVERS", "DEV", vlans.ToArray()[5..6], "Linux", "Ubuntu", 4, 32);

            generator.AddMachines(30, "TST-APP-SERVERS", "TST", vlans.ToArray()[6..7], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "TST-DBS-SERVERS", "TST", vlans.ToArray()[7..8], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "TST-ANS-SERVERS", "TST", vlans.ToArray()[8..9], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "TST-IDX-SERVERS", "TST", vlans.ToArray()[9..10], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "TST-IDX-SERVERS", "TST", vlans.ToArray()[10..11], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "TST-BBC-SERVERS", "TST", vlans.ToArray()[11..12], "Linux", "Ubuntu", 8, 32);

            generator.AddMachines(30, "PRD-APP-SERVERS", "PRD", vlans.ToArray()[12..13], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "PRD-DBS-SERVERS", "PRD", vlans.ToArray()[13..14], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "PRD-ANS-SERVERS", "PRD", vlans.ToArray()[14..15], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "PRD-IDX-SERVERS", "PRD", vlans.ToArray()[15..16], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "PRD-IDX-SERVERS", "PRD", vlans.ToArray()[16..17], "Linux", "Ubuntu", 8, 32);
            generator.AddMachines(30, "PRD-BBC-SERVERS", "PRD", vlans.ToArray()[17..18], "Linux", "Ubuntu", 8, 32);

            generator.AddFirewallRules(FirewallRule.Generate(generator._machines, generator._vlans, 3000));

            generator.Generate(@"C:\sencha\cmdb\samples\generated");
        }
    }

    public class SampleDataGeneror
    {
        List<FirewallRule> _firewallRules = new List<FirewallRule>();

        List<Environment> _environments = new List<Environment>();
        
        public List<VLan> _vlans = new List<VLan>();

        public List<Machine> _machines = new List<Machine>();

        public void AddEnvironment(string name, string description = "")
        {
            _environments.Add(new Environment { Name = name, Description = description });
        }

        public void AddVlan(VLan vlan)
        {
            _vlans.Add(vlan);
        }

        public void AddVlans(IEnumerable<VLan> vlans)
        {
            _vlans.AddRange(vlans);
        }

        public void AddMachines(int count, string name, string env, VLan[] vlans, string operatingSystemClass, string operatingSystem, int vcpu, int memory, string systemVersion = "18.10")
        {
            for (int i = 0; i < count; i++)
            {

                var machine = new Machine
                {
                    Name = name + i.ToString("00"),
                    Env = env,
                    OperatingSystemDistribution = operatingSystem,
                    OperatingSystemClass = operatingSystemClass,
                    OperatingSystemVersion = systemVersion,
                    vcpu = vcpu,
                    memory = memory,
                    Description = "Server for tasks :)",
                    FQDN = name + i.ToString("00") + ".cmdb-gig.localdomain"
                };

                if (vlans != null)
                {
                    for (int j = 0; j < vlans.Length; j++)
                    {
                        var networkInterface = vlans[j].CreateNetworkInterface();
                        networkInterface.Name = networkInterface.Name.Substring(0, networkInterface.Name.Length - 2) + (j * 10 + 2).ToString();
                        machine.NetworkInterfaces.Add(networkInterface);
                    }
                }

                machine.DataVolumes.Add(new MachineDataVolume { Device="sda", FsType="ext4", Size="10GB", Mount="/" });
                machine.DataVolumes.Add(new MachineDataVolume { Device="sda", FsType="ext4", Size="10GB", Mount="/tmp" });
                machine.DataVolumes.Add(new MachineDataVolume { Device="sda", FsType="ext4", Size="10GB", Mount="/var" });
                machine.DataVolumes.Add(new MachineDataVolume { Device="sda", FsType="ext4", Size="10GB", Mount="/var/log" });
                machine.DataVolumes.Add(new MachineDataVolume { Device="sda", FsType="ext4", Size="10GB", Mount="/opt" });
                machine.DataVolumes.Add(new MachineDataVolume { Device="sda", FsType="ext4", Size="10GB", Mount="/data" });
                _machines.Add(machine);
            }
        }

        public void Generate(string path)
        {
            if (!path.EndsWith("/") || !path.EndsWith("\\"))
            {
                path = path + Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            SaveData(_environments, path, "env");

            SaveData(_vlans, path, "vlans");
            SaveData(_machines, path, "machines");
            SaveData(_firewallRules, path, "firewall-rules");
        }

        private ISerializer GetSerializer()
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .Build();
            return serializer;
        }

        private void SaveData<T>(T data, string path, string dataType)
        {
            var fileName = dataType + ".yaml";
            var document = new Dictionary<string, object>() { { dataType, data } };

            var serializer = GetSerializer();

            var yaml = serializer.Serialize(document);

            var filePath = Path.Combine(path, fileName);

            File.WriteAllText(filePath, yaml);
        }

        internal void AddFirewallRules(List<FirewallRule> list)
        {
            _firewallRules.AddRange(list);
        }
    }
}
