// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobExtension.cs" company=" "> 
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

namespace Trac.BulkDeviceManagement.FuncApp.Extensions
{
    public static class BlobExtension
    {
        /// <summary>
        /// Failed Container Name
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static string FailedContainerName(this string container)
        {
            return container + "-Failed-" + DateTime.Now.ToString("ddmmyyyyhhmmss") + ".txt";
        }

        /// <summary>
        /// Success Container Name
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static string SuccessContainerName(this string container)
        {
            return container + "-success-" + DateTime.Now.ToString("ddmmyyyyhhmmss") + ".txt";
        }
    }
}
