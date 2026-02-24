using AppSec_Web_API.Domain;

namespace AppSec_Web_API.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
