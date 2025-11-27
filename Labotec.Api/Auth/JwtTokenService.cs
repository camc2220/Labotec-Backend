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
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            // 1. Obtener Claims de la base de datos (aquí viene el patientId)
            var userClaims = await userManager.GetClaimsAsync(user);

            // 2. Verificar si es paciente usando la constante correcta
            var isPatient = userClaims.Any(c => c.Type == AppClaims.PatientId);

            // 3. Obtener Roles
            var roles = await userManager.GetRolesAsync(user);

            // 4. Si es paciente, ocultamos el rol Admin para evitar confusiones de seguridad en el frontend
            var filteredRoles = isPatient
                ? roles.Where(r => !string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase))
                : roles;

            claims.AddRange(filteredRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            // 5. Agregar los claims del usuario (incluyendo patientId) al token final
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