using Microsoft.AspNetCore.Mvc;
using RESTfullStock.Services; // O AuthService que gera os tokens
using SOAPServiceReference; // Para validar no SOAP
using RESTfullStock.Models;

namespace RESTfullStock.Controllers
{
    /// <summary>
    /// Controlador responsável por autenticação e gestão de utilizadores.
    /// Fornece endpoints para login e registro de utilizadores.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ServiceClient _soapClient;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="AuthController"/>.
        /// </summary>
        /// <param name="authService">Instância do serviço responsável pela geração de tokens JWT.</param>
        public AuthController(AuthService authService)
        {
            _authService = authService;
            _soapClient = new ServiceClient(); // Cliente SOAP
        }

        /// <summary>
        /// Login do utilizador: valida credenciais e gera token JWT.
        /// </summary>
        /// <param name="loginRequest">
        /// Objeto contendo as credenciais do utilizador (email e senha).
        /// </param>
        /// <returns>
        /// Um objeto JSON contendo o token JWT, o role e o ID do utilizador, caso as credenciais sejam válidas.
        /// Retorna <see cref="UnauthorizedResult"/> se as credenciais forem inválidas ou o utilizador não for encontrado.
        /// </returns>
        /// <response code="200">Retorna o token JWT, o role e o ID do utilizador.</response>
        /// <response code="401">Credenciais inválidas ou utilizador não encontrado.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            // Valida as credenciais usando o método SOAP ValidateUser
            var isValidUser = await _soapClient.ValidateUserAsync(loginRequest.Email, loginRequest.Password);
            if (!isValidUser)
            {
                return Unauthorized(new { mensagem = "Credenciais inválidas." });
            }

            // Obtém os detalhes do utilizador pelo email
            var userDetails = await _soapClient.GetUserByEmailAsync(loginRequest.Email);
            if (userDetails == null)
            {
                return Unauthorized(new { mensagem = "Utilizador não encontrado." });
            }

            // Gera o token JWT usando o AuthService
            var token = _authService.GenerateToken(userDetails.Email, new[] { userDetails.Role });

            // Retorna o token e o role do utilizador
            return Ok(new
            {
                Token = token,
                UserId = userDetails.Id 
            });
        }

        /// <summary>
        /// Registra um novo utilizador no sistema.
        /// </summary>
        /// <param name="newUser">
        /// Objeto contendo os dados do novo utilizador a ser registrado (nome, email, senha, e roles).
        /// </param>
        /// <returns>
        /// Retorna uma mensagem indicando o sucesso ou falha do registro.
        /// Retorna <see cref="BadRequestObjectResult"/> se houver erro ao registrar o utilizador.
        /// </returns>
        /// <response code="200">Utilizador registrado com sucesso.</response>
        /// <response code="400">Erro ao registrar o utilizador.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RestUser newUser)
        {
            // Adiciona o novo utilizador usando o método SOAP AddUser
            var isAdded = await _soapClient.AddUserAsync(
                newUser.Nome,
                newUser.Email,
                newUser.Password,
                newUser.Roles.FirstOrDefault() ?? "user" // Papel padrão se não especificado
            );

            if (!isAdded)
            {
                return BadRequest(new { mensagem = "Erro ao registrar utilizador." });
            }

            return Ok(new { mensagem = "Utilizador registrado com sucesso." });
        }
    }
}
