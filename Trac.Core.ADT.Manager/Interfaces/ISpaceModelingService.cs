// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISpaceModellingService.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Trac.Core.ADT.Common.Models;
using Trac.Core.ADT.Common.ViewModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Trac.Core.ADT.Manager.Interfaces
{
    public interface ISpaceModelingService
    {
        Task<bool> CreateSpace(BuildingVM building);

        Task<bool> UpdateBuilding(BuildingVM building);

        Task<bool> UpdateFloor(FloorVM floor);

        Task<bool> UpdateParkingSlot(ParkingSlotVM parkingSlot);

        Task<List<BuildingVMResponse>> GetAllBuildings();

        Task<List<JObject>> GetAllSpaces();

        Task<List<FloorVMResponse>> GetAllFloors();

        Task<List<ParkingSlotVMResponse>> GetAllParkingSlots();

        Task<ParkingSlotVMResponse> GetParkingSlotStatusBySlotId(string slotId);

        Task<List<ParkingSlotVMResponse>> GetParkingSlotsBySlotStatus(string status);

        Task<dynamic> GetAllRelationships(string resourceId);

        Task<List<Dictionary<string, List<ParkingSlotVMResponse>>>> GetParkingSlotsByBuilding(string building);

        Task<List<ParkingSlotVMResponse>> GetParkingSlotsByFloor(string floor);

        Task<List<BuildingVMResponse>> GetBuildingByName(string buildingName);

        Task<List<FloorVMResponse>> GetFloorsInABuilding(string buildingName);

        Task<bool> MapParkingSlotToDevice(MapSpaceToDeviceVM mapSlotToDeviceVm);
    }
}
