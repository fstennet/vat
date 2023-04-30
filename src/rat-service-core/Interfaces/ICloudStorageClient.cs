using rat_service_core.Entities;

namespace rat_service_core.Interfaces;

public interface ICloudStorageClient
{
  Task<string> UploadFileAsync(string bucketName, string fileName, string contentType, Stream fileStream, Dictionary<string, string> labels);
  Task<MemoryStream> GetFileAsync(string bucketName, string fileName);
  IEnumerable<CloudStorageObjectMetadata> ListFiles(string? bucketName);
  string UploadFile(string bucketName, string fileName, string contentType, Stream fileStream, IDictionary<string, string> labels);
  MemoryStream GetFile(string? bucketName, string? fileName);
}