// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyVaultStoreManager.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Microsoft.Extensions.Configuration.Json;
using System.Text.Json;
using System;
using Microsoft.Extensions.Configuration;

namespace Trac.Core.Common.Configuration
{
    public sealed class KeyVaultStoreManager
    {
        // TODO :: Connect to keyvault to get the value based on key shared;
        private static readonly KeyVaultStoreManager _keyVaultStoreManagerInstance = new KeyVaultStoreManager();

        static KeyVaultStoreManager()
        {
        }

        private KeyVaultStoreManager()
        {
        }

        public static KeyVaultStoreManager keyVaultStoreManagerInstance
        {
            get
            {
                return _keyVaultStoreManagerInstance;
            }
        }
        public IConfiguration GetValueFromKeyVault()
        {
            //IConfiguration config;
            // Read configuration data from the keyvalue, c

            return new ConfigurationBuilder()
               .AddJsonFile("KeyVaultReplicaSecrets.json", false, true)
               .Build();

            //clientId = config["clientId"];
            //tenantId = config["tenantId"];
            //adtInstanceUrl = config["instanceUrl"];
            //return config;
        }
    }
}

