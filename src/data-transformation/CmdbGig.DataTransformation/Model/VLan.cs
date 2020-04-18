using System;
using System.Collections.Generic;
using System.Text;

namespace CmdbGig.DataTransformation.Model
{
    class VLan
    {


        public string Name { get; set; }

        public string Code { get; set; }

        public string Alias { get; set; }

        public string Cidr { get; private set; }

        public string Description { get; set; }

        public string Env { get; set; }

    }
}
