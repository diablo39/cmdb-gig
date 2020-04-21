using System;
using System.Collections.Generic;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace UsefullCode
{
    public class Machine
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "env")]
        public string Env { get; set; }

        [YamlMember(Alias = "description")]
        public string Description { get; set; }

        [YamlMember(Alias = "vcpu")]
        public int vcpu { get; set; }

        [YamlMember(Alias = "memory")]
        public int memory { get; set; }

        [YamlMember(Alias = "operating-system-class")]
        public string OperatingSystemClass { get; set; }

        [YamlMember(Alias = "operating-system-distribution")]
        public string OperatingSystemDistribution { get; set; }

        [YamlMember(Alias = "operating-system-version")]
        public string OperatingSystemVersion { get; set; }

        [YamlMember(Alias = "fqdn")]
        public string FQDN { get; set; }

        public string Template { get; set; }

        [YamlMember(Alias = "network-interfaces")]
        public List<MachineNetworkInterface> NetworkInterfaces { get; private set; } = new List<MachineNetworkInterface>();

        [YamlMember(Alias = "data-volumes")]
        public List<MachineDataVolume> DataVolumes { get; private set; } = new List<MachineDataVolume>();


    }

    public class MachineNetworkInterface
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "ipv4-address")]
        public string Ipv4 { get; set; }

        [YamlMember(Alias = "ipv4-netmask")]
        public string NetMask { get; set; }

        [YamlMember(Alias = "ipv4-network")]
        public string Network { get; set; }

    }

    public class MachineDataVolume
    {
        [YamlMember(Alias = "device")]
        public string Device { get; set; }

        [YamlMember(Alias = "size")]
        public string Size { get; set; }

        [YamlMember(Alias = "mount")]
        public string Mount { get; set; }

        [YamlMember(Alias = "fstype")]
        public string FsType { get; set; }
    }
}
