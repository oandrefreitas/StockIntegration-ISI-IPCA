using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOAPServiceReference;

/// <summary>
/// Controlador para testar a ligação RESTful com o serviço SOAP.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "admin")]
public class TesteController : ControllerBase
{
    private readonly ServiceClient _soapClient;

    /// <summary>
    /// Inicializa uma nova instância do TesteController.
    /// </summary>
    public TesteController()
    {
        // Inicializa o cliente SOAP gerado pelo Connected Services.
        _soapClient = new ServiceClient();
    }

    /// <summary>
    /// Testa a conexão com o serviço SOAP para verificar se está acessível.
    /// </summary>
    /// <returns>Mensagem indicando sucesso ou erro na conexão.</returns>
    /// <response code="200">Conexão bem-sucedida com o serviço SOAP.</response>
    /// <response code="500">Erro ao tentar conectar ao serviço SOAP.</response>
    [HttpGet("testar-conexao")]
    public async Task<IActionResult> TestarConexao()
    {
        try
        {
            // Chama o método SOAP para testar a conexão
            string resultado = await _soapClient.TestarConexaoAsync();

            // Retorna a resposta em formato JSON
            return Ok(new { mensagem = resultado });
        }
        catch (Exception ex)
        {
            // Retorna erro com mensagem detalhada em caso de exceção
            return StatusCode(500, new { mensagem = "Erro ao testar conexão", erro = ex.Message });
        }
    }
}
