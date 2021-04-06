using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Core.DM.Manager.Models
{
    public class EnrollmentModel
    {
        public string EnrollmentName { get; set; }
        public EnrollmentType EnrollmentType { get; set; }
        public string GlobalDeviceEndpoint { get; set; }
        public string IdScope { get; set; }
        public TransportType TransportType { get; set; }
        public GroupEnrollmentModel GroupEnrollmentModel { get; set; }
    }

    public enum EnrollmentType
    {
        IndividualEnrollment,
        GroupEnrollment
    }
}
