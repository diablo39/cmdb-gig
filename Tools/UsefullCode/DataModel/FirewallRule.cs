using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace UsefullCode.DataModel
{
    public class FirewallRule
    {
        static Random rnd = new Random();

        static int _rfc = 1;

        [YamlMember(Alias = "rfc")]
        public string RFC { get; set; }

        [YamlMember(Alias = "protocol")]
        public string Protocol { get; set; }

        [YamlMember(Alias = "service")]
        public string Service { get; set; }

        [YamlMember(Alias = "description")]
        public string Description { get; set; }

        [YamlMember(Alias = "source-application")]
        public string SourceApplication { get; set; }

        [YamlMember(Alias = "source-env")]
        public string SourceEnv { get; set; }

        [YamlMember(Alias = "source-ipv4")]
        public string SourceIpv4 { get; set; }

        [YamlMember(Alias = "source-description")]
        public string SourceDescription { get; set; }

        [YamlMember(Alias = "source-port")]
        public string SourcePort { get; set; }

        [YamlMember(Alias = "destination-application")]
        public string DestinationApplication { get; set; }

        [YamlMember(Alias = "destination-env")]
        public string DestinationEnv { get; set; }

        [YamlMember(Alias = "destination-ipv4")]
        public string DestinationIpv4 { get; set; }

        [YamlMember(Alias = "destination-description")]
        public string DestinationDescription { get; set; }

        [YamlMember(Alias = "destination-port")]
        public string DestinationPort { get; set; }

        public static List<FirewallRule> Generate(List<Machine> machines, List<VLan> vlans, int count)
        {
            var result = new List<FirewallRule>();

            for (int i = 0; i < count; i++)
            {
                var source = GetParty(machines, vlans);
                var destination = GetParty(machines, vlans);

                var rule = new FirewallRule
                {
                    Description = "Rule is required for ...",
                    Protocol = "TCP",
                    RFC = GetRFC(),
                    Service = GetService(),
                    SourceApplication = GetSourceApplication(),
                    SourceDescription = "Some description of the source",
                    SourceEnv = source.env,
                    SourceIpv4 = source.ip,
                    SourcePort = "any",
                    DestinationApplication = GetSourceApplication(),
                    DestinationDescription = "Some description of the target",
                    DestinationEnv = destination.env,
                    DestinationIpv4 = destination.ip,
                    DestinationPort = GenerateTargetPort()
                };

                result.Add(rule);
            }

            return result;
        }

        private static (string env, string ip) GetParty(List<Machine> machines, List<VLan> vlans)
        {
            string env, ip;

            if (IsVlan())
            {
                var vlan = vlans[rnd.Next(vlans.Count - 1)];
                env = vlan.Env;
                ip = vlan.Cidr;
            }
            else
            {
                var machine = machines[rnd.Next(machines.Count - 1)];
                env = machine.Env;
                ip = machine.NetworkInterfaces.First().Ipv4;
            }

            return (env, ip);
        }

        private static string GenerateTargetPort()
        {
            var result = new List<string>();
            var numerOfPorts = rnd.Next(1, 4);
            for (int i = 0; i < numerOfPorts; i++)
            {
                result.Add(rnd.Next(1, 62000).ToString());
            }

            return string.Join(",", result);
        }



        private static string GetRFC()
        {
            return "NET" + (_rfc++).ToString("00000000");
        }
        private static string GetService()
        {
            var services = new string[] {
                "HTTP",
                "SSH",
                "HTTP",
                "SSH",
                "MSSQL",
                "Kafka",
                "AMQP"
            };

            var service = services[rnd.Next(0, services.Length - 1)];

            return service;
        }

        private static bool IsVlan()
        {
            return rnd.Next() % 2 == 0;
        }

        private static string GetSourceApplication()
        {
            var applications = new string[] { 
                "CRM",
                "Redis",
                "Antyvirus",
                "MS SQL",
                "Oracle",
                "DNS",
                "Store",
                "DataStore",
                "S3",
                "MySQL",
                "Office365",
                "HR System",
                "Portal",
                "DataMart"
            
            };

            var result = applications[rnd.Next(applications.Length - 1)];

            return result;
        }

    }
}
