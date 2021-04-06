// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICosmosSQLService.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Trac.Cosmos.FuncApp.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Trac.Cosmos.FuncApp.Interfaces
{
    public interface ICosmosSQLService
    {
        //CosmosClient client
        //{
        //    get;
        //    set;
        //}
        //string databaseName
        //{
        //    get;
        //}
        //string containerName
        //{
        //    get;
        //}
        //Task<IEnumerable<DeviceTelemetry>> GetTelemetriesAsync(string query);
        //Task<DeviceTelemetry> GetTelemetryAsync(string id);
        Task AddTelemetryAsync(DeviceTelemetry Telemetry);
        //Task UpdateTelemetryAsync(string id, DeviceTelemetry Telemetry);
        //Task DeleteTelemetryAsync(string id);
        
    }
}
