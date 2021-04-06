// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DMConstants.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trac.Core.DM.Manager
{
    /// <summary>
    /// Device Management Constants
    /// </summary>
    public class DMConstants
    {
        public const string iothubconnectionstring = "DeviceManagement:Registration:IotHubConnectionString";
        public const string registrationTimeOutInSeconds = "DeviceManagement:Registration:registrationTimeOutInSeconds";
        public const string AzureBulkRegisterStorage = "DeviceManagement:Registration:AzureBulkRegisterStorage";
    }
}
