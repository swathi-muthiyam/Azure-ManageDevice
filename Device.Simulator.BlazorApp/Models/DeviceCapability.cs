using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Device.Simulator.BlazorApp.Models
{
    public class DeviceCapability
    {
        public string temperature { get; set; }
        public string humidity { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
}
