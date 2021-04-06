using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Core.DM.Manager.Models
{
    public class GroupEnrollmentModel
    {
        //public string EnrollmentGroupName { get; set; }
        public AssestationType AssestationType { get; set; }
        public bool isIotEdgeDevice { get; set; }
        public string? SymPrimaryKey { get; set; }
        public string? SymSecondaryKey { get; set; }
        // TODO : If Certificate is enabled , flow for certificate based assestation to be built.
        public bool IsEnabled { get; set; }

    }

    public enum AssestationType
    {
        Certificate,
        SymmetricKey
    }
    public enum CertificateType
    {
        CACertificate,
        IntermediateCertificate
    }
}
