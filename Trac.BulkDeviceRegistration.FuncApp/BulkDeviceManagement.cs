using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trac.Core.ADT.Manager;
using Trac.Core.Common.Extensions;
using Trac.Core.DeviceManagement.API.Services;
using Trac.Core.DM.Manager.Models;
using Trac.Core.DM.Manager.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Dasync.Collections;
using Trac.BulkDeviceManagement.FuncApp.Extensions;
using Trac.BulkDeviceManagement.FuncApp.Constants;

namespace Trac.BulkDeviceRegistration.FuncApp
{
    public static class BulkDeviceManagement
    {
        [FunctionName("BulkDeviceManagementFunction")]
        public static async System.Threading.Tasks.Task RunAsync(
            [BlobTrigger("bulkdevice/{name}", Connection = "AzureWebJobsStorage")] Stream devicesBlob,
            //[Blob("bulkdevice-success/{name}", FileAccess.Write)] Stream success,
            //[Blob("bulkdevice-failed/{name}", FileAccess.Write)] Stream failed,
            string name, ILogger log)
        {
            // TODO : need to discuss on herirachy of container structure. Current assuming no child folders.
            string[] containerFile = name.Split("/")[0].Split(".");

            // Get blob from BLOB STORAGE  , read blob and csvBlobcall api recurssively to register device
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {devicesBlob.Length} Bytes");
            BlobOperation blobOperation = new BlobOperation(); // TODO messaging to be implemented as a different service.
            string blobname = string.Empty;
            object jObject;
            if (name.Contains(".csv"))
            {
                //jObject = DataConverter.ConvertCsvToJson(devicesBlob);
                jObject = devicesBlob.ConvertCsvToJson();
            }
            else
            {
                //TODO : Not tested , discussion pending
                StreamReader reader = new StreamReader(devicesBlob);
                //string blobContent = reader.ReadToEnd();
                jObject = JsonConvert.DeserializeObject<JObject>(reader.ReadToEnd());
            }
            #region 
            /*string path = @"D:\ -SourceCode\BulkUpload\successResponse.txt";
            //string request;
            using (var streamReader = new StreamReader(path, Encoding.UTF8))
            {
                string request = streamReader.ReadToEnd();
                request = request.TrimStart('\"');
                request = request.TrimEnd('\"');
                request = request.Replace("\\", "");
                var result = JsonConvert.DeserializeObject<List<DeviceRegistryOperationResult>>(request);
                if (result != null && result.Count() > 0)
                {
                    var failedDevices = result.Where(r => r.IsSuccessful == false);
                    if (failedDevices != null && failedDevices.Count() > 0)
                    {
                        var failedParm = JsonConvert.SerializeObject(failedDevices);
                        string  blobname = containerFile[0] + "-Failed-" + DateTime.Now.ToString("ddmmyyyyhhmmss")+ ".txt";
                        await UploadBlobAsync("bulkdevice-failed", blobname, failedParm);
                    }
                    var successDevices = result.Where(r => r.IsSuccessful == true);
                    if (successDevices != null && successDevices.Count() > 0)
                    {
                        string blobname = containerFile[0] + "-success-" + DateTime.Now.ToString("ddmmyyyyhhmmss")+".txt";
                        var successParm = JsonConvert.SerializeObject(successDevices);
                        await UploadBlobAsync("bulkdevice-success", blobname, successParm);
                    }
                }
            }

           */

            #endregion


            if (jObject.Equals(BDMConstants.NoHeaderError))
            {
                blobname = containerFile[0].FailedContainerName();
                await blobOperation.UploadAndDeleteBlobAsync(BDMConstants.Failed, blobname, BDMConstants.NoHeaderError);
                return;
            }
            //Call devicemgmt  API
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44363/api/bulkupload"))
            using (var httpContent = CreateHttpContent(jObject))
            {
                request.Content = httpContent;
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(int.Parse("120")));
                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token)
                    .ConfigureAwait(false))
                {
                    // seggrigate the results and upload the success & failed files to BLOB.
                    response.EnsureSuccessStatusCode();
                    var res = await response.Content.ReadAsStringAsync();
                    res = res.TrimStart('\"');
                    res = res.TrimEnd('\"');
                    res = res.Replace("\\", "");
                    var result = JsonConvert.DeserializeObject<List<DeviceRegistryOperationResult>>(res);
                    log.LogInformation($"No of responses of registered devices : { result.Count()} ");
                    if (result != null && result.Count() > 0)
                    {
                        var failedDevices = result.Where(r => r.IsSuccessful == false);
                        if (failedDevices != null && failedDevices.Count() > 0)
                        {
                            var failedParm = JsonConvert.SerializeObject(failedDevices);
                            //blobname = containerFile[0] + "-Failed-" + DateTime.Now.ToString("ddmmyyyyhhmmss") + ".txt";
                            blobname = containerFile[0].FailedContainerName();
                            log.LogInformation($"Uploading  : { blobname} with { failedDevices.Count()} failed records with exception");
                            await blobOperation.UploadAndDeleteBlobAsync(BDMConstants.Failed, blobname, failedParm);
                            log.LogInformation($"Uploaded  : { blobname} ");
                        }
                        var successDevices = result.Where(r => r.IsSuccessful == true);
                        if (successDevices != null && successDevices.Count() > 0)
                        {
                            blobname = containerFile[0].SuccessContainerName();
                            var successParm = JsonConvert.SerializeObject(successDevices);
                            log.LogInformation($"Uploading  : { blobname} with success count { successDevices.Count() }");
                            await blobOperation.UploadAndDeleteBlobAsync(BDMConstants.Success, blobname, successParm);
                            log.LogInformation($"Uploaded  : { blobname} ");
                        }
                    }

                }
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;
            try
            {
                if (content != null)
                {
                    var ms = new MemoryStream();
                    //DataConverter.SerializeJsonIntoStream(content, ms);
                    content.SerializeJsonIntoStream(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    httpContent = new StreamContent(ms);
                    httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return httpContent;
        }



    }
}
