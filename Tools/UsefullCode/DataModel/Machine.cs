using System;
using System.Collections.Generic;

namespace UsefullCode
{
    public class Machine
    {
        public string Name { get; set; }

        public string Env { get; set; }

        public string Template { get; set; }

        public string Description { get; set; }

        public int vcpu { get; set; }

        public int memory { get; set; }

        public string operatingSystemClass { get; set; }

        public string operatingSystem { get; set; }

        public List<MachineNetworkInterface> NetworkInterfaces { get; private set; } = new List<MachineNetworkInterface>();
    }
}
