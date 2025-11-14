using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace StockClient
{
    public static class GlobalConfig
    {
        /// <summary>
        /// Instância global do HttpClient compartilhada no projeto.
        /// </summary>
        public static HttpClient HttpClient { get; } = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7292/api/") // Configuração da URL base
        };

        private static string _jwtToken;

        /// <summary>
        /// Token JWT atual usado para autenticação.
        /// </summary>
        public static string JwtToken
        {
            get => _jwtToken;
            set
            {
                _jwtToken = value;

                // Configura automaticamente o cabeçalho de autorização no HttpClient
                if (!string.IsNullOrEmpty(_jwtToken))
                {
                    HttpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _jwtToken);
                }
                else
                {
                    HttpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
        }

        /// <summary>
        /// ID do utilizador autenticado.
        /// </summary>
        public static int UserID { get; set; } // Propriedade para armazenar o ID do utilizador
    }

}
