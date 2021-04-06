// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroundSensorDevice.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Trac.Core.ADT.Common.Constants;

namespace Trac.Core.ADT.Common.Models
{
    public class GroundSensorDevice
    {
        public string IdDef { get; } = "dtmi:com:bosch:trac360:sm:parking:floor:slot:groundSensor;1";
        public string DeviceId { get; set; }
        public string ETag { get; set; }
        public DeviceStatus Status { get; set; }
    }
}
