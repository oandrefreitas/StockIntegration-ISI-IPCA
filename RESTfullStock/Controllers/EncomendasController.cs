using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTfullStock.Models;
using SOAPServiceReference;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfullStock.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de encomendas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EncomendasController : ControllerBase
    {
        private readonly ServiceClient _soapClient;

        /// <summary>
        /// Inicializa uma nova instância do controlador de encomendas.
        /// </summary>
        public EncomendasController()
        {
            _soapClient = new ServiceClient(); // Cliente SOAP gerado
        }

        /// <summary>
        /// Cria uma nova encomenda no sistema.
        /// </summary>
        /// <param name="novaEncomenda">Objeto contendo os dados da encomenda.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="201">Encomenda criada com sucesso.</response>
        /// <response code="400">Os dados fornecidos são inválidos.</response>
        /// <response code="401">O utilizador não está autenticado ou autorizado.</response>
        /// <response code="500">Ocorreu um erro interno no servidor ao criar a encomenda.</response>
        [HttpPost ("AdicionarEncomenda")]
        [Authorize(Policy = "Gestor")] 
        public async Task<IActionResult> CreateEncomenda([FromBody] CreateEncomendaModel novaEncomenda)
        {
            try
            {
                // Valida os dados recebidos
                if (novaEncomenda == null || novaEncomenda.FornecedorID <= 0 || novaEncomenda.UtilizadorID <= 0)
                {
                    return BadRequest(new { mensagem = "Dados inválidos para criação da encomenda." });
                }

                // Converte o modelo RESTful para o modelo SOAP
                var encomendaSoap = new SOAPServiceReference.Encomenda
                {
                    Data = novaEncomenda.Data,
                    FornecedorID = novaEncomenda.FornecedorID,
                    UtilizadorID = novaEncomenda.UtilizadorID,
                    Estado = novaEncomenda.Estado,
                    Detalhes = novaEncomenda.Detalhes?.Select(d => new SOAPServiceReference.DetalheEncomenda
                    {
                        ProdutoID = d.ProdutoID,
                        Quantidade = d.Quantidade,
                        PrecoTotal = d.PrecoTotal
                    }).ToArray()
                };

                // Chama o método SOAP para criar a encomenda
                var sucesso = await _soapClient.AddEncomendaAsync(encomendaSoap);

                if (sucesso)
                {
                    return StatusCode(201, new { mensagem = "Encomenda criada com sucesso." });
                }
                else
                {
                    return BadRequest(new { mensagem = "Erro ao criar encomenda." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno ao criar encomenda.", erro = ex.Message });
            }
        }
        /// <summary>
        /// Atualiza o estado de uma encomenda existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite atualizar o estado de uma encomenda para "P" (Pedida) ou "E" (Entregue).
        /// 
        /// Exemplo de corpo da requisição:
        /// 
        /// "E"
        /// </remarks>
        /// <param name="id">ID da encomenda a ser atualizada.</param>
        /// <param name="novoEstado">Novo estado da encomenda ("P" ou "E").</param>
        /// <returns>Status da operação.</returns>
        /// <response code="200">Estado da encomenda atualizado com sucesso.</response>
        /// <response code="400">O estado fornecido é inválido.</response>
        /// <response code="404">A encomenda não foi encontrada.</response>
        /// <response code="500">Erro interno ao atualizar o estado da encomenda.</response>
        [HttpPut("AtualizaEstado")]
        public async Task<IActionResult> UpdateEstado(int id, [FromBody] string novoEstado)
        {
            try
            {
                // Valida o novo estado (somente "P" ou "E" são aceitos)
                if (string.IsNullOrEmpty(novoEstado) || (novoEstado != "P" && novoEstado != "E"))
                {
                    return BadRequest(new { mensagem = "Estado inválido. Deve ser 'P' (Pedida) ou 'E' (Entregue)." });
                }

                // Chama a função SOAP para atualizar o estado
                var sucesso = await Task.Run(() => _soapClient.UpdateEncomendaEstadoAsync(id, novoEstado));

                if (sucesso)
                {
                    return Ok(new { mensagem = "Estado da encomenda atualizado com sucesso." });
                }
                else
                {
                    return NotFound(new { mensagem = $"Encomenda com ID {id} não encontrada." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno ao atualizar o estado da encomenda.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém uma encomenda específica pelo ID.
        /// </summary>
        /// <param name="id">ID da encomenda a ser obtida.</param>
        /// <returns>Dados da encomenda solicitada.</returns>
        /// <response code="200">Encomenda encontrada e retornada com sucesso.</response>
        /// <response code="404">A encomenda com o ID fornecido não foi encontrada.</response>
        /// <response code="500">Erro interno ao obter a encomenda.</response>
        [HttpGet("EncomendasPorId")]
        public async Task<IActionResult> GetEncomendaById(int id)
        {
            try
            {
                // Chama o método SOAP para obter a encomenda pelo ID
                var encomendaSoap = await Task.Run(() => _soapClient.GetEncomendaByIdAsync(id));

                if (encomendaSoap == null)
                {
                    return NotFound(new { mensagem = $"Encomenda com ID {id} não encontrada." });
                }

                // Converte o modelo SOAP para o modelo RESTful
                var encomenda = new EncomendaModel
                {
                    EncomendaID = encomendaSoap.EncomendaID,
                    Data = encomendaSoap.Data,
                    FornecedorID = encomendaSoap.FornecedorID,
                    UtilizadorID = encomendaSoap.UtilizadorID,
                    Estado = encomendaSoap.Estado,
                    Detalhes = encomendaSoap.Detalhes?.Select(d => new DetalheEncomendaModel
                    {
                        ProdutoID = d.ProdutoID,
                        Quantidade = d.Quantidade,
                        PrecoTotal = d.PrecoTotal
                    }).ToList()
                };

                return Ok(encomenda);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno ao obter a encomenda.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém a lista de todas as encomendas disponíveis.
        /// </summary>
        /// <returns>Lista de encomendas.</returns>
        /// <response code="200">Lista de encomendas retornada com sucesso.</response>
        /// <response code="500">Erro interno ao obter as encomendas.</response>
        [HttpGet("TodasEncomendas")]
        public async Task<IActionResult> GetAllEncomendas()
        {
            try
            {
                // Chama o método SOAP para obter todas as encomendas
                var encomendasSoap = await Task.Run(() => _soapClient.GetAllEncomendasAsync());

                if (encomendasSoap == null || !encomendasSoap.Any())
                {
                    return Ok(new List<EncomendaModel>()); // Retorna lista vazia
                }

                // Converte os modelos SOAP para modelos RESTful
                var encomendas = encomendasSoap.Select(e => new EncomendaModel
                {
                    EncomendaID = e.EncomendaID,
                    Data = e.Data,
                    FornecedorID = e.FornecedorID,
                    UtilizadorID = e.UtilizadorID,
                    Estado = e.Estado,
                    Detalhes = e.Detalhes?.Select(d => new DetalheEncomendaModel
                    {
                        ProdutoID = d.ProdutoID,
                        Quantidade = d.Quantidade,
                        PrecoTotal = d.PrecoTotal
                    }).ToList()
                }).ToList();

                return Ok(encomendas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno ao obter as encomendas.", erro = ex.Message });
            }
        }






    }





}
