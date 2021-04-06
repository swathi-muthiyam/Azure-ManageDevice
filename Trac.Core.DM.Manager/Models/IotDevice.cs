// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IotDevices.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Trac.Core.DM.Manager.Models
{
    /// <summary>
    /// IoT Hub Device
    /// </summary>
    public class IotDevice
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Required(ErrorMessage = "Device Id is required")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        /// <value>
        /// The authentication.
        /// </value>
        public string Auth { get; set; }

        /// <summary>
        /// Gets or sets the C2DMessageCount.
        /// </summary>
        /// <value>
        /// The C2D Message Count.
        /// </value>
        public int C2DMessageCount { get; set; }

        /// <summary>
        /// Gets or sets the Connection State.
        /// </summary>
        /// <value>
        /// The Connection State.
        /// </value>
        public DeviceConnectionState ConnectionState { get; set; }

        /// <summary>
        /// Gets or sets the Connection State Updated Time.
        /// </summary>
        /// <value>
        /// The Connection State Updated Time.
        /// </value>
        public DateTime ConnectionStateUpdatedTime { get; set; }

        //[Required(ErrorMessage = "Tenent is Required!")]
        /// <summary>
        /// Gets or sets the ETag.
        /// </summary>
        /// <value>
        /// The ETag.
        /// </value>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the Generation Id.
        /// </summary>
        /// <value>
        /// The Generation Id.
        /// </value>
        public string GenerationId { get; set; }

        /// <summary>
        /// Gets or sets the Last Activity Time
        /// </summary>
        /// <value>
        /// The Last Activity Time.
        /// </value>
        public DateTime LastActivityTime { get; set; }

        /// <summary>
        /// Gets or sets the Scope
        /// </summary>
        /// <value>
        /// The Scope.
        /// </value>
        public string Scope { get; set; }

        /// <summary>
        /// Gets or sets the Status
        /// </summary>
        /// <value>
        /// The Status.
        /// </value>
        public DeviceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the Status Reason
        /// </summary>
        /// <value>
        /// The Status Reason.
        /// </value>
        public string StatusReason { get; set; }

        /// <summary>
        /// Gets or sets the Status Updated Time
        /// </summary>
        /// <value>
        /// The Status Updated Time.
        /// </value>
        public DateTime StatusUpdatedTime { get; set; }

        /// <summary>
        /// Gets or sets the Module Id
        /// </summary>
        /// <value>
        /// The Module Id.
        /// </value>
        public string ModuleId { get; set; }

        public TrackingAccess TrackingAccess { get; set; }

        /// <summary>
        /// Gets or sets the Space Id - this is to map space and device
        /// </summary>
        /// <value>
        /// The Space Id.
        /// </value>
        public string SpaceId { get; set; }

        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the Payload(Additional or Optional) data to be sent to the service
        /// </summary>
        /// <value>
        /// The Payload.
        /// </value>
        public Payload Payload { get; set; }

        public string iothubConnection { get; set; }
    }

    public class Payload
    {
        //[Required(ErrorMessage = "ModelId is required")]
        public string modelId { get; set; }

        public Dictionary<string, string> content { get; set; }
    }

    public class TrackingAccess
    {
        public bool IsUpdate { get; set; }

        public bool AllowTracking { get; set; }
    }
}
