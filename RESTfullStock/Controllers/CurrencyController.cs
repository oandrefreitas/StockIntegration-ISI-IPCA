using Microsoft.AspNetCore.Mvc;
using RESTfullStock.Services;

namespace RESTfullStock.Controllers
{
    /// <summary>
    /// Controlador responsável por lidar com operações relacionadas a taxas de câmbio.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _currencyService;

        /// <summary>
        /// Inicializa uma nova instância do controlador de moedas.
        /// </summary>
        /// <param name="currencyService">Serviço responsável por consultar taxas de câmbio.</param>
        public CurrencyController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        /// <summary>
        /// Obtém a taxa de câmbio de uma moeda específica em relação ao EUR.
        /// </summary>
        /// <param name="currency">Código da moeda desejada (ex.: USD, GBP).</param>
        /// <returns>Um objeto contendo os detalhes da taxa de câmbio.</returns>
        /// <response code="200">Retorna a taxa de câmbio com sucesso.</response>
        /// <response code="400">Retorna um erro caso a moeda não seja encontrada ou a requisição falhe.</response>
        [HttpGet("TaxaCambio")]
        [ProducesResponseType(typeof(CurrencyRate), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetExchangeRate(string currency)
        {
            try
            {
                var currencyRate = await _currencyService.GetCurrencyValueInEuroAsync(currency);
                return Ok(currencyRate); // Retorna o objeto com os detalhes da moeda
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
