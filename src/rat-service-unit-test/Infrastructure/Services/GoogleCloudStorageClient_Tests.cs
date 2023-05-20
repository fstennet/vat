using Google.Cloud.Storage.V1;
using Moq;
using rat_service_infrastructure.Services;
using Microsoft.Extensions.Logging;
using Google.Apis.Upload;
using Google.Api.Gax.Rest;

namespace rat_service_unit_test.Infrastructure.Services;

public class GoogleCloudStorageClient_Tests
{
  private readonly Mock<StorageClient> _storageClientMock;
  private readonly Mock<ILogger<GoogleCloudStorageClient>> _loggingMock;
  private readonly GoogleCloudStorageClient _sut;
  private readonly string _bucketName, _fileName, _contentType;
  private readonly Dictionary<string, string> _labels;

  public GoogleCloudStorageClient_Tests()
  {
    _storageClientMock = new Mock<StorageClient>();
    _loggingMock = new Mock<ILogger<GoogleCloudStorageClient>>();

    _sut = new GoogleCloudStorageClient(_loggingMock.Object, _storageClientMock.Object);

    _bucketName = "ABC";
    _fileName = "123.jpg";
    _contentType = "image/jpeg";
    _labels = new Dictionary<string, string>() { { "test", "test" } };
  }


  [Fact]
  public void Constructor_LoggerIsNull_ThrowsException()
  {
    // Arrange
    ArgumentNullException exception = Assert
                                      .Throws<ArgumentNullException>(
                                        () => new GoogleCloudStorageClient(null, null));

    // Act & Assert
    Assert.Equal("Value cannot be null. (Parameter 'logger')", exception.Message);
  }

  // This unit test is useless
  [Fact]
  public void Constructor_StorageClientIsNull_StorageClientIsCreated()
  {
    // Arrange
    var result = new GoogleCloudStorageClient(_loggingMock.Object, null);

    // Act & Assert
    Assert.NotNull(result);
  }
  [Fact]
  public void GetFile_FileIsDownloaded()
  {
    // Arrange
    _storageClientMock
      .Setup(
        x => x.DownloadObject(
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<Stream>(),
          null, null)).Verifiable();
    
    // Act
    var result = _sut.GetFile(_bucketName, _fileName);

    // Assert
    _storageClientMock.Verify(
      x => x.DownloadObject(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<Stream>(),
        null, null), Times.Once());
  }

  [Fact]
  public void GetFile_ExceptionOccurs_ExceptionIsLogged()
  {
    // Arrange
    _storageClientMock
      .Setup(
        x => x.DownloadObject(
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<Stream>(),
          null, null)).Throws(new Exception());

    // Act
    var result = _sut.GetFile(_bucketName, _fileName);

    // Assert
    _storageClientMock.Verify(
      x => x.DownloadObject(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<Stream>(),
        null, null), Times.Once());

    _loggingMock.Verify(
      x => x.Log(
        LogLevel.Error,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((@object, _) => @object.ToString().Contains("An exception occured while downloading a file.")),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
  }
  
  [Fact]
  public void UploadFileWithMetadata_ReturnsObjectId()
  {
    // Arrange
    var resultObject = new Google.Apis.Storage.v1.Data.Object() 
    { Id = "something" };

    _storageClientMock
      .Setup(x => x.UploadObject(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<Stream>(),
        It.IsAny<UploadObjectOptions>(),
        It.IsAny<IProgress<IUploadProgress>>()
      )).Returns(new Google.Apis.Storage.v1.Data.Object());

    _storageClientMock
      .Setup(x => x.UpdateObject(
        It.IsAny<Google.Apis.Storage.v1.Data.Object>(),
        It.IsAny<UpdateObjectOptions>()
        )).Returns(resultObject);

    // Act
    var result = _sut.UploadFileWithMetadata(_bucketName, _fileName, _contentType, new MemoryStream(), _labels);

    // Assert
    Assert.Equal(resultObject.Id, result);
  }

  [Fact]
  public void UploadFileWithMetadata_Exception_LogsError()
  {
    // Arrange
    _storageClientMock
      .Setup(x => x.UploadObject(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<Stream>(),
        It.IsAny<UploadObjectOptions>(),
        It.IsAny<IProgress<IUploadProgress>>()
      )).Throws(new Exception());

    // Act & Assert
    Assert.Throws<Exception>(() => _sut.UploadFileWithMetadata(_bucketName, _fileName, _contentType, new MemoryStream(), _labels)); 

    _loggingMock
      .Verify(
        x => x.Log(
          LogLevel.Error,
          It.IsAny<EventId>(),
          It.Is<It.IsAnyType>((@object, _) => @object.ToString().Contains("An exception occured while uploading a file. Exception:")),
          It.IsAny<Exception>(),
          It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
  }

  [Fact]
  public void UploadFile_FileIsUploaded_ReturnsId()
  {
    // Arrange
    var resultObject = new Google.Apis.Storage.v1.Data.Object() { Id = "something" };

    _storageClientMock
      .Setup(x => x.UploadObject(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<Stream>(),
        It.IsAny<UploadObjectOptions>(),
        It.IsAny<IProgress<IUploadProgress>>()
      )).Returns(resultObject);

    // Act
    var result = _sut.UploadFile(_bucketName, _fileName, _contentType, new MemoryStream());

    // Assert
    Assert.Equal(resultObject.Id, result);
  }

  [Fact]
  public void UploadFile_ExceptionOccurs_LogsError()
  {
      // Arrange
      _storageClientMock
        .Setup(x => x.UploadObject(
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<Stream>(),
          It.IsAny<UploadObjectOptions>(),
          It.IsAny<IProgress<IUploadProgress>>()
        )).Throws(new Exception());

      // Act & Assert
      Assert.Throws<Exception>(() => _sut.UploadFile(_bucketName, _fileName, _contentType, new MemoryStream()));

      _loggingMock
        .Verify(
          x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((@object, _) => @object.ToString().Contains("An exception occured while uploading a file. Exception:")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
          Times.Once);
  }
}