using System.Text;
using rat_service.Extensions;


namespace rat_service_unit_test.Extensions;

public class Conversions_Tests
{
    [Fact]
    public void ToBase64String_ValidMemoryStream_IsConvertedToBase64String()
    {
        // Arrange
        var plainString = "abc123";
        var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(plainString));
        var memStream = new MemoryStream(Encoding.ASCII.GetBytes(plainString));

        // Act
        var result = memStream.ToBase64String();

        // Assert
        Assert.Equal(base64String, result);
    }

    [Fact]
    public void ToMemoryStream_ValidString_ReturnsMemoryStream()
    {
        // Arrange
        var plainString = "abc123";
        var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes(plainString));
        var memStream = new MemoryStream(Encoding.ASCII.GetBytes(plainString));

        // Act
        var result = base64String.ToMemoryStream();

        // Assert
        Assert.Equal(memStream.ToString(), result.ToString());
    }
}