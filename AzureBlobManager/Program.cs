using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AzureBlobManager
{
    class Program
    {
        static async Task  Main(string[] args)
        {


            Console.WriteLine("Azure Blob storage v12 - .NET  C# Console quickstart sample\n");

            // Retrieve the connection string for use with the application. The storage
            // connection string is stored in an environment variable on the machine
            // running the application called AZURE_STORAGE_CONNECTION_STRING. If the
            // environment variable is created after the application is launched in a
            // console or with Visual Studio, the shell or application needs to be closed
            // and reloaded to take the environment variable into account.
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            Console.WriteLine(string.Concat("connectionString:", connectionString));



            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //Create a unique name for the container
            string containerName = "test" + Guid.NewGuid().ToString();

            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);


            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "./data/";
            string fileName = "test" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);

            // Write text to the file
             File.WriteAllText(localFilePath, "Hello, World!");

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            // Open the file and upload its data
            using (FileStream uploadFileStream = File.OpenRead(localFilePath)) {
                await blobClient.UploadAsync(uploadFileStream, true);
                uploadFileStream.Close();
            } ;



            Console.WriteLine("Listing blobs...");

            // List all blobs in the container
             foreach (BlobItem blobItem in containerClient.GetBlobs())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }

            // Download the blob to a local file
            // Append the string "DOWNLOADED" before the .txt extension 
            // so you can compare the files in the data directory
            string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }


            // Clean up
            Console.Write("Press any key to begin clean up");
            Console.ReadLine();

            Console.WriteLine("Deleting blob container...");
            await containerClient.DeleteAsync();

            Console.WriteLine("Deleting the local source and downloaded files...");
            File.Delete(localFilePath);
            File.Delete(downloadFilePath);

            Console.WriteLine("Done");



            Console.ReadLine();
        }
    }
}
