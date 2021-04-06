// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ADTClientManager.cs" company=" "> 
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
using Azure.DigitalTwins.Core;
using Azure.DigitalTwins.Core.Serialization;
using Azure.Identity;
using Trac.Core.ADT.Manager.Constants;
using Trac.Core.ADT.Manager.Interfaces;
using Trac.Core.Common.Configuration;
using Trac.Core.Common.Configuration.Models;
using Dasync.Collections;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trac.Core.ADT.Manager.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Trac.Core.DigitalTwin.Manager
{
    public class ADTClientManager : IADTClientManager
    {
        private DigitalTwinsClient _digitalTwinsClient;

        public ADTClientManager()
        {
            if (_digitalTwinsClient == null)
            {               
                var adtSecrets = KeyVaultStoreManager.keyVaultStoreManagerInstance.GetValueFromKeyVault();
                var credentials = new ClientSecretCredential(adtSecrets[ADTConstants.tenentId], adtSecrets[ADTConstants.clientId], adtSecrets[ADTConstants.clientSecret]);
                this._digitalTwinsClient = new DigitalTwinsClient(new Uri(adtSecrets[ADTConstants.instanceUrl]), credentials);
            }
        }
        public ADTClientManager(DigitalTwinsClient digitalTwinsClient)
        {
            this._digitalTwinsClient = digitalTwinsClient;
        }

        public async Task<Response<string>> GetDigitalTwin(string digitalTwinId)
        {
            if (string.IsNullOrWhiteSpace(digitalTwinId))
            {
                throw new ArgumentNullException("DigitalTwinId can't be null or empty");
            }
            Response<string> res = null;
            try
            {
                res = await _digitalTwinsClient.GetDigitalTwinAsync(digitalTwinId);
            }
            catch (Exception)
            {
                return null;
            }

            return res;
        }

        public async Task<string> CreateDigitalTwin(BasicDigitalTwin digitalTwin)
        {
            Response<string> response = null;
            try
            {
                response = await _digitalTwinsClient.CreateDigitalTwinAsync($"{digitalTwin.Id}",
                    JsonSerializer.Serialize(digitalTwin));
                Console.WriteLine($"Created twin: {digitalTwin.Id}");

            }
            catch (RequestFailedException rex)
            {
                Console.WriteLine($"Create twin error: {rex.Status}:{rex.Message}");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Create twin error: {e.Message}");
            }

            return response.Value;
        }

        public async Task<List<string>> CreateDigitalTwin(List<BasicDigitalTwin> digitalTwins)
        {
            ConcurrentBag<string> resultCollection = new ConcurrentBag<string>();

            try
            {
                await digitalTwins.ParallelForEachAsync(async twin =>
                {
                    var response = await CreateDigitalTwin(twin);
                    resultCollection.Add(response);
                }, maxDegreeOfParallelism: 10);
            }
            catch (RequestFailedException rex)
            {
                Console.WriteLine($"Create twin error: {rex.Status}:{rex.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Create twin error: {e.Message}");
            }

            return resultCollection.ToList();
        }

        public async Task UpdateTwinProperty(string twinId, string propertyPath, object value)
        {
            // If the twin does not exist, this will log an error
            try
            {
                // Update twin property
                UpdateOperationsUtility uou = new UpdateOperationsUtility();
                uou.AppendAddOp(propertyPath, Convert.ToString(value));

                await _digitalTwinsClient.UpdateDigitalTwinAsync(twinId, uou.Serialize());
            }
            catch (RequestFailedException exc)
            {
                throw exc;// TODO Exception handling & Logging appropriately.
                //log.LogInformation($"*** Error:{exc.Status}/{exc.Message}");
            }
            catch (Exception ex)
            {
                throw ex;  // TODO Exception handling & Logging appropriately.
                //log.LogInformation($"*** Error: {ex.Message}");
            }
        }

        public async Task<Response<string>> CreateRelationship(CreateRelationship relationshipObj)
        {
            Response<string> response = null;
            Dictionary<string, object> body = new Dictionary<string, object>()
            {
                { "$targetId", relationshipObj.TargetTwinId },
                { "$relationshipName", relationshipObj.RelationshipName }
            };
            try
            {
                response = await _digitalTwinsClient.CreateRelationshipAsync(relationshipObj.SourceTwinId, relationshipObj.RelationshipId, JsonSerializer.Serialize(body));
                Console.WriteLine($"Relationship {relationshipObj.RelationshipId} of type {relationshipObj.RelationshipName} created successfully from {relationshipObj.SourceTwinId} to {relationshipObj.TargetTwinId}!");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error {e.Status}: {e.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return response;
        }

        public async Task<bool> DeleteRelationship(DeleteRelationship relationshipObj)
        {
            try
            {
                await _digitalTwinsClient.DeleteRelationshipAsync(relationshipObj.SourceTwinId, relationshipObj.RelationshipId);
                Console.WriteLine($"Relationship '{relationshipObj.RelationshipId}' for twin '{relationshipObj.SourceTwinId}' of type '{relationshipObj.RelationshipName}' deleted successfully!");
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine($"Error {e.Status}: {e.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                return false;
            }

            return true;
        }

        public async Task UpdateTwinProperty(string twinId, Dictionary<string, object> propertyPathValueObj)
        {
            // If the twin does not exist, this will log an error
            try
            {
                // Update twin properties
                UpdateOperationsUtility uou = new UpdateOperationsUtility();

                foreach (KeyValuePair<string, object> itemPropertyValue in propertyPathValueObj)
                {
                    uou.AppendAddOp(itemPropertyValue.Key, itemPropertyValue.Value);
                }

                await _digitalTwinsClient.UpdateDigitalTwinAsync(twinId, uou.Serialize());
            }
            catch (RequestFailedException exc)
            {
                throw exc;// TODO Exception handling & Logging appropriately.
                //log.LogInformation($"*** Error:{exc.Status}/{exc.Message}");
            }
            catch (Exception ex)
            {
                throw ex;  // TODO Exception handling & Logging appropriately.
                //log.LogInformation($"*** Error: {ex.Message}");
            }
        }

        public async Task<string> FindParent(string childTwinId, string relationshipName)
        {
            // Find parent using incoming relationships
            try
            {
                AsyncPageable<IncomingRelationship> rels = _digitalTwinsClient.GetIncomingRelationshipsAsync(childTwinId);

                await foreach (IncomingRelationship ie in rels)
                {
                    if (ie.RelationshipName.ToLower() == relationshipName.ToLower())
                        return (ie.SourceId);
                }
            }
            catch (RequestFailedException exc)
            {
                return exc.Message;
                //log.LogInformation($"*** Error in retrieving parent:{exc.Status}:{exc.Message}");
            }
            return null;
        }

        public async Task<IncomingRelationship> FindParentRelationship(string childTwinId, string relationshipName)
        {
            // Find parent using incoming relationships
            try
            {
                AsyncPageable<IncomingRelationship> rels = _digitalTwinsClient.GetIncomingRelationshipsAsync(childTwinId);

                await foreach (IncomingRelationship ie in rels)
                {
                    if (ie.RelationshipName.ToLower() == relationshipName.ToLower())
                        return ie;
                }
            }
            catch (RequestFailedException exc)
            {
                return null;
                //log.LogInformation($"*** Error in retrieving parent:{exc.Status}:{exc.Message}");
            }
            return null;
        }
    }
}
