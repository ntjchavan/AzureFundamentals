using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public class BlobTriggerFunctions
    {
        [FunctionName("my-container-upload")]
        public void UploadFile([BlobTrigger("my-container/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }

        [FunctionName("ProcessCsvFile")]
        public static void ProcessCsvFileData(
            [BlobTrigger("csv-container/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name,
            ILogger logger
            )
        {
            logger.LogInformation("Processing csv file started.");

            using var reader = new StreamReader(myBlob);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            List<CsvFileData> csvFileData = csv.GetRecords<CsvFileData>().ToList();

            string conn = "Server=.;Database=Test;Trusted_Connection=True;TrustServerCertificate=True";
            using var sqlConnection = new SqlConnection(conn);
            sqlConnection.Open();

            foreach(var data in csvFileData)
            {
                var cmd = new SqlCommand("INSERT INTO dbo.CsvFileData (csvID, csvName, csvLocation) VALUES (@csvID, @csvName, @csvLocation)", sqlConnection);
                cmd.Parameters.AddWithValue("@csvID", data.ID);
                cmd.Parameters.AddWithValue("@csvName", data.Name);
                cmd.Parameters.AddWithValue("@csvLocation", data.Location);
                cmd.ExecuteNonQuery();
            }

            if(sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            logger.LogInformation($"Csv file processing completed with records {csvFileData.Count}");
        }
    }

    public class CsvFileData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
    }
}
