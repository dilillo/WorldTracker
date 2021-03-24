using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WorldTrackerDomain.Configuration;

namespace WorldTrackerWeb.Components
{
    public interface IBlobUploader
    {
        Task<string> UploadPersonPicture(IFormFile formFile);

        Task<string> UploadPlacePicture(IFormFile formFile);
    }

    public class BlobUploader : IBlobUploader
    {
        private readonly string _connectionString;

        public BlobUploader(IOptions<WorldTrackerOptions> worldTrackerOptions)
        {
            _connectionString = worldTrackerOptions.Value.BlobStorageConnectionString;
        }

        public Task<string> UploadPersonPicture(IFormFile formFile)
        {
            return UploadPicture(formFile, "people");

        }
        public Task<string> UploadPlacePicture(IFormFile formFile)
        {
            return UploadPicture(formFile, "places");
        }

        private async Task<string> UploadPicture(IFormFile formFile, string containerName)
        {
            using (var stream = formFile.OpenReadStream())
            {
                var client = new BlobClient(_connectionString, containerName, formFile.FileName);

                var transferOptions = new StorageTransferOptions();

                transferOptions.MaximumConcurrency = 5;
                transferOptions.MaximumTransferLength = 1048576;
                transferOptions.InitialTransferSize = 1048576;

                var blobUploadOptions = new BlobUploadOptions();

                blobUploadOptions.TransferOptions = transferOptions;

                await client.UploadAsync(stream, blobUploadOptions);

                return client.Uri.ToString();
            }
        }
    }
}
