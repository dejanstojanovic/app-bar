using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopBar.Plugins.TimeZones
{
    public class Configuration
    {
        public IEnumerable<ClockConfiguration> DatesAndTimes { get; set; }
    }

    public class ClockConfiguration
    {
        public string Title { get; set; }
        public string TimeZoneId { get; set; }

        public TimeZoneInfo TimeZone { get => TimeZoneInfo.GetSystemTimeZones().SingleOrDefault(z => z.Id.Equals(TimeZoneId, StringComparison.InvariantCultureIgnoreCase)); }
    }
}
