// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpaceModellingController.cs" company=" "> 
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
using System.Linq;
using System.Threading.Tasks;
using Trac.Core.ADT.Common.ViewModels;
using Trac.Core.ADT.Manager.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Trac.Core.SpaceModelling.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceModelingController : ControllerBase
    {
        private readonly ILogger<SpaceModelingController> _logger;
        private readonly ISpaceModelingService _adtClientService;

        public SpaceModelingController(ILogger<SpaceModelingController> logger, ISpaceModelingService adtClientService)
        {
            _logger = logger;
            _adtClientService = adtClientService;
        }

        [HttpPost]
        [Route("CreateSpace")]
        public async Task<bool> CreateSpace(BuildingVM buildingVm)
        {
            var completed = await _adtClientService.CreateSpace(buildingVm);
            return completed;
        }

        [HttpPut]
        [Route("UpdateBuilding")]
        public async Task<bool> UpdateBuilding(BuildingVM buildingVm)
        {
            var completed = await _adtClientService.UpdateBuilding(buildingVm);
            return completed;
        }

        [HttpPut]
        [Route("UpdateFloor")]
        public async Task<bool> UpdateFloor(FloorVM floorVm)
        {
            var completed = await _adtClientService.UpdateFloor(floorVm);
            return completed;
        }

        [HttpPut]
        [Route("UpdateParkingSlot")]
        public async Task<bool> UpdateParkingSlot(ParkingSlotVM parkingSlotVm)
        {
            var completed = await _adtClientService.UpdateParkingSlot(parkingSlotVm);
            return completed;
        }

        [HttpGet]
        [Route("Space")]
        public IActionResult GetAllSpaces()
        {
            try
            {
                var result = _adtClientService.GetAllSpaces().Result;
                if (result.Count() > 0)
                {
                    return Ok(result);
                }
                return NotFound("No Space Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //[HttpGet]
        //[Route("Model")]
        //public IActionResult GetModel(string modelId)
        //{
        //    try
        //    {
        //        var result = _adtClientService.GetModel(modelId).Result;
        //        if (result.Value != null)
        //        {
        //            return Ok(result.Value);
        //        }
        //        return NotFound("No Space Found");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("Building")]
        public IActionResult GetAllBuildings()
        {
            try
            {
                var result = _adtClientService.GetAllBuildings().Result;
                if (result.Count() > 0)
                {
                    return Ok(result);
                }
                return NotFound("No Building Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Floor")]
        public IActionResult GetAllFloors()
        {
            try
            {
                var result = _adtClientService.GetAllFloors().Result;
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No Floors Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("ParkingSlot")]
        public IActionResult GetAllParkingSlots()
        {
            try
            {
                var result = _adtClientService.GetAllParkingSlots().Result;
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No ParkingSlot Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("ParkingSlotStatus/{slotId}")]
        public IActionResult GetParkingSlotStatusBySlotId(string slotId)
        {
            try
            {
                var result = _adtClientService.GetParkingSlotStatusBySlotId(slotId).Result;
                if (result != null)
                {
                    return Ok(result);
                }
                return NotFound("Invalid SlotId");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("ParkingSlotsBySlotStatus/{status}")]
        public IActionResult GetParkingSlotsBySlotStatus(string status)
        {
            try
            {
                var result = _adtClientService.GetParkingSlotsBySlotStatus(status).Result;
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No ParkingSlots Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Relationship/{resourceId}")]
        public IActionResult GetAllRelationships(string resourceId)
        {
            try
            {
                dynamic result = _adtClientService.GetAllRelationships(resourceId).Result;
                
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No Related Resources Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("GetBuildingByName/{buildingName}")]
        public ActionResult GetBuildingByName(string buildingName)
        {
            try
            {
                var result = _adtClientService.GetBuildingByName(buildingName).Result;
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No Buildings Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetFloorsInABuilding/{buildingName}")]
        public ActionResult GetFloorsInABuilding(string buildingName)
        {
            try
            {
                var result = _adtClientService.GetFloorsInABuilding(buildingName).Result;
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No Floors Found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("ParkingSlotsByBuilding/{building}")]
        public IActionResult GetParkingSlotsByBuilding(string building)
        {
            try
            {
                var result = _adtClientService.GetParkingSlotsByBuilding(building).Result;
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No ParkingSlots found for this Building OR Invalid Building");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("ParkingSlotsByFloor/{floor}")]
        public IActionResult GetParkingSlotsByFloor(string floor)
        {
            try
            {
                var result = _adtClientService.GetParkingSlotsByFloor(floor).Result;
                if (result.Count > 0)
                {
                    return Ok(result);
                }
                return NotFound("No ParkingSlots found for this Floor OR Invalid Floor");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("MapParkingSlotToDevice")]
        public async Task<bool> MapParkingSlotToDevice(MapSpaceToDeviceVM mapSlotToDeviceVm)
        {
            var completed = await _adtClientService.MapParkingSlotToDevice(mapSlotToDeviceVm);
            return completed;
        }
    }
}
