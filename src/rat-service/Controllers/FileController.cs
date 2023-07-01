using rat_service_core.Interfaces;
using rat_service_core.Entities;
using rat_service.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace rat_service.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "AuthZPolicy")]
public class FileController : ControllerBase
{
    private readonly ICloudStorageClient _storageClient;
    private readonly CloudStorageOptions _storageOptions;
    private readonly ILogger<FileController> _logger;
    
    public FileController(ICloudStorageClient storageClient, IOptions<CloudStorageOptions> storageOptions, ILogger<FileController> logger)
    {
        _storageClient = storageClient ?? throw new ArgumentNullException(nameof(storageClient));
        _storageOptions = storageOptions != null ? storageOptions.Value : throw new ArgumentNullException(nameof(storageOptions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("list")]
    public IEnumerable<CloudStorageObjectMetadata> List()
    {
        return _storageClient.ListFiles(_storageOptions.BucketName);
    }

    [HttpGet]
    public IActionResult Get(string fileName)
    {
        if (fileName != null)
        {
            var memStream = _storageClient.GetFile(_storageOptions.BucketName, fileName);

            if (memStream != null)
            {
                return Ok(memStream.ToBase64String());
            }

            return NotFound("File not found");
        }

        return BadRequest("No filename provided");
    }

    [HttpPost]
    public IActionResult Post([FromBody] CloudStorageObject file)
    {
        if (file == null)
        {
            return BadRequest("Invalid request");
        }

        var result = _storageClient.UploadFileWithMetadata(_storageOptions.BucketName, file.FileName, file.ContentType, file.FileString.ToMemoryStream(), file.Labels);
        return Ok(result);
    }
}