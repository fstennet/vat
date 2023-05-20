using rat_service_core.Entities;
using rat_service_core.Interfaces;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;

namespace rat_service_infrastructure.Services;

public class GoogleCloudStorageClient : ICloudStorageClient
{
  private readonly StorageClient _storageClient;
  private readonly ILogger<GoogleCloudStorageClient> _logger;

  public GoogleCloudStorageClient(ILogger<GoogleCloudStorageClient> logger, StorageClient storageClient)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _storageClient = storageClient ?? throw new ArgumentNullException(nameof(storageClient));
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

  public IEnumerable<CloudStorageObjectMetadata> ListFiles(string? bucketName)
  {
    var objectList = _storageClient.ListObjects(bucketName);

    return objectList.Select(x => new CloudStorageObjectMetadata() { FileName = x.Name, ContentType = x.ContentType, Labels = x.Metadata });
  }

  public string? UploadFile(string bucketName, string fileName, string contentType, Stream fileStream)
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
  
  public string UploadFileWithMetadata(string bucketName, string fileName, string contentType, Stream fileStream, IDictionary<string, string> labels)
  {
    try
    {
      var result = _storageClient.UploadObject(bucketName, fileName, contentType, fileStream);

      result.Metadata = labels;

      var resultWithMetadata = _storageClient.UpdateObject(result);

      return resultWithMetadata.Id;
    }
    catch (Exception ex)
    {
      _logger.LogError("An exception occured while uploading a file. Exception: {exception}", ex.Message);
      throw;
    }
  }
}
