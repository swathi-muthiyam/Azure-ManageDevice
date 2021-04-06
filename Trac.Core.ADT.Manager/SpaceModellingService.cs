// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpaceModellingService.cs" company=" "> 
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.DigitalTwins.Core;
using Azure.DigitalTwins.Core.Serialization;
using Azure.Identity;
using Trac.Core.ADT.Common.Constants;
using Trac.Core.ADT.Common.Models;
using Trac.Core.ADT.Common.ViewModels;
using Trac.Core.ADT.Manager.Interfaces;
using Trac.Core.ADT.Manager.Models;
using Trac.Core.Common.Configuration.Models;
using Trac.Core.DigitalTwin.Manager;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Trac.Core.ADT.Manager
{
    public class SpaceModellingService : ISpaceModelingService
    {
        private readonly ADTCredentials adtCredentials;
        private DigitalTwinsClient _digitalTwinsClient;
        private IADTClientManager _adtClientManager;
        private readonly Random _random = new Random();

        public SpaceModellingService(IOptions<ADTCredentials> adtCredentialsOptions)
        {
            this.adtCredentials = adtCredentialsOptions.Value;
            CreateAdtClient();
        }

        private void CreateAdtClient()
        {
            if (_digitalTwinsClient == null)
            {
                var credential = new ClientSecretCredential(adtCredentials.TenantId, adtCredentials.ClientId, adtCredentials.ClientSecret);
                _digitalTwinsClient = new DigitalTwinsClient(new Uri(adtCredentials.AdtInstanceUrl), credential);
                _adtClientManager = new ADTClientManager(_digitalTwinsClient);
            }
        }

        #region Insert/Update Spaces
        public async Task<bool> CreateSpace(BuildingVM buildingVm)
        {
            try
            {
                Building building = ConstructSpaceObject(buildingVm);

                BasicDigitalTwin buildingDigitalTwin = CreateBuildingDigitalTwin(building);
                await CreateDigitalTwin(new List<BasicDigitalTwin>() { buildingDigitalTwin }, "Building");
                //await _adtClientManager.CreateDigitalTwin(buildingDigitalTwin);

                // Building Id -> and Its floors DTs
                Dictionary<Guid, List<BasicDigitalTwin>> floorsDigitalTwins = new Dictionary<Guid, List<BasicDigitalTwin>>();
                List<BasicDigitalTwin> floorDTs = new List<BasicDigitalTwin>();

                foreach (Floor buildingFloor in building.Floors)
                {
                    var floorDTobj = CreateFloorDigitalTwin(buildingFloor);
                    floorDTs.Add(floorDTobj);
                }

                floorsDigitalTwins.Add(building.BuildingId, floorDTs);

                // Create DT for floors in DT instance
                foreach (KeyValuePair<Guid, List<BasicDigitalTwin>> floorDTitem in floorsDigitalTwins)
                {
                    await CreateDigitalTwin(floorDTitem.Value, "Floor");
                }

                // Floor Id -> and Its parking slots DTs
                Dictionary<Guid, List<BasicDigitalTwin>> parkingSlotDigitalTwin = new Dictionary<Guid, List<BasicDigitalTwin>>();

                foreach (Floor buildingFloor in building.Floors)
                {
                    List<BasicDigitalTwin> parkingSlotDTs = new List<BasicDigitalTwin>();

                    foreach (ParkingSlot parkingSlot in buildingFloor.ParkingSlots)
                    {
                        var parkingSlotDTobj = CreateParkingSlotDigitalTwin(parkingSlot);
                        parkingSlotDTs.Add(parkingSlotDTobj);
                    }

                    parkingSlotDigitalTwin.Add(buildingFloor.FloorId, parkingSlotDTs);
                }

                // Create DT for parking slots in DT instance
                foreach (KeyValuePair<Guid, List<BasicDigitalTwin>> floorsParkingSlotDTitem in parkingSlotDigitalTwin)
                {
                    await CreateDigitalTwin(floorsParkingSlotDTitem.Value, "ParkingSlot");
                }

                //Create relationships between building and floors and parking slots
                foreach (var floorItem in building.Floors)
                {
                    CreateRelationship relationshipObj = new CreateRelationship()
                    { SourceTwinId = $"Building-{building.BuildingId}", TargetTwinId = $"Floor-{floorItem.FloorId}", RelationshipName = building.Relationship, RelationshipId = $"bfrelationship{_random.Next()}" };

                    await CreateRelationship(relationshipObj);
                }

                // Added separately to create the relationship between floor and parkingslots after the previous operations completed
                foreach (var itemFloor in building.Floors)
                {
                    foreach (var itemParkingSlot in itemFloor.ParkingSlots)
                    {
                        CreateRelationship relationshipObj = new CreateRelationship()
                        { SourceTwinId = $"Floor-{itemFloor.FloorId}", TargetTwinId = $"ParkingSlot-{itemParkingSlot.SlotId}", RelationshipName = itemFloor.Relationship, RelationshipId = $"fprelationship{_random.Next()}" };

                        await CreateRelationship(relationshipObj);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateBuilding(BuildingVM building)
        {
            Dictionary<string, object> updateData = new Dictionary<string, object>();

            updateData.Add($"/{nameof(building.Name)}", building.Name);
            updateData.Add($"/{nameof(building.Address)}", building.Address);
            updateData.Add($"/{nameof(building.SpaceVertical)}", building.SpaceVertical.ToString());

            try
            {
                await _adtClientManager.UpdateTwinProperty($"Building-{building.BuildingId}", updateData);
            }
            catch (Exception)
            {
                return false;
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateFloor(FloorVM floor)
        {
            Dictionary<string, object> updateData = new Dictionary<string, object>();

            updateData.Add($"/{nameof(floor.FloorLevel)}", floor.FloorLevel);
            updateData.Add($"/{nameof(floor.FloorType)}", floor.FloorType.ToString());

            try
            {
                await _adtClientManager.UpdateTwinProperty($"Floor-{floor.FloorId}", updateData);
            }
            catch (Exception)
            {
                return false;
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateParkingSlot(ParkingSlotVM parkingSlot)
        {
            Dictionary<string, object> updateData = new Dictionary<string, object>();

            updateData.Add($"/{nameof(parkingSlot.SlotStatus)}", parkingSlot.SlotStatus.ToString());

            try
            {
                await _adtClientManager.UpdateTwinProperty($"ParkingSlot-{parkingSlot.SlotId}", updateData);
            }
            catch (Exception)
            {
                return false;
            }

            return await Task.FromResult(true);
        }

        private BasicDigitalTwin CreateBuildingDigitalTwin(Building building)
        {
            // Initialize twin data
            BasicDigitalTwin twinData = new BasicDigitalTwin();
            twinData.Metadata.ModelId = building.IdDef;
            twinData.Id = building.BuildingId.ToString();

            twinData.CustomProperties.Add(nameof(building.Name), building.Name);
            twinData.CustomProperties.Add(nameof(building.Address), building.Address);
            twinData.CustomProperties.Add(nameof(building.BuildingId), building.BuildingId.ToString());
            twinData.CustomProperties.Add(nameof(building.SpaceVertical), building.SpaceVertical.ToString());

            return twinData;
        }

        private BasicDigitalTwin CreateFloorDigitalTwin(Floor floor)
        {
            // Initialize twin data
            BasicDigitalTwin twinData = new BasicDigitalTwin();
            twinData.Metadata.ModelId = floor.IdDef;
            twinData.Id = floor.FloorId.ToString();

            twinData.CustomProperties.Add(nameof(floor.FloorId), floor.FloorId.ToString());
            twinData.CustomProperties.Add(nameof(floor.FloorLevel), floor.FloorLevel);
            twinData.CustomProperties.Add(nameof(floor.FloorType), floor.FloorType.ToString());

            return twinData;
        }

        private BasicDigitalTwin CreateParkingSlotDigitalTwin(ParkingSlot parkingSlot)
        {
            // Initialize twin data
            BasicDigitalTwin twinData = new BasicDigitalTwin();
            twinData.Metadata.ModelId = parkingSlot.IdDef;
            twinData.Id = parkingSlot.SlotId.ToString();

            twinData.CustomProperties.Add(nameof(parkingSlot.SlotId), parkingSlot.SlotId.ToString());
            twinData.CustomProperties.Add(nameof(parkingSlot.SlotStatus), parkingSlot.SlotStatus.ToString());
            twinData.CustomProperties.Add(nameof(parkingSlot.SlotType), parkingSlot.SlotType.ToString());


            return twinData;
        }

        private Building ConstructSpaceObject(BuildingVM buildingVm)
        {
            var buildingObj = new Building() { BuildingId = Guid.NewGuid(), Name = buildingVm.Name, Address = buildingVm.Address, SpaceVertical = buildingVm.SpaceVertical, Floors = new List<Floor>() };

            List<Floor> floors = new List<Floor>();

            foreach (FloorVM floorVm in buildingVm.Floors)
            {
                Floor floorObj = new Floor()
                {
                    FloorId = Guid.NewGuid(),
                    FloorType = floorVm.FloorType,
                    FloorLevel = floorVm.FloorLevel,
                    ParkingSlots = new List<ParkingSlot>()
                };

                foreach (ParkingSlotVM parkingSlotVm in floorVm.ParkingSlots)
                {
                    ParkingSlot parkingSlotObj = new ParkingSlot()
                    {
                        SlotId = Guid.NewGuid(),
                        SlotStatus = parkingSlotVm.SlotStatus,
                        SlotType = parkingSlotVm.SlotType
                    };

                    floorObj.ParkingSlots.Add(parkingSlotObj);
                }

                buildingObj.Floors.Add(floorObj);
            }

            return buildingObj;
        }

        private async Task<Response<string>> CreateDigitalTwin(List<BasicDigitalTwin> digitalTwins, string prefix)
        {
            Response<string> response = null;
            try
            {
                foreach (BasicDigitalTwin digitalTwin in digitalTwins)
                {
                    response = await _digitalTwinsClient.CreateDigitalTwinAsync($"{prefix}-{digitalTwin.Id}",
                        JsonSerializer.Serialize(digitalTwin));
                    Console.WriteLine($"Created twin: {prefix}-{digitalTwin.Id}");
                }
            }
            catch (RequestFailedException rex)
            {
                Console.WriteLine($"Create twin error: {rex.Status}:{rex.Message}");
            }

            return response;
        }

        private async Task<Response<string>> CreateRelationship(CreateRelationship relationshipObj)
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

        #endregion

        #region Get Spaces

        public async Task<List<JObject>> GetAllSpaces()
        {
            string query = "SELECT * FROM DIGITALTWINS";
            try
            {
                return await ResultToJson(_digitalTwinsClient.QueryAsync(query));
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<List<BuildingVMResponse>> GetAllBuildings()
        {
            string query = $"SELECT * FROM DIGITALTWINS T WHERE IS_OF_MODEL(T, '{SpaceDtId.Buiding}')";
            try
            {
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                var buildingsResult = result.Select(obj => new BuildingVMResponse() { BuildingId = obj["BuildingId"].ToString(), Name = obj["Name"].ToString(), Address = obj["Address"].ToString(), SpaceVertical = obj["SpaceVertical"].ToString() }).ToList();

                return buildingsResult;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<List<FloorVMResponse>> GetAllFloors()
        {
            string query = $"SELECT * FROM DIGITALTWINS T WHERE IS_OF_MODEL(T, '{SpaceDtId.Floor}')";
            try
            {
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                var floorsResult = result.Select(obj => new FloorVMResponse() { FloorId = obj["FloorId"].ToString(), FloorLevel = obj["FloorLevel"].ToString(), FloorType = obj["FloorType"].ToString() }).ToList();

                return floorsResult;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<List<ParkingSlotVMResponse>> GetAllParkingSlots()
        {
            string query = $"SELECT * FROM DIGITALTWINS T WHERE IS_OF_MODEL(T, '{SpaceDtId.ParkingSlot}')";
            try
            {
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                var parkingSlotsResult = result.Select(obj => new ParkingSlotVMResponse() { SlotId = obj["SlotId"].ToString(), SlotType = obj["SlotType"].ToString(), SlotStatus = obj["SlotStatus"].ToString() }).ToList();

                return parkingSlotsResult;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<ParkingSlotVMResponse> GetParkingSlotStatusBySlotId(string slotId)
        {
            string query = $"SELECT * FROM DIGITALTWINS T WHERE T.SlotId = '{slotId}'";

            try
            {
                var qResult = _digitalTwinsClient.QueryAsync(query);
                ParkingSlotVMResponse result = null;
                await foreach (var item in qResult)
                {
                    var temp = JObject.Parse(item);
                    result = new ParkingSlotVMResponse()
                    {
                        SlotId = temp["SlotId"].ToString(),
                        SlotType = temp["SlotType"].ToString(),
                        SlotStatus = temp["SlotStatus"].ToString()
                    };
                }

                return result;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<bool> MapParkingSlotToDevice(MapSpaceToDeviceVM mapSlotToDeviceVm)
        {
            var relationshipExists = false;

            // Check if the device is already linked to another parking slo
            var existingParkingSlot = await _adtClientManager.FindParentRelationship(mapSlotToDeviceVm.DeviceId, "hasGroundSensor");

            if (existingParkingSlot != null)
            {
                // Remove existing relationship
                var deletedRelationship = await _adtClientManager.DeleteRelationship(new DeleteRelationship()
                {
                    SourceTwinId = existingParkingSlot.SourceId,
                    RelationshipId = existingParkingSlot.RelationshipId,
                    RelationshipName = existingParkingSlot.RelationshipName
                });

                relationshipExists = deletedRelationship != true;
            }

            if (relationshipExists == false) // Proceed creating new relationship
            {
                CreateRelationship relationshipObj = new CreateRelationship()
                {
                    SourceTwinId = $"ParkingSlot-{mapSlotToDeviceVm.SlotId}",
                    TargetTwinId = $"Device-{mapSlotToDeviceVm.DeviceId}",
                    RelationshipName = new ParkingSlot().Relationship,
                    RelationshipId = $"slotDeviceRelationship{_random.Next()}"
                };

                await _adtClientManager.CreateRelationship(relationshipObj);
            }

            return true;
        }

        public async Task<List<ParkingSlotVMResponse>> GetParkingSlotsBySlotStatus(string status)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            status = textInfo.ToTitleCase(status);

            string query = $"SELECT * FROM DIGITALTWINS T WHERE T.SlotStatus = '{status}'";
            try
            {
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                var parkingSlotsResult = result.Select(obj => new ParkingSlotVMResponse() { SlotId = obj["SlotId"].ToString(), SlotType = obj["SlotType"].ToString(), SlotStatus = obj["SlotStatus"].ToString() }).ToList();

                return parkingSlotsResult;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<dynamic> GetAllRelationships(string resourceId)
        {
            var query = $"SELECT Target FROM DIGITALTWINS Source JOIN Target RELATED Source.contains where Source.$dtId = '{resourceId}'";

            try
            {
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                dynamic endResult = null;

                var floorList = new Dictionary<string, List<FloorVMResponse>>();
                var parkingList = new Dictionary<string, List<ParkingSlotVMResponse>>();

                var fList = new List<FloorVMResponse>();
                var pList = new List<ParkingSlotVMResponse>();

                foreach (var item in result.Properties())
                {
                    if (resourceId.StartsWith("Building"))
                    {
                        fList.Add(item.Value.ToObject<FloorVMResponse>());
                    }
                    else if (resourceId.StartsWith("Floor"))
                    {
                        pList.Add(item.Value.ToObject<ParkingSlotVMResponse>());
                    }
                }

                if (resourceId.StartsWith("Building"))
                {
                    floorList.Add("Floors", fList);
                    endResult = floorList;
                }
                else if (resourceId.StartsWith("Floor"))
                {
                    parkingList.Add("ParkingSlots", pList);
                    endResult = parkingList;
                }
                return endResult;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<List<Dictionary<string, List<ParkingSlotVMResponse>>>> GetParkingSlotsByBuilding(string building)
        {
            var slotList = new List<Dictionary<string, List<ParkingSlotVMResponse>>>();
            string query;
            try
            {
                if (building.StartsWith("Building-"))
                {
                    query = $"SELECT Floor FROM DIGITALTWINS Building JOIN Floor RELATED Building.contains WHERE Building.$dtId = '{building}' ";
                }

                else
                {
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    building = textInfo.ToTitleCase(building);

                    string subQuery = $"SELECT * FROM DIGITALTWINS T WHERE T.Name = '{building}'";
                    var subResult = await ResultToJson(_digitalTwinsClient.QueryAsync(subQuery));

                    if (subResult.Count == 0)
                    {
                        return slotList;
                    }

                    query = $"SELECT Floor FROM DIGITALTWINS Building JOIN Floor RELATED Building.contains WHERE Building.$dtId = '{subResult[0]["$dtId"]}'";

                }

                var floorList = new Dictionary<string, string>();
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                foreach (JProperty item in result.Properties())
                {
                    floorList.Add(item.Value["FloorLevel"].ToString(), item.Value["$dtId"].ToString());
                }

                foreach (var floor in floorList)
                {
                    slotList.Add(new Dictionary<string, List<ParkingSlotVMResponse>>(){
                            { floor.Key, GetParkingSlotsByFloor(floor.Value).Result}
                    });
                }
            }
            catch (RequestFailedException)
            {
                throw;
            }
            return slotList;
        }

        public async Task<List<ParkingSlotVMResponse>> GetParkingSlotsByFloor(string floor)
        {
            string query;
            var parkingSlotList = new List<ParkingSlotVMResponse>();

            try
            {
                if (floor.StartsWith("Floor-"))
                {
                    query = $"SELECT ParkingSlot FROM DIGITALTWINS Floor JOIN ParkingSlot RELATED Floor.contains WHERE Floor.$dtId = '{floor}'";
                }
                else
                {
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                    floor = textInfo.ToTitleCase(floor);

                    string subQuery = $"SELECT * FROM DIGITALTWINS T WHERE T.FloorLevel = '{floor}'";
                    var subResult = await ResultToJson(_digitalTwinsClient.QueryAsync(subQuery));

                    if (subResult.Count == 0)
                    {
                        return parkingSlotList;
                    }

                    query = $"SELECT ParkingSlot FROM DIGITALTWINS Floor JOIN ParkingSlot RELATED Floor.contains WHERE Floor.$dtId = '{subResult[0]["$dtId"]}'";
                }
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                foreach (JProperty item in result.Properties())
                {
                    parkingSlotList.Add(new ParkingSlotVMResponse()
                    {
                        SlotId = item.Value["SlotId"].ToString(),
                        SlotStatus = item.Value["SlotStatus"].ToString(),
                        SlotType = item.Value["SlotType"].ToString()
                    });
                }
            }
            catch (RequestFailedException)
            {
                throw;
            }
            return parkingSlotList;
        }

        public async Task<List<BuildingVMResponse>> GetBuildingByName(string buildingName)
        {
            string query = $"SELECT * FROM DIGITALTWINS T WHERE T.Name = '{buildingName}' AND IS_OF_MODEL(T, '{SpaceDtId.Buiding}')";
            try
            {
                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                var buildingsResult = result.Select(obj => new BuildingVMResponse() { BuildingId = obj["BuildingId"].ToString(), Name = obj["Name"].ToString(), Address = obj["Address"].ToString(), SpaceVertical = obj["SpaceVertical"].ToString() }).ToList();

                return buildingsResult;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }

        public async Task<List<FloorVMResponse>> GetFloorsInABuilding(string buildingName)
        {
            string buildingQuery = $"SELECT * FROM DIGITALTWINS T WHERE T.Name = '{buildingName}' AND IS_OF_MODEL(T, '{SpaceDtId.Buiding}')";

            try
            {
                var buildingResult = await ResultToJson(_digitalTwinsClient.QueryAsync(buildingQuery));

                var buildingId = (buildingResult.First()["$dtId"]).ToString();

                string query = $"SELECT Floor FROM DIGITALTWINS Building JOIN Floor RELATED Building.contains WHERE Building.$dtId = '{buildingId}' ";

                var result = await ResultToJson(_digitalTwinsClient.QueryAsync(query));
                var floorsResult = result.Select(obj => new FloorVMResponse() { FloorId = obj["Floor"]["FloorId"].ToString(), FloorLevel = obj["Floor"]["FloorLevel"].ToString(), FloorType = obj["Floor"]["FloorType"].ToString() }).ToList();

                return floorsResult;
            }
            catch (RequestFailedException)
            {
                throw;
            }
        }
        #endregion

        #region Support Methods
        private async static Task<List<JObject>> ResultToJson(AsyncPageable<string> qResult)
        {
            var result = new List<JObject>();
            try
            {
                await foreach (string item in qResult)
                {
                    result.Add(JObject.Parse(item));
                };
            }
            catch (RequestFailedException)
            {
                throw;
            }

            return result;
        }
        #endregion
    }
}
