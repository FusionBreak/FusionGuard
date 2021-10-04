using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionGuard.Database
{
    public class Panic
    {
        public int Id { get; set; }
        public User? User { get; set; }
        public DateTime Beginn { get; set; }
        public DateTime End { get; set; }
    }
}
