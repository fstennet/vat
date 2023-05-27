using System.Text.Json.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace rat_service_core.Entities;

[ExcludeFromCodeCoverage]
public class CloudStorageObjectMetadata
{
  [JsonPropertyName("fileName")]
  [Required]
  public string FileName { get; set; }
  [JsonPropertyName("contentType")]
  [Required]
  public string ContentType { get; set; }
  [JsonPropertyName("labels")]
  public IDictionary<string, string> Labels { get; set; }
}