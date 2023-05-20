using rat_service_core.Entities;

namespace rat_service_core.Interfaces;

public interface ICloudStorageClient
{
  IEnumerable<CloudStorageObjectMetadata> ListFiles(string? bucketName);
  string UploadFile(string bucketName, string fileName, string contentType, Stream fileStream);
  string UploadFileWithMetadata(string bucketName, string fileName, string contentType, Stream fileStream, IDictionary<string, string> labels);
  MemoryStream GetFile(string? bucketName, string? fileName);
}