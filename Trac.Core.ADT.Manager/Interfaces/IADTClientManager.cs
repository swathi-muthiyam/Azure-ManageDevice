// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IADTClientManager.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Azure;
using Azure.DigitalTwins.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.DigitalTwins.Core;
using Trac.Core.ADT.Manager.Models;

namespace Trac.Core.ADT.Manager.Interfaces
{
    public interface IADTClientManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="digitalTwinId"></param>
        /// <returns></returns>
        public Task<Response<string>> GetDigitalTwin(string digitalTwinId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digitalTwin"></param>
        /// <returns></returns>
        public Task<string> CreateDigitalTwin(BasicDigitalTwin digitalTwin);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="digitalTwins"></param>
        /// <returns></returns>
        public Task<List<string>> CreateDigitalTwin(List<BasicDigitalTwin> digitalTwins);

        public Task<Response<string>> CreateRelationship(CreateRelationship relationshipObj);

        public Task<bool> DeleteRelationship(DeleteRelationship relationshipObj);


        public Task UpdateTwinProperty(string twinId, string propertyPath, object value);

        public Task UpdateTwinProperty(string twinId, Dictionary<string, object> propertyPathValueObj);

        public Task<string> FindParent(string childTwinId, string relationshipName);

        /// <summary>
        /// Returns the IncomingRelationship which can be used for update/delete relationship
        /// </summary>
        /// <param name="digitalTwinId"></param>
        /// <param name="relationshipName"></param>
        /// <returns></returns>
        public Task<IncomingRelationship> FindParentRelationship(string childTwinId, string relationshipName);
    }
}
