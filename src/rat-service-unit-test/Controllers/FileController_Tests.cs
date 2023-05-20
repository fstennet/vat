using rat_service.Controllers;
using rat_service_core.Interfaces;
using rat_service_core.Entities;
using rat_service.Extensions;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;

namespace rat_service_unit_test.Controllers;

public class FileController_Tests
{
    private readonly Mock<ICloudStorageClient> _iCloudStorageClientMock;
    private readonly IOptions<CloudStorageOptions> _storageOptions;
    private readonly Mock<ILogger<FileController>> _loggerMock;
    private readonly FileController _sut;

    public FileController_Tests()
    {
        _iCloudStorageClientMock = new Mock<ICloudStorageClient>();
        _storageOptions = Options.Create(new CloudStorageOptions() { BucketName = "test" });
        _loggerMock = new Mock<ILogger<FileController>>();

        _sut = new FileController(_iCloudStorageClientMock.Object, _storageOptions, _loggerMock.Object);
    }
    
    [Fact]
    public void Constructor_StorageClientIsNull_ExceptionIsRaised()
    {
        // Arrange
        ArgumentNullException exception = 
            Assert
                .Throws<ArgumentNullException>(
                    () => new FileController(null, null, null));

        // Act & Assert
        Assert.Equal("Value cannot be null. (Parameter 'storageClient')", exception.Message);
    }

    [Fact]
    public void Constructor_StorageOptionsIsNull_ExceptionIsRaised()
    {
        // Arrange
        ArgumentNullException exception =
            Assert
                .Throws<ArgumentNullException>(
                    () => new FileController(_iCloudStorageClientMock.Object, null, null));

        // Act & Assert
        Assert.Equal("Value cannot be null. (Parameter 'storageOptions')", exception.Message);
    }

    [Fact]
    public void Constructor_IloggerIsNull_ExceptionIsRaised()
    {
        // Arrange
        ArgumentNullException exception =
            Assert
                .Throws<ArgumentNullException>(
                    () => new FileController(_iCloudStorageClientMock.Object, _storageOptions, null));

        // Act & Assert
        Assert.Equal("Value cannot be null. (Parameter 'logger')", exception.Message);
    }

    [Fact]
    public void List_FilesExists_FilesReturned()
    {
        // Arrange
        var objectMetadata = new CloudStorageObjectMetadata() 
        {
            FileName = "test",
            ContentType = "test/test"
        };

        var objectList = new List<CloudStorageObjectMetadata>() { { objectMetadata } };

        _iCloudStorageClientMock
            .Setup(
                x => x.ListFiles(It.IsAny<string>()))
            .Returns(objectList);

        // Act
        var response = _sut.List();

        // Assert
        Assert.Equal(objectList, response);
    }

    [Fact]
    public void List_FilesDoesNotExist_NoFilesReturned()
    {
        // Arrange
        _iCloudStorageClientMock
            .Setup(
                x => x.ListFiles(It.IsAny<string>()))
            .Returns<IEnumerable<CloudStorageObject>>(null);

        // Act
        var response = _sut.List();

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public void Get_FileExists_OKWithFileReturned()
    {
        // Arrange
        var fileName = "testFile.png";
        var fileMemStream = new MemoryStream(Encoding.ASCII.GetBytes(fileName));

        _iCloudStorageClientMock
            .Setup(
                x => x.GetFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(fileMemStream);

        // Act
        var response = _sut.Get(fileName) as ObjectResult;

        // Assert
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("dGVzdEZpbGUucG5n", response.Value);
    }

    [Fact]
    public void Get_FileDoesNotExist_404IsReturned()
    {
        // Arrange
        var fileName = "testFile.png";

        _iCloudStorageClientMock
            .Setup(
                x => x.GetFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns<MemoryStream>(null);

        // Act
        var response = _sut.Get(fileName) as ObjectResult;

        // Assert
        Assert.Equal(404, response.StatusCode);
        Assert.Equal("File not found", response.Value);
    }

    [Fact]
    public void Get_FileNameNotProvided_400IsReturned()
    {
        // Arrange & Act
        var response = _sut.Get(null) as ObjectResult;

        // Assert
        Assert.Equal(400, response.StatusCode);
        Assert.Equal("No filename provided", response.Value);
    }

    [Fact]
    public void Post_ModelIsValid_200IsReturned()
    {
        // Arrange 
        var location = "blob/file.png";

        var file = new CloudStorageObject()
        {
            FileName = "file.png",
            ContentType = "image/png",
            FileString = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+CjwhLS0gQ3JlYXRlZCB3aXRoIElua3NjYXBlIChodHRwOi8vd3d3Lmlua3NjYXBlLm9yZy8pIC0tPgo8c3ZnCiAgIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIKICAgeG1sbnM6Y2M9Imh0dHA6Ly93ZWIucmVzb3VyY2Uub3JnL2NjLyIKICAgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIgogICB4bWxuczpzdmc9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIgogICB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciCiAgIHhtbG5zOnNvZGlwb2RpPSJodHRwOi8vc29kaXBvZGkuc291cmNlZm9yZ2UubmV0L0RURC9zb2RpcG9kaS0wLmR0ZCIKICAgeG1sbnM6aW5rc2NhcGU9Imh0dHA6Ly93d3cuaW5rc2NhcGUub3JnL25hbWVzcGFjZXMvaW5rc2NhcGUiCiAgIHdpZHRoPSIzNDNwdCIKICAgaGVpZ2h0PSIzNDBwdCIKICAgaWQ9InN2ZzIxNjAiCiAgIHNvZGlwb2RpOnZlcnNpb249IjAuMzIiCiAgIGlua3NjYXBlOnZlcnNpb249IjAuNDUuMSIKICAgc29kaXBvZGk6ZG9jYmFzZT0iQzpcVXNlcnNcVHJlbnRvbiBDcm9uaG9sbVxEZXNrdG9wXElua3NjYXBlIgogICBzb2RpcG9kaTpkb2NuYW1lPSJwaWUuc3ZnIgogICBpbmtzY2FwZTpvdXRwdXRfZXh0ZW5zaW9uPSJvcmcuaW5rc2NhcGUub3V0cHV0LnN2Zy5pbmtzY2FwZSI+CiAgPGRlZnMKICAgICBpZD0iZGVmczIxNjIiIC8+CiAgPHNvZGlwb2RpOm5hbWVkdmlldwogICAgIHBhZ2Vjb2xvcj0iI2ZmZmZmZiIKICAgICBib3JkZXJjb2xvcj0iIzY2NjY2NiIKICAgICBib3JkZXJvcGFjaXR5PSIxLjAiCiAgICAgaW5rc2NhcGU6cGFnZW9wYWNpdHk9IjAuMCIKICAgICBpbmtzY2FwZTpwYWdlc2hhZG93PSIyIgogICAgIGlua3NjYXBlOnpvb209IjEuMjIzNTI5NCIKICAgICBpbmtzY2FwZTpjeD0iMjE0LjM3NSIKICAgICBpbmtzY2FwZTpjeT0iMjc3Ljg4NDYyIgogICAgIGlua3NjYXBlOmRvY3VtZW50LXVuaXRzPSJwdCIKICAgICBpbmtzY2FwZTpjdXJyZW50LWxheWVyPSJsYXllcjEiCiAgICAgaWQ9Im5hbWVkdmlldzIxNjQiCiAgICAgaW5rc2NhcGU6d2luZG93LXdpZHRoPSIxMDI0IgogICAgIGlua3NjYXBlOndpbmRvdy1oZWlnaHQ9IjcxOCIKICAgICBpbmtzY2FwZTp3aW5kb3cteD0iLTgiCiAgICAgaW5rc2NhcGU6d2luZG93LXk9Ii04IiAvPgogIDxtZXRhZGF0YQogICAgIGlkPSJtZXRhZGF0YTIxNjYiPgogICAgPHJkZjpSREY+CiAgICAgIDxjYzpXb3JrCiAgICAgICAgIHJkZjphYm91dD0iIj4KICAgICAgICA8ZGM6Zm9ybWF0PmltYWdlL3N2Zyt4bWw8L2RjOmZvcm1hdD4KICAgICAgICA8ZGM6dHlwZQogICAgICAgICAgIHJkZjpyZXNvdXJjZT0iaHR0cDovL3B1cmwub3JnL2RjL2RjbWl0eXBlL1N0aWxsSW1hZ2UiIC8+CiAgICAgIDwvY2M6V29yaz4KICAgIDwvcmRmOlJERj4KICA8L21ldGFkYXRhPgogIDxnCiAgICAgaW5rc2NhcGU6bGFiZWw9IkxheWVyIDEiCiAgICAgaW5rc2NhcGU6Z3JvdXBtb2RlPSJsYXllciIKICAgICBpZD0ibGF5ZXIxIj4KICAgIDxwYXRoCiAgICAgICBzb2RpcG9kaTp0eXBlPSJhcmMiCiAgICAgICBzdHlsZT0ib3BhY2l0eToxO2ZpbGw6bm9uZTtmaWxsLW9wYWNpdHk6MTtmaWxsLXJ1bGU6bm9uemVybztzdHJva2U6IzAwMDAwMDtzdHJva2Utd2lkdGg6MS4zNzU7c3Ryb2tlLWxpbmVjYXA6cm91bmQ7c3Ryb2tlLW1pdGVybGltaXQ6NDtzdHJva2UtZGFzaGFycmF5Om5vbmU7c3Ryb2tlLWRhc2hvZmZzZXQ6MDtzdHJva2Utb3BhY2l0eToxIgogICAgICAgaWQ9InBhdGgyMTY5IgogICAgICAgc29kaXBvZGk6Y3g9IjIxNC4xMzQ2MSIKICAgICAgIHNvZGlwb2RpOmN5PSIyMTIuNSIKICAgICAgIHNvZGlwb2RpOnJ4PSIyMTIuNSIKICAgICAgIHNvZGlwb2RpOnJ5PSIyMTAuMDQ4MDgiCiAgICAgICBkPSJNIDQyNi42MzQ2MSAyMTIuNSBBIDIxMi41IDIxMC4wNDgwOCAwIDEgMSAgMS42MzQ2MTMsMjEyLjUgQSAyMTIuNSAyMTAuMDQ4MDggMCAxIDEgIDQyNi42MzQ2MSAyMTIuNSB6IgogICAgICAgdHJhbnNmb3JtPSJtYXRyaXgoLTAuODQ2NzIyNywtMC41MzIwMzQ0LC0wLjUzMjAzNDQsMC44NDY3MjI3LDUwOC45Njg1NywxNDcuMjc0MzcpIiAvPgogICAgPHBhdGgKICAgICAgIHNvZGlwb2RpOm5vZGV0eXBlcz0iY2NjYyIKICAgICAgIGlkPSJwYXRoMzE0NSIKICAgICAgIGQ9Ik0gMzEwLjQ2MjUyLDQwMi4wODEwNCBMIDIxMy43OTQ2NywyMTQuODkxMTggTCAzMjYuNDE3MTMsMzUuNjU0NjI1IEMgNDU0LjA4MDEzLDExMS42NDEzNyA0NjguNTU3MzQsMzE5LjU1NjUxIDMxMC40NjI1Miw0MDIuMDgxMDQgeiAiCiAgICAgICBzdHlsZT0iZmlsbDojZDQwMDAwO2ZpbGwtcnVsZTpldmVub2RkO3N0cm9rZTpub25lO3N0cm9rZS13aWR0aDoxcHg7c3Ryb2tlLWxpbmVjYXA6YnV0dDtzdHJva2UtbGluZWpvaW46bWl0ZXI7c3Ryb2tlLW9wYWNpdHk6MSIgLz4KICAgIDxwYXRoCiAgICAgICBzdHlsZT0iZmlsbDojMDAwMGZmO2ZpbGwtcnVsZTpldmVub2RkO3N0cm9rZTpub25lO3N0cm9rZS13aWR0aDoxcHg7c3Ryb2tlLWxpbmVjYXA6YnV0dDtzdHJva2UtbGluZWpvaW46bWl0ZXI7c3Ryb2tlLW9wYWNpdHk6MSIKICAgICAgIGQ9Ik0gMy4wOTcyNDAyLDIwOC43NTk2NiBMIDIxMy42OTIwNiwyMTQuNjM2ODcgTCAzMjYuMzE0NTEsMzUuNDAwMzE1IEMgMjAyLjQ2MjM3LC00Ni42NTEzMjYgOC44NTU0NTkxLDMwLjUxNTExOSAzLjA5NzI0MDIsMjA4Ljc1OTY2IHogIgogICAgICAgaWQ9InBhdGgzMTQ3IgogICAgICAgc29kaXBvZGk6bm9kZXR5cGVzPSJjY2NjIiAvPgogICAgPHBhdGgKICAgICAgIHN0eWxlPSJmaWxsOiMwMDgwMDA7ZmlsbC1ydWxlOmV2ZW5vZGQ7c3Ryb2tlOm5vbmU7c3Ryb2tlLXdpZHRoOjFweDtzdHJva2UtbGluZWNhcDpidXR0O3N0cm9rZS1saW5lam9pbjptaXRlcjtzdHJva2Utb3BhY2l0eToxIgogICAgICAgZD0iTSAzLjU0ODI2NzUsMjA4LjE4ODM1IEwgMjEzLjU1MDI5LDIxNC41NTUwOSBMIDMxMS42OTI3OSw0MDAuOTI3NDYgQyAxNzcuMzg4NDQsNDY3LjcyNDMzIDMuODUzOTkxMywzODAuMTE2MzggMy41NDgyNjc1LDIwOC4xODgzNSB6ICIKICAgICAgIGlkPSJwYXRoMzE0OSIKICAgICAgIHNvZGlwb2RpOm5vZGV0eXBlcz0iY2NjYyIKICAgICAgIGlua3NjYXBlOnRyYW5zZm9ybS1jZW50ZXIteT0iOTguODcwNTQ3IgogICAgICAgaW5rc2NhcGU6dHJhbnNmb3JtLWNlbnRlci14PSI1OC45NDUzMzciIC8+CiAgPC9nPgo8L3N2Zz4K"
        };

        _iCloudStorageClientMock
            .Setup(x => x.UploadFileWithMetadata(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<Stream>(), 
                It.IsAny<IDictionary<string, string>>()))
            .Returns(location);
        
        //& Act
        var response = _sut.Post(file) as ObjectResult;

        // Assert
        Assert.Equal(200, response.StatusCode);
        Assert.Equal(location, response.Value);
    }

    [Fact]
    public void Post_ModelIsInvalid_400IsReturned()
    {
        // Arrange & Act
        var response = _sut.Post(null) as ObjectResult;

        // Assert
        Assert.Equal(400, response.StatusCode);
        Assert.Equal("Invalid request", response.Value);
    }
}