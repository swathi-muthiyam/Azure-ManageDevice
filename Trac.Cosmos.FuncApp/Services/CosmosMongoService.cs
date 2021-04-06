// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CosmosMongoService.cs" company=" "> 
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
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace Trac.Cosmos.FuncApp.Services
{
    public class CosmosMongoService : ICosmosMongoService
    {
        private MongoClient _mongoClient;
        private string _databaseName;
        public string _collectionName;
        public CosmosMongoService(MongoClient mongoClient, string databaseName)//, string collectionName)
        {
            _mongoClient = mongoClient;
            _databaseName = databaseName;
            // _collectionName = collectionName;

        }
        // Gets all Task items from the MongoDB server.        
        //public List<DeviceTelemetry> GetAllTasks()
        //{
        //    try
        //    {
        //        var collection = GetTasksCollection();
        //        return collection.Find(new BsonDocument()).ToList();
        //    }
        //    catch (MongoConnectionException)
        //    {
        //        return new List<MyTask>();
        //    }
        //}

        // Creates a Task and inserts it into the collection in MongoDB.
        public void CreateTask(MDeviceTelemetry deviceTelemetry)
        {            
            try
            {
                var database = _mongoClient.GetDatabase(_databaseName);
               //var collection = database.CreateCollectionAsync<MDeviceTelemetry>(deviceTelemetry.SystemProperty.DeviceId);
                
                var todoTaskCollection = database.GetCollection<MDeviceTelemetry>(deviceTelemetry.SystemProperty.DeviceId);
                todoTaskCollection.InsertOne(deviceTelemetry);
            }
            catch (MongoCommandException ex)
            {
                string msg = ex.Message;
            }
        }

        //private IMongoCollection<MyTask> GetTasksCollection()
        //{


        //    MongoIdentity identity = new MongoInternalIdentity(dbName, userName);
        //    MongoIdentityEvidence evidence = new PasswordEvidence(password);

        //    settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

        //    MongoClient client = new MongoClient(settings);
        //    var database = client.GetDatabase(dbName);
        //    var todoTaskCollection = database.GetCollection<MyTask>(collectionName);
        //    return todoTaskCollection;
        //}

     
    }
}
