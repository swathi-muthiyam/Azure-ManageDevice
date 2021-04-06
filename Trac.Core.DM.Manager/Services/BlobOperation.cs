// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobOperation.cs" company=" "> 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
//   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR 
//   OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
//   ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 
//   OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Trac.Core.Common.Configuration;
using Trac.Core.DM.Manager;
using Trac.Core.DM.Manager.Interfaces;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using BlobType = Microsoft.WindowsAzure.Storage.Blob.BlobType;

namespace Trac.Core.DeviceManagement.API.Services
{
    public class BlobOperation
    {
        private readonly IConfiguration _registerConfig;
        private readonly string _connectionString;

        public BlobOperation()
        {
            this._registerConfig = KeyVaultStoreManager.keyVaultStoreManagerInstance.GetValueFromKeyVault();
            _connectionString = Convert.ToString(_registerConfig["DeviceManagement:Registration:AzureBulkRegisterStorage"]);
        }
        public async Task<bool> UploadAndDeleteBlobAsync(string containerName, string blobName, string body)
        {
           // var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);

            // string connectionString = CloudConfigurationManager.GetSetting("StorageConnection"); // TODO : once we finalize on storage to store secrets
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
                //string body;
                //body = await req.Content.ReadAsStringAsync();
                await WriteBlob();

                async Task WriteBlob()
                {
                    using (var stream = await cloudBlockBlob.OpenWriteAsync())
                    using (var sw = new StreamWriter(stream))
                    {
                        await sw.WriteLineAsync(body);

                        DeleteBlob("bulkdevice", "bulkuploaddevices.csv");
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on UploadBinaryAsync : " + blobName);
                //logger.Error(e);
                return false;
            }
            return true;
        }

        public async Task<bool> UploadBlobAsync(string containerName, string blobName, string body)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);

            // string connectionString = CloudConfigurationManager.GetSetting("StorageConnection"); // TODO : once we finalize on storage to store secrets
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
                //string body;
                //body = await req.Content.ReadAsStringAsync();
                await WriteBlob();

                async Task WriteBlob()
                {
                    using (var stream = await cloudBlockBlob.OpenWriteAsync())
                    using (var sw = new StreamWriter(stream))
                    {
                        await sw.WriteLineAsync(body);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on UploadBinaryAsync : " + blobName);
                //logger.Error(e);
                return false;
            }
            return true;
        }
        public void DeleteBlob(string containerName, string blobName)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_connectionString);
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
                cloudBlockBlob.DeleteIfExistsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void DownloadBlob(string blobToDownload, string containerName, string downloadsPath)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(_registerConfig[DMConstants.iothubconnectionstring]);
            string connectionString = Convert.ToString(_registerConfig["DeviceManagement:Registration:AzureBulkRegisterStorage"]);
            // string connectionString = CloudConfigurationManager.GetSetting("StorageConnection"); // TODO : once we finalize on storage to store secrets
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            container.CreateIfNotExists(PublicAccessType.Blob);

            //lines modified
            var blockBlob = container.GetBlobClient(blobToDownload);
            using (var fileStream = System.IO.File.OpenWrite(downloadsPath + "\\" + blobToDownload))
            {
                blockBlob.DownloadTo(fileStream);
            }
        }

    }
}
