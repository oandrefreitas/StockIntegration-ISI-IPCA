
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTfullStock.Models;
using SOAPServiceReference;

namespace RESTfullStock.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de fornecedores.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedoresController : ControllerBase
    {
        private readonly ServiceClient _soapClient;

        /// <summary>
        /// Inicializa uma nova instância do controlador de fornecedores.
        /// </summary>
        public FornecedoresController()
        {
            _soapClient = new ServiceClient(); // Cliente SOAP gerado
        }

        /// <summary>
        /// Obtém todos os fornecedores disponíveis.
        /// </summary>
        /// <returns>Lista de fornecedores.</returns>
        /// <response code="200">Retorna a lista de fornecedores.</response>
        /// <response code="500">Erro interno ao obter os fornecedores.</response>
        [HttpGet("GetFornecedores")]
        public async Task<IActionResult> GetAllFornecedores()
        {
            try
            {
                // Chama o método SOAP para obter todos os fornecedores
                var fornecedoresSoap = await _soapClient.GetAllFornecedoresAsync();

                // Converte para o modelo RESTful
                var fornecedores = fornecedoresSoap.Select(f => new FornecedorModel
                {
                    FornecedorID = f.FornecedorID,
                    Nome = f.FornecedorNome,
                    Contacto = f.Contacto,
                    Morada = f.Morada
                }).ToList();

                return Ok(fornecedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter fornecedores", erro = ex.Message });
            }
        }


        /// <summary>
        /// Adiciona um novo fornecedor.
        /// </summary>
        /// <param name="fornecedor">Dados do fornecedor a ser adicionado.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="200">Fornecedor adicionado com sucesso.</response>
        /// <response code="400">Dados inválidos para criação do fornecedor.</response>
        /// <response code="500">Erro interno ao criar o fornecedor.</response>
        [HttpPost("CriarFornecedor")]
        [Authorize(Policy = "AdminOrGestor")] 
        public async Task<IActionResult> AddFornecedor([FromBody] FornecedorModel fornecedor)
        {
            try
            {
                // Valida os dados recebidos
                if (fornecedor == null || string.IsNullOrEmpty(fornecedor.Nome))
                {
                    return BadRequest(new { mensagem = "Dados inválidos para criação do fornecedor." });
                }

                // Converte o modelo RESTful para o modelo SOAP
                var fornecedorSoap = new SOAPServiceReference.Fornecedor
                {
                    FornecedorNome = fornecedor.Nome,
                    Contacto = fornecedor.Contacto,
                    Morada = fornecedor.Morada
                };

                // Chama o método SOAP para criar o fornecedor
                var resultado = await _soapClient.AddFornecedorAsync(fornecedorSoap);

                if (resultado)
                {
                    return Ok(new { mensagem = "Fornecedor criado com sucesso." });
                }
                else
                {
                    return BadRequest(new { mensagem = "Erro ao criar fornecedor." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um fornecedor existente.
        /// </summary>
        /// <param name="fornecedor">Dados do fornecedor a ser atualizado.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="200">Fornecedor atualizado com sucesso.</response>
        /// <response code="400">Dados inválidos para atualização do fornecedor.</response>
        /// <response code="404">Fornecedor não encontrado.</response>
        /// <response code="500">Erro interno ao atualizar o fornecedor.</response>
        [HttpPost("AtualizarFornecedor")]
        [Authorize(Policy = "AdminOrGestor")]
        public async Task<IActionResult> UpdateFornecedor([FromBody] FornecedorModel fornecedor)
        {
            try
            {
                if (fornecedor == null || fornecedor.FornecedorID <= 0)
                {
                    return BadRequest(new { mensagem = "Dados inválidos para atualização do fornecedor." });
                }

                // Converte o modelo RESTful para o modelo SOAP
                var fornecedorSoap = new SOAPServiceReference.Fornecedor
                {
                    FornecedorID = fornecedor.FornecedorID,
                    FornecedorNome = fornecedor.Nome,
                    Contacto = fornecedor.Contacto,
                    Morada = fornecedor.Morada
                };

                // Chama o método SOAP para atualizar o fornecedor
                var resultado = await _soapClient.UpdateFornecedorAsync(fornecedorSoap);

                if (resultado)
                {
                    return Ok(new { mensagem = "Fornecedor atualizado com sucesso." });
                }
                else
                {
                    return NotFound(new { mensagem = "Fornecedor não encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Remove um fornecedor com base no seu identificador.
        /// </summary>
        /// <param name="fornecedor">Objeto contendo o ID do fornecedor a ser removido.</param>
        /// <returns>Retorna o status da operação.</returns>
        /// <response code="200">Fornecedor removido com sucesso.</response>
        /// <response code="400">Dados inválidos na requisição.</response>
        /// <response code="404">Fornecedor não encontrado.</response>
        [HttpPost("RemoverFornecedor")]
        [Authorize(Policy = "AdminOrGestor")]
        public async Task<IActionResult> RemoveFornecedor([FromBody] FornecedorModel fornecedor)
        {
            try
            {
                if (fornecedor.FornecedorID <= 0)
                {
                    return BadRequest(new { mensagem = "ID de fornecedor inválido." });
                }
                // Chama o método SOAP para remover o fornecedor
                var resultado = await _soapClient.DeleteFornecedorAsync(fornecedor.FornecedorID);
                if (resultado)
                {
                    return Ok(new { mensagem = "Fornecedor removido com sucesso." });
                }
                else
                {
                    return NotFound(new { mensagem = "Fornecedor não encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno.", erro = ex.Message });
            }
        }


    }
}