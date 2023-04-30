using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace rat_service_core.Entities;

[ExcludeFromCodeCoverage]
public class CloudStorageObject : CloudStorageObjectMetadata
{
    [JsonPropertyName("fileString")]
    public string? FileString { get; set; }
}