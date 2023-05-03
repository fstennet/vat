using Google.Cloud.Storage.V1;
using Moq;
using rat_service_infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace rat_service_unit_test.Infrastructure.Services;

public class GoogleCloudStorageClient_Tests
{
  private readonly Mock<StorageClient> _storageClientMock;
  private readonly Mock<ILogger<GoogleCloudStorageClient>> _loggingMock;
  private readonly GoogleCloudStorageClient _sut;
  public GoogleCloudStorageClient_Tests()
  {
    _storageClientMock = new Mock<StorageClient>();
    _loggingMock = new Mock<ILogger<GoogleCloudStorageClient>>();

    _sut = new GoogleCloudStorageClient(_loggingMock.Object, _storageClientMock.Object);
  }


  [Fact]
  public void Constructor_LoggerIsNull_ThrowsException()
  {
    // Arrange
    ArgumentNullException exception = Assert
                                      .Throws<ArgumentNullException>(
                                        () => new GoogleCloudStorageClient(null));

    // Act & Assert
    Assert.Equal("Value cannot be null. (Parameter 'logger')", exception.Message);
  }

  [Fact]
  public void GetFileAsync_FileIsDownloaded()
  {
    // Arrange
    var bucketName = "ABC";
    var fileName = "123.jpg";

    _storageClientMock
      .Setup(
        x => x.DownloadObject(
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<Stream>(),
          null, null)).Verifiable();
    
    // Act
    var result = _sut.GetFile(bucketName, fileName);

    // Assert
    _storageClientMock.Verify(
      x => x.DownloadObject(
        It.IsAny<string>(),
        It.IsAny<string>(),
        It.IsAny<Stream>(),
        null, null), Times.Once());
  }

  [Fact]
  public void GetFileAsync_ExceptionOccurs_ExceptionIsLogged()
  {
    // Arrange
    var bucketName = "ABC";
    var fileName = "123.jpg";

    _storageClientMock
      .Setup(
        x => x.DownloadObject(
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<Stream>(),
          null, null)).Throws(new Exception());

    // Act
    var result = _sut.GetFile(bucketName, fileName);

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
}