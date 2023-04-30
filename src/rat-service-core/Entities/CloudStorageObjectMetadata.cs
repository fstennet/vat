using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace rat_service_core.Entities;

[ExcludeFromCodeCoverage]
public class CloudStorageObjectMetadata
{
  [JsonPropertyName("fileName")]
  public string? FileName { get; set; }
  [JsonPropertyName("contentType")]
  public string? ContentType { get; set; }
  [JsonPropertyName("labels")]
  public IDictionary<string, string>? Labels { get; set; }
}