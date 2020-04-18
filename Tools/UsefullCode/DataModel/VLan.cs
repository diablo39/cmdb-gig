using System;
using System.Net;

namespace UsefullCode
{
    public class VLan
    {
        private IPNetwork _ipnetwork;
        private IPAddressCollection _ipAddresses;

        public string Name { get; set; }

        public string Code { get { return "VLAN:" + Env + Name; } }

        public string Alias { get { return Env + Name; } }

        public string Cidr { get; }

        public string Description { get { return "Vlan for some mysterious machines"; } }

        public string Env { get; set; }

        public VLan(string cidr)
        {
            Cidr = cidr;
            _ipnetwork = IPNetwork.Parse(Cidr);

            _ipAddresses = _ipnetwork.ListIPAddress(FilterEnum.Usable);
        }

        public MachineNetworkInterface CreateNetworkInterface()
        {
            var result = new MachineNetworkInterface();
            result.Name = "ens302";
            result.Ipv4 = _ipAddresses.MoveNext() ? _ipAddresses.Current.ToString() : throw new Exception("Out of ip addresses");

            result.NetMask = _ipnetwork.Netmask.ToString();

            return result;
        }
    }
}
