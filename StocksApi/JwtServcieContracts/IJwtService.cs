using StocksApi.DTO;
using StocksApi.DatabaseContext;
using StocksApi.DTO;
using System.Security.Claims;

namespace StocksApi.ServiceContracts
{
    public interface IJwtService
    {
        Task<AuthenticationResponse> CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
