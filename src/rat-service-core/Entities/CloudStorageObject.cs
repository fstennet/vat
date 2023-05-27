using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace rat_service_core.Entities;

[ExcludeFromCodeCoverage]
public class CloudStorageObject : CloudStorageObjectMetadata
{
    [JsonPropertyName("fileString")]
    [Required]
    public string FileString { get; set; }
}