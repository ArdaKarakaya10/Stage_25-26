using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assenteller
{
    internal class data
    {
        public string Received { get; set; }
        public string DevEUI { get; set; }
        public string Payload { get; set; }
        public string IO { get; set; }
        public string Temperature { get; set; }
        public string Battery { get; set; }
        public int CountUp { get; set; }
        public int CountDown { get; set; }
        public int Type { get; set; }
    }
}
