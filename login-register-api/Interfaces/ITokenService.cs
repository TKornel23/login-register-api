using login_register_api.Models;

namespace login_register_api.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
