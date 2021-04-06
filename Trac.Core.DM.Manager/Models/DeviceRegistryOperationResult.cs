// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceRegistryOperationResult.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using static Trac.Core.DeviceManagement.API.Utilities.DeviceManagerUtils;

namespace Trac.Core.DM.Manager.Models
{
    public class DeviceRegistryOperationResult
    {
        /// <summary>
        /// Gets or sets the device registration.
        /// </summary>
        /// <value>
        /// The successful.
        /// </value>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public OperationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets Iot Devices
        /// </summary>
        /// <value>
        /// The Iot Device
        /// </value>
        public IEnumerable<IotDevice> IotDevicesData { get; set; }       

        /// <summary>
        /// Gets or sets Iot Device
        /// </summary>
        /// <value>
        /// The Iot Device
        /// </value>
        public IotDevice IotDeviceData { get; set; }
    }
}
