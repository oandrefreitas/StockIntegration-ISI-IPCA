using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RESTfullStock.Services
{
    /// <summary>
    /// Serviço para consultar o valor de 1 unidade de uma moeda em EUR.
    /// </summary>
    public class CurrencyService
    {
        private readonly HttpClient _httpClient; //definido no program.cs
        private readonly string _baseUrl; //definido no appsettings.json
        private readonly string _accessKey; //definido no appsettings.json

        /// <summary>
        /// Inicializa uma nova instância do <see cref="CurrencyService"/>.
        /// </summary>
        /// <param name="httpClient">Instância de <see cref="HttpClient"/> para realizar requisições HTTP.</param>
        /// <param name="configuration">Instância de <see cref="IConfiguration"/> para acessar configurações da API.</param>
        public CurrencyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["CurrencyApiSettings:BaseUrl"];
            _accessKey = configuration["CurrencyApiSettings:AccessKey"];
        }

        /// <summary>
        /// Consulta o valor de 1 unidade de uma moeda em EUR.
        /// </summary>
        /// <param name="currency">Código da moeda desejada (ex.: USD).</param>
        /// <returns>Uma instância de <see cref="CurrencyRate"/> contendo a taxa de câmbio e informações adicionais.</returns>
        /// <exception cref="HttpRequestException">Lançada em caso de erro durante a requisição ou se a moeda não for encontrada.</exception>
        public async Task<CurrencyRate> GetCurrencyValueInEuroAsync(string currency)
        {
            currency = currency.ToUpperInvariant(); // Normaliza para maiúsculas
            var url = $"{_baseUrl}/latest?access_key={_accessKey}&symbols={currency}";

            try
            {
                // Faz a requisição à API
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Erro HTTP: {response.StatusCode}");
                }

                // Lê e processa o conteúdo da resposta
                var content = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(content);
                var root = jsonDoc.RootElement;

                // Verifica se a moeda está presente nos dados retornados
                if (root.TryGetProperty("rates", out var rates) &&
                    rates.TryGetProperty(currency, out var rate))
                {
                    var exchangeRate = rate.GetDecimal(); // Taxa obtida
                    return new CurrencyRate(currency, exchangeRate); // Retorna o objeto CurrencyRate
                }
                else
                {
                    throw new HttpRequestException($"Erro: Taxa de câmbio para {currency} não encontrada.");
                }
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Erro durante a conversão: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Representa a taxa de câmbio de uma moeda em relação ao EUR.
    /// </summary>
    public class CurrencyRate
    {
        /// <summary>
        /// Código da moeda (ex.: USD, GBP, JPY).
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Taxa de câmbio em relação ao EUR.
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// Data e hora da consulta.
        /// </summary>
        public DateTime RetrievedAt { get; set; }

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CurrencyRate"/>.
        /// </summary>
        /// <param name="currency">Código da moeda.</param>
        /// <param name="rate">Taxa de câmbio.</param>
        public CurrencyRate(string currency, decimal rate)
        {
            Currency = currency;
            Rate = rate;
            RetrievedAt = DateTime.UtcNow; // Define o momento da consulta
        }
    }
}
