using Microsoft.AspNetCore.Mvc;
using RESTfullStock.Models;
using SOAPServiceReference;
using System.Threading.Tasks;
using System.Linq;

namespace RESTfullStock.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de movimentos de stock.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MovimentosController : ControllerBase
    {
        private readonly ServiceClient _soapClient;

        /// <summary>
        /// Inicializa uma nova instância do controlador de movimentos de stock.
        /// </summary>
        public MovimentosController()
        {
            _soapClient = new ServiceClient(); // Cliente SOAP gerado
        }

        /// <summary>
        /// Adiciona um novo movimento de stock.
        /// </summary>
        /// <param name="movimento">Objeto contendo os dados do movimento.</param>
        /// <returns>Status da operação.</returns>
        /// <response code="201">Movimento criado com sucesso.</response>
        /// <response code="400">Os dados fornecidos são inválidos.</response>
        /// <response code="500">Erro interno ao criar o movimento.</response>
        [HttpPost("AdicionarMovimento")]
        public async Task<IActionResult> AddMovimento([FromBody] MovimentoModel movimento)
        {
            try
            {
                // Valida os dados recebidos
                if (movimento == null || movimento.ProdutoID <= 0 || movimento.UtilizadorID <= 0 || movimento.Quantidade <= 0)
                {
                    return BadRequest(new { mensagem = "Dados inválidos para criação do movimento." });
                }

                // Converte o modelo RESTful para o modelo SOAP
                var movimentoSoap = new SOAPServiceReference.Movimento
                {
                    ProdutoID = movimento.ProdutoID,
                    Data = movimento.Data,
                    TipoInOut = movimento.TipoInOut,
                    Quantidade = movimento.Quantidade,
                    UtilizadorID = movimento.UtilizadorID
                };

                // Chama o método SOAP para criar o movimento
                var sucesso = await _soapClient.AddMovimentoAsync(movimentoSoap);

                if (sucesso)
                {
                    return StatusCode(201, new { mensagem = "Movimento criado com sucesso." });
                }
                else
                {
                    return BadRequest(new { mensagem = "Erro ao criar movimento." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno ao criar movimento.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os movimentos de stock.
        /// </summary>
        /// <returns>Lista de movimentos de stock.</returns>
        [HttpGet("TodosMovimentos")]
        public async Task<IActionResult> GetAllMovimentos()
        {
            try
            {
                var movimentosSoap = await _soapClient.GetAllMovimentosAsync();
                var movimentos = movimentosSoap.Select(m => new MovimentoModel
                {
                    MovimentoID = m.MovimentoID,
                    ProdutoID = m.ProdutoID,
                    UtilizadorID = m.UtilizadorID,
                    Data = m.Data,
                    TipoInOut = m.TipoInOut,
                    Quantidade = m.Quantidade
                }).ToList();

                return Ok(movimentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter movimentos.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém os movimentos filtrados por produto.
        /// </summary>
        /// <param name="produtoId">ID do produto.</param>
        /// <returns>Movimentos relacionados ao produto.</returns>
        [HttpGet("PorProduto/{produtoId}")]
        public async Task<IActionResult> GetMovimentosByProduto(int produtoId)
        {
            try
            {
                var movimentosSoap = await _soapClient.GetMovimentosByProdutoAsync(produtoId);
                var movimentos = movimentosSoap.Select(m => new MovimentoModel
                {
                    MovimentoID = m.MovimentoID,
                    ProdutoID = m.ProdutoID,
                    UtilizadorID = m.UtilizadorID,
                    Data = m.Data,
                    TipoInOut = m.TipoInOut,
                    Quantidade = m.Quantidade
                }).ToList();

                return Ok(movimentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao filtrar movimentos.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se existe stock suficiente para realizar uma saída.
        /// </summary>
        /// <param name="produtoId">ID do produto.</param>
        /// <param name="quantidade">Quantidade solicitada.</param>
        /// <returns>Confirmação da disponibilidade de stock.</returns>
        [HttpGet("VerificarStock/{produtoId}/{quantidade}")]
        public async Task<IActionResult> VerificarStockDisponivel(int produtoId, int quantidade)
        {
            try
            {
                var resultado = await _soapClient.VerificarStockDisponivelAsync(produtoId, quantidade);
                return Ok(new { stockDisponivel = resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao verificar stock.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Obtém o histórico de movimentos por utilizador.
        /// </summary>
        /// <param name="utilizadorId">ID do utilizador.</param>
        /// <returns>Histórico de movimentos realizados pelo utilizador.</returns>
        [HttpGet("HistoricoPorUtilizador/{utilizadorId}")]
        public async Task<IActionResult> GetHistoricoMovimentosByUtilizador(int utilizadorId)
        {
            try
            {
                var movimentosSoap = await _soapClient.GetHistoricoMovimentosByUtilizadorAsync(utilizadorId);
                var movimentos = movimentosSoap.Select(m => new MovimentoModel
                {
                    MovimentoID = m.MovimentoID,
                    ProdutoID = m.ProdutoID,
                    UtilizadorID = m.UtilizadorID,
                    Data = m.Data,
                    TipoInOut = m.TipoInOut,
                    Quantidade = m.Quantidade
                }).ToList();

                return Ok(movimentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter histórico de movimentos.", erro = ex.Message });
            }
        }
    }
}