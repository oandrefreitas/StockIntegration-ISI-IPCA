using Microsoft.AspNetCore.Mvc;
using RESTfullStock.Models;
using SOAPServiceReference;

namespace RESTfullStock.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de produtos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly ServiceClient _soapClient;

        /// <summary>
        /// Inicializa uma nova instância do controlador de produtos.
        /// </summary>
        public ProdutosController()
        {
            _soapClient = new ServiceClient(); // Cliente SOAP gerado
        }

        /// <summary>
        /// Obtém todos os produtos disponíveis.
        /// </summary>
        /// <returns>Lista de produtos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProdutos()
        {
            try
            {
                var produtosSoap = await _soapClient.GetAllProdutosAsync();
                var produtos = produtosSoap.Select(p => new ProdutoModel
                {
                    ProdutoID = p.ProdutoID,
                    ProdutoNome = p.ProdutoNome,
                    Descricao = p.Descricao,
                    Preco = p.Preco,
                    Stock = p.Stock,
                    Estado = p.Estado
                }).ToList();

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter produtos", erro = ex.Message });
            }
        }

        /// <summary>
        /// Adiciona um novo produto.
        /// </summary>
        /// <param name="produto">Dados do produto a ser adicionado.</param>
        /// <returns>Status da operação.</returns>
        [HttpPost("CriarProduto")]
        public async Task<IActionResult> AddProduto([FromBody] ProdutoModel produto)
        {
            try
            {
                if (produto == null || string.IsNullOrEmpty(produto.ProdutoNome))
                {
                    return BadRequest(new { mensagem = "Dados inválidos para criação do produto." });
                }

                var produtoSoap = new SOAPServiceReference.Produto
                {
                    ProdutoNome = produto.ProdutoNome,
                    Descricao = produto.Descricao,
                    Preco = produto.Preco,
                    Stock = produto.Stock,
                    Estado = produto.Estado
                };

                var resultado = await _soapClient.AddProdutoAsync(produtoSoap);

                if (resultado)
                {
                    return Ok(new { mensagem = "Produto criado com sucesso." });
                }
                else
                {
                    return BadRequest(new { mensagem = "Erro ao criar produto." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza os dados de um produto existente.
        /// </summary>
        /// <param name="produto">Dados do produto a ser atualizado.</param>
        /// <returns>Status da operação.</returns>
        [HttpPost("AtualizarProduto")]
        public async Task<IActionResult> UpdateProduto([FromBody] ProdutoModel produto)
        {
            try
            {
                if (produto == null || produto.ProdutoID <= 0)
                {
                    return BadRequest(new { mensagem = "Dados inválidos para atualização do produto." });
                }

                var produtoSoap = new SOAPServiceReference.Produto
                {
                    ProdutoID = produto.ProdutoID,
                    ProdutoNome = produto.ProdutoNome,
                    Descricao = produto.Descricao,
                    Preco = produto.Preco,
                    Stock = produto.Stock,
                    Estado = produto.Estado
                };

                var resultado = await _soapClient.UpdateProdutoAsync(produtoSoap);

                if (resultado)
                {
                    return Ok(new { mensagem = "Produto atualizado com sucesso." });
                }
                else
                {
                    return NotFound(new { mensagem = "Produto não encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro interno.", erro = ex.Message });
            }
        }
    }
}
