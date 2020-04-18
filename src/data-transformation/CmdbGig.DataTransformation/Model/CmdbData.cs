using System;
using System.Collections.Generic;
using System.Text;

namespace CmdbGig.DataTransformation.Model
{
    class CmdbData
    {
        public List<VLan> Vlans { get; } = new List<VLan>();

        public List<CmdbEnvironment> Env { get; } = new List<CmdbEnvironment>();

        public List<Machine> Machines { get; } = new List<Machine>();
    }
}
