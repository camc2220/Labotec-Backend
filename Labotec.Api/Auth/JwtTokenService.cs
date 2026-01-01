using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Labotec.Api.Auth
{
    public class JwtTokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<string> CreateAsync(IdentityUser user, UserManager<IdentityUser> userManager)
        {
            var claims = new List<Claim>
            {
                // ✅ Esto hace tu vida más fácil en controllers: User.FindFirstValue(ClaimTypes.NameIdentifier)
                new Claim(ClaimTypes.NameIdentifier, user.Id),

                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

<<<<<<< Updated upstream
<<<<<<< Updated upstream
            // 1. Obtener Claims de la base de datos (aqu viene el patientId)
            var userClaims = await userManager.GetClaimsAsync(user);

            // 2. Obtener Roles sin filtrar para permitir combinaciones (p. ej. Admin + Paciente)
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // 3. Agregar los claims del usuario (incluyendo patientId) al token final
=======
            // Claims de la BD (aquí viene patientId)
            var userClaims = await userManager.GetClaimsAsync(user);

            // Roles
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Agregar claims extra (patientId, etc.)
>>>>>>> Stashed changes
=======
            // Claims de la BD (aquí viene patientId)
            var userClaims = await userManager.GetClaimsAsync(user);

            // Roles
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            // Agregar claims extra (patientId, etc.)
>>>>>>> Stashed changes
            claims.AddRange(userClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _settings.Issuer,
                _settings.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
