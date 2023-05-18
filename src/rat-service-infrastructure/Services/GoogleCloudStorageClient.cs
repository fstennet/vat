using rat_service_core.Entities;
using rat_service_core.Interfaces;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;

namespace rat_service_infrastructure.Services;

public class GoogleCloudStorageClient : ICloudStorageClient
{
  private readonly StorageClient _storageClient;
  private readonly ILogger<GoogleCloudStorageClient> _logger;

  public GoogleCloudStorageClient(ILogger<GoogleCloudStorageClient> logger, StorageClient storageClient = null)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _storageClient = storageClient ?? StorageClient.Create();
  }
  
  public MemoryStream? GetFile(string? bucketName, string? fileName)
  {
    try
    {
      using var memStream = new MemoryStream();
      _storageClient.DownloadObject(bucketName, fileName, memStream);
      return memStream;
    }
    catch (Exception ex)
    {
      _logger.LogError("An exception occured while downloading a file. Exception: {exception}", ex.Message);
      return null;
    }
  }

  public Task<MemoryStream> GetFileAsync(string bucketName, string fileName)
  {
    throw new NotImplementedException();
  }

  public IEnumerable<CloudStorageObjectMetadata> ListFiles(string? bucketName)
  {
    var objectList = _storageClient.ListObjects(bucketName);

    return objectList.Select(x => new CloudStorageObjectMetadata() { FileName = x.Name, ContentType = x.ContentType, Labels = x.Metadata });
  }

  public string? UploadFile(string bucketName, string fileName, string contentType, Stream fileStream, IDictionary<string, string> labels)
  {
    try
    {
      var result = _storageClient.UploadObject(bucketName, fileName, contentType, fileStream);
      return result.Id;
    }
    catch (Exception ex)
    {
      _logger.LogError("An exception occured while uploading a file. Exception: {exception}", ex.Message);
      throw;
    }

  }

  public Task<string> UploadFileAsync(string bucketName, string fileName, string contentType, Stream fileStream, Dictionary<string, string> labels)
  {
    throw new NotImplementedException();
  }
}
