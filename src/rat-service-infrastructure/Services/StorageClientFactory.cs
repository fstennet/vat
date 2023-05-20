using System.Diagnostics.CodeAnalysis;
using Google.Cloud.Storage.V1;

namespace rat_service_infrastructure.Services;

[ExcludeFromCodeCoverage]
public class StorageClientFactory
{
    public StorageClient CreateStorageClient()
    {
        return StorageClient.Create();
    }
}