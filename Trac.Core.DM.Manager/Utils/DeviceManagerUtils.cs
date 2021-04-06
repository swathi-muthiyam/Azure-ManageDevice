// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeviceManagerUtils.cs" company=" "> 
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
using System.Linq;
using System.Threading.Tasks;

namespace Trac.Core.DM.Manager.Utils
{
    public class DeviceManagerUtils
    {
        /// <summary>
        /// Operation Status
        /// </summary>
        public enum OperationStatus
        {
            /// <summary>
            /// The success
            /// </summary>
            Success,

            /// <summary>
            /// The failure
            /// </summary>
            Failure,

            /// <summary>
            /// The device not found
            /// </summary>
            DeviceNotFound,

            /// <summary>
            /// The device already exists
            /// </summary>
            DeviceAlreadyExists,

            /// <summary>
            /// The invalid error
            /// </summary>
            InvalidError
        }


        
    }
}
