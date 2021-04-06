// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company=" "> 
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
using Trac.Cosmos.FuncApp.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Authentication;
using System.Text;

[assembly: FunctionsStartup(typeof(Trac.Cosmos.FuncApp.Startup))]
namespace Trac.Cosmos.FuncApp
{
    public class Startup : FunctionsStartup
    {
        private string userName = "FILLME";
        private string host = "FILLME";
        private string password = "FILLME";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // As discussed during design discussion not generalizing the code as cosmos db can be replaced with time series db as we start working with live devices.
            //TODO : Once actual devices are onboarded, This code needs to be aligned with the actual telemetry data,
            builder.Services.AddSingleton<ICosmosSQLService>((s) =>
            {
                return (CosmosSQLService)InitializeCosmosSQLClientInstanceAsync();
            });

            //builder.Services.AddSingleton<ICosmosMongoService>((s) =>
            //{
            //    return (CosmosMongoService)InitializeCosmosMongoClientInstanceAsync();
            //});

        }

        private CosmosMongoService InitializeCosmosMongoClientInstanceAsync()
        {
            //MongoClientSettings settings = new MongoClientSettings();
            //settings.Server = new MongoServerAddress(host, 10255);
            //settings.UseSsl = true;
            //settings.SslSettings = new SslSettings();
            //settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            string databaseName = System.Environment.GetEnvironmentVariable("TelemetryDatabaseName", EnvironmentVariableTarget.Process);

            //MongoIdentity identity = new MongoInternalIdentity(databaseName, userName);
            //MongoIdentityEvidence evidence = new PasswordEvidence(password);

            //settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            //MongoClient client = new MongoClient(settings);

            string connectionString = System.Environment.GetEnvironmentVariable("MongoApiConnectionstring", EnvironmentVariableTarget.Process);
            MongoClient client = new MongoClient(connectionString);
            CosmosMongoService cms = new CosmosMongoService(client, databaseName);
            return cms;
        }

        private CosmosSQLService InitializeCosmosSQLClientInstanceAsync()
        {
            string databaseName = System.Environment.GetEnvironmentVariable("TelemetryDatabaseName", EnvironmentVariableTarget.Process);
            string containerName = System.Environment.GetEnvironmentVariable("TelemetryContainerName", EnvironmentVariableTarget.Process);
            string endpoint = System.Environment.GetEnvironmentVariable("TelemetryStorageEndpoint", EnvironmentVariableTarget.Process);
            string authkey = System.Environment.GetEnvironmentVariable("TelemetryStorageAuthKey", EnvironmentVariableTarget.Process);
            CosmosClient client = new CosmosClient(endpoint, authkey);
            CosmosSQLService cosmosDbService = new CosmosSQLService(client, databaseName, containerName);
             

            return cosmosDbService;
           // return new object { };
        }
    }
}
