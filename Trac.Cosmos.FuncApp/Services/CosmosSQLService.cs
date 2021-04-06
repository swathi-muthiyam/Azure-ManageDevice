// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CosmosSQLService.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Trac.Cosmos.FuncApp.Interfaces;
using Trac.Cosmos.FuncApp.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Container = Microsoft.Azure.Cosmos.Container;

namespace Trac.Cosmos.FuncApp.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CosmosSQLService : ICosmosSQLService
    {
        public Container _container;
        private CosmosClient _client;
        private string _databaseName;
        public string _containerName;
       

        public CosmosSQLService(CosmosClient client, string databaseName, string containerName)
        {
            this._client = client;
            this._databaseName = databaseName;
            this._containerName = containerName;
        }

        //public CosmosClient client { get => this._client; set => _client = _client; }
        //public string databaseName => this._databaseName;

        //public string containerName => this._containerName;

        public async Task AddTelemetryAsync(DeviceTelemetry Telemetry)
        {
            try
            {
                //var databaseCreationResult = _client.CreateDatabaseAsync(_databaseName ).Result;
                //Console.WriteLine("The database Id created is: " + databaseCreationResult.Resource.Id);

                //var database = _client.GetDatabase(_databaseName);
                //this._container = await database.CreateContainerIfNotExistsAsync(Telemetry.deviceId, "/id");
                //ItemResponse<dynamic> response = await _container.CreateItemAsync<dynamic>(Telemetry, new PartitionKey("NewUser01"));


                #region
                DatabaseResponse databaseRes = await _client.CreateDatabaseIfNotExistsAsync(_databaseName);
                //await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

                var database = _client.GetDatabase(_databaseName);
                this._container = await database.CreateContainerIfNotExistsAsync(Telemetry.SystemProperty.DeviceId, "/id");
                //var telemetry = new DeviceTelemetry() { deviceId = Telemetry.deviceId, MessageBody = Telemetry.MessageBody };

                var res = await this._container.CreateItemAsync(Telemetry);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public Task DeleteTelemetryAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DeviceTelemetry>> GetTelemetriesAsync(string query)
        {
            throw new NotImplementedException();
        }

        public Task<DeviceTelemetry> GetTelemetryAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTelemetryAsync(string id, DeviceTelemetry Telemetry)
        {
            throw new NotImplementedException();
        }


    }
}
