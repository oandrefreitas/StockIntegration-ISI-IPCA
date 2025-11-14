using Microsoft.AspNetCore.Mvc;
using RESTfullStock.Models;
using SOAPServiceReference;

namespace RESTfullStock.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilizadoresController : ControllerBase
    {
        private readonly ServiceClient _soapClient;

        /// <summary>
        /// Construtor para inicializar o cliente SOAP.
        /// </summary>
        public UtilizadoresController()
        {
            _soapClient = new ServiceClient(); // Inicia o cliente SOAP
        }

        /// <summary>
        /// Obtém todos os utilizadores do sistema.
        /// </summary>
        /// <returns>Lista de utilizadores.</returns>
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                // Obtém os utilizadores do sistema através do cliente SOAP
                var usersSoap = await _soapClient.GetAllUsersAsync(); // Chama o método SOAP

                // Mapeia os utilizadores SOAP para o modelo RestUser
                var users = usersSoap.Select(u => new RestUser(
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Password,
                    new List<string> { u.Role } // Converte a role única para uma lista
                )).ToList();

                // Retorna a lista de utilizadores como resposta
                return Ok(users);
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna status 500 com a mensagem de erro
                return StatusCode(500, new { mensagem = "Erro ao obter utilizadores.", erro = ex.Message });
            }
        }
    }
}