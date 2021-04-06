using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Cosmos.FuncApp.Models
{
    public class DeviceTelemetryModel
    {

        public int messageId { get; set; }
        public string deviceId { get; set; }
        public int temperature { get; set; }
        public double humidity { get; set; }

    }
}
