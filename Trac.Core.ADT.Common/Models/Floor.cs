// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Floor.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Trac.Core.ADT.Common.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Core.ADT.Common.Models
{
    public class Floor
    {
        public string IdDef { get; } = "dtmi:com:bosch:trac360:sm:parking:floor;1";
        public Guid FloorId { get; set; }
        public string FloorLevel { get; set; }
        public FloorType FloorType { get; set; }
        public List<ParkingSlot> ParkingSlots { get; set; }
        public string Relationship { get; } = "contains";
    }
}
