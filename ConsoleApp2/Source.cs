using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmadataAPIClient
{
    public class Source
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public bool isCentral { get; set; }
        public DateTime? registered { get; set; }
    }
}
