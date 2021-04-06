using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Core.DM.Manager.Models
{
    public class StorageProperties
    {
        public string ContainerName { get; set; }

        public string BlobName { get; set; }

        public byte[] fileData { get; set; }
        
        public string fileMimeType { get; set; }
    }
}
