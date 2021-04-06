using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Trac.BulkDeviceManagement.FuncApp
{
    public static class BulkDeviceManagementQueue
    {
        [FunctionName("BulkDeviceManagementQueue")]
        public static void Run(
            [QueueTrigger("bulkdeviceinfo", Connection = "AzureWebJobsStorage")]string myQueueItem,
            [Blob("bulkdevicefiles/{queueTrigger}", FileAccess.Read)] Stream myBlob,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
