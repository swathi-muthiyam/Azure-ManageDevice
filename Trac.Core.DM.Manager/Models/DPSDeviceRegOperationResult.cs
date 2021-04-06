using Microsoft.Azure.Devices.Provisioning.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Core.DM.Manager.Models
{
    public class DPSDeviceRegOperationResult
    {
        /// <summary>
        /// Gets or sets Iot Device Registration Result
        /// </summary>
        /// <value>
        /// The Iot Device Registration Result
        /// </value>
        public DeviceRegistrationResult DeviceRegResult { get; set; }

        /// <summary>
        /// Gets or sets Iot Device Key
        /// </summary>
        /// <value>
        /// The Iot Device key
        /// </value>
        public byte[] DeviceKey { get; set; }

        /// <summary>
        /// Gets or sets Message
        /// </summary>
        /// <value>
        /// The Message
        /// </value>
        public string Message { get; set; }

        //Status
    }
}
