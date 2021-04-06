// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MDeviceTelemetry.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trac.Cosmos.FuncApp.Models
{
    public class MDeviceTelemetry
    {
        //[BsonId(IdGenerator = typeof(CombGuidGenerator))]
        [BsonElement("deviceId")]
        public string Id { get; set; }

        [BsonElement("Message")]
        public string MessageBody { get; set; }

        [BsonElement("SysProperty")]
        public SysProperties SystemProperty { get; set; }

        [BsonElement("Properties")]
        public GenericProperties Properties { get; set; }
    }
}
