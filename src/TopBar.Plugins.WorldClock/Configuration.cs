using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBar.Plugins.WorldClock
{
    public class Configuration
    {
        IEnumerable<ClockConfiguration> Clocks { get;}
    }

    public class ClockConfiguration
    {
        public string Title { get; set; }
        public string TimeZoneId { get; set; }
        public bool Active { get; set; }
    }
}
