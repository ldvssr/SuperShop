using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SuperShop.Helpers
{
    public class BlobHelper : IBlobHelper
    {
        private readonly BlobContainerClient _blobClient;
        private readonly IConfiguration _configuration;
        public BlobHelper(IConfiguration configuration) 
        {
            string keys = configuration["Blob:ConnectionString"]; 
            _configuration = configuration;
        }

        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream();
            return await UploadStreamAsync(stream, containerName);
        }

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            return await UploadStreamAsync(stream, containerName);
        }

        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            Guid name = Guid.NewGuid();

            var blobContainerClient =
            new BlobContainerClient(
                _configuration["Blob:ConnectionString"],
                "images/products" + containerName);          

            var blobClient = blobContainerClient.GetBlobClient(name.ToString());

            await blobClient.UploadAsync(stream);

            return name;
        }
    }
}
