using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace database_test
{
    internal class WagonStatus
    {
        public long identification { get; set; }
        public int deviceID {  get; set; }
        public DateTime timestamp { get; set; }
        public char direction { get; set; }
        public int speed { get; set; }
        public string spoor { get; set; }

    }
}
