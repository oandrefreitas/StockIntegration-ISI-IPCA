using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace RESTfullStock.Services
{
    /// <summary>
    /// Serviço responsável por gerir tokens JWT.
    /// </summary>
    public class AuthService
    {
        private readonly string _privateKey;
        private readonly int _tokenExpirationMinutes;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="AuthService"/> para gestão de tokens JWT.
        /// </summary>
        /// <param name="privateKey">
        /// A chave secreta usada para assinar e validar tokens JWT.
        /// Deve ser mantida segura e consistente em todas as instâncias do serviço.
        /// </param>
        /// <param name="tokenExpirationMinutes">
        /// O tempo de expiração dos tokens JWT, em minutos.
        /// Após esse período, o token será considerado inválido.
        /// </param>
        public AuthService(string privateKey, int tokenExpirationMinutes)
        {
            _privateKey = privateKey;
            _tokenExpirationMinutes = tokenExpirationMinutes;
        }

        /// <summary>
        /// Gera um token JWT com base nas informações do utilizador.
        /// </summary>
        /// <param name="email">Email do utilizador.</param>
        /// <param name="roles">Lista de roles do utilizador.</param>
        /// <returns>Token JWT como string.</returns>
        public string GenerateToken(string email, string[] roles)
        {
            var key = Encoding.ASCII.GetBytes(_privateKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Valida um token JWT e retorna os claims se válido.
        /// </summary>
        /// <param name="token">Token JWT a ser validado.</param>
        /// <returns>ClaimsPrincipal contendo os dados do token, ou null se inválido.</returns>
        public ClaimsPrincipal ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_privateKey);

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Opcional: configurar emissor específico
                    ValidateAudience = false, // Opcional: configurar audiência específica
                    ValidateLifetime = true, // Valida expiração
                    ClockSkew = TimeSpan.Zero // Sem tolerância para expiração
                };

                var principal = handler.ValidateToken(token, tokenValidationParameters, out _);
                return principal;
            }
            catch
            {
                return null; // Token inválido
            }
        }
    }
}
