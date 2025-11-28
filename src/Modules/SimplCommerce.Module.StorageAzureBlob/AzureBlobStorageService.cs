using System.Diagnostics.Contracts;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using SimplCommerce.Module.Core.Services;

namespace SimplCommerce.Module.StorageAzureBlob
{
    public class AzureBlobStorageService : IStorageService
    {
        private BlobContainerClient _blobContainer;
        private string _publicEndpoint;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var storageConnectionString = configuration["Azure:Blob:StorageConnectionString"];
            var containerName = configuration["Azure:Blob:ContainerName"];
            _publicEndpoint = configuration["Azure:Blob:PublicEndpoint"];

            // Allow null/invalid config for local development (skip Azure Blob)
            if (string.IsNullOrWhiteSpace(storageConnectionString) || 
                storageConnectionString.Contains("YOUR_ACCOUNT_KEY") ||
                storageConnectionString.Contains("YOUR_AZURE"))
            {
                // Skip Azure Blob initialization for local dev
                return;
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                return;
            }

            try
            {
                var blobClient = new BlobServiceClient(storageConnectionString);
                _blobContainer = blobClient.GetBlobContainerClient(containerName);

                if (string.IsNullOrWhiteSpace(_publicEndpoint))
                {
                    _publicEndpoint = _blobContainer.Uri.AbsoluteUri;
                }
            }
            catch
            {
                // Ignore errors during local development
            }
        }
        public async Task DeleteMediaAsync(string fileName)
        {
            if (_blobContainer == null) return;
            
            var blockBlob = _blobContainer.GetBlobClient(fileName);
            await blockBlob.DeleteIfExistsAsync();
        }

        public string GetMediaUrl(string fileName)
        {
            if (_blobContainer == null) return $"/user-content/{fileName}";
            
            return $"{_publicEndpoint}/{fileName}";
        }

        public async Task SaveMediaAsync(Stream mediaBinaryStream, string fileName, string mimeType = null)
        {
            if (_blobContainer == null) return;
            
            await _blobContainer.CreateIfNotExistsAsync();
            await _blobContainer.SetAccessPolicyAsync(accessType: PublicAccessType.BlobContainer);

            var blockBlob = _blobContainer.GetBlobClient(fileName);

            var blobHttpHeader = mimeType != null ? new BlobHttpHeaders { ContentType = mimeType } : null;

            if (await blockBlob.ExistsAsync())
            {
                if (blobHttpHeader != null)
                {
                    await blockBlob.SetHttpHeadersAsync(blobHttpHeader);
                }

                await blockBlob.UploadAsync(mediaBinaryStream, overwrite: true);
            }
            else
            {
                await blockBlob.UploadAsync(mediaBinaryStream, blobHttpHeader);
            }
        }
    }
}
