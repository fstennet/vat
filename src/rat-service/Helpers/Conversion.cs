namespace rat_service.Helpers;

public static class Conversion
{
    public static string ToBase64String(this MemoryStream stream)
    {
        return Convert.ToBase64String(stream.ToArray());
    }

    public static MemoryStream ToMemoryStream(this string fileString)
    {
        byte[] data = Convert.FromBase64String(fileString);
        return new MemoryStream(data);
    }
}