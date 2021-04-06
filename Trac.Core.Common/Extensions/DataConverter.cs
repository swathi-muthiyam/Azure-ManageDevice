using CsvHelper;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Trac.Core.Common.Extensions
{
    public static class DataConverter
    {
        public static string ConvertCsvToJson(this Stream blob)
        {
            try
            {
                var sReader = new StreamReader(blob);
                var csv = new CsvReader(sReader, CultureInfo.InvariantCulture);
                csv.Read();
                csv.ReadHeader();
                var csvRecords = csv.GetRecords<object>().ToList();
                return JsonConvert.SerializeObject(csvRecords);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void SerializeJsonIntoStream(this object value, Stream stream)
        {
            try
            {
                using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
                using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
                {
                    var js = new JsonSerializer();
                    js.Serialize(jtw, value);
                    jtw.Flush();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;
            try
            {
                if (content != null)
                {
                    var ms = new MemoryStream();
                    // DataConverter.SerializeJsonIntoStream(content, ms);
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
