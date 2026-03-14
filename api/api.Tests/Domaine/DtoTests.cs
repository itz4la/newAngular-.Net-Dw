using domaine;
using Xunit;

namespace api.Tests.Domaine;

public class DtoTests
{
    [Fact]
    public void LoginDto_StoresCredentials()
    {
        var dto = new LoginDTO
        {
            Username = "tester",
            Password = "p@ss"
        };

        Assert.Equal("tester", dto.Username);
        Assert.Equal("p@ss", dto.Password);
    }

    [Fact]
    public void RegisterDto_StoresUserData()
    {
        var dto = new RegisterDTO
        {
            Username = "tester",
            Password = "p@ss",
            Email = "tester@example.com"
        };

        Assert.Equal("tester", dto.Username);
        Assert.Equal("p@ss", dto.Password);
        Assert.Equal("tester@example.com", dto.Email);
    }
}
