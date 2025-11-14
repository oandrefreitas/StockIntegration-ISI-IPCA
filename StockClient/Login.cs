using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;


namespace StockClient
{
    public partial class Login : Form
    {

 
        public Login()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, preencha ambos os campos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var loginData = new
                {
                    Email = username,
                    Password = password
                };

                var jsonContent = JsonSerializer.Serialize(loginData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await GlobalConfig.HttpClient.PostAsync("auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                   
                    // Armazena o token no GlobalConfig
                    GlobalConfig.JwtToken = tokenResponse.Token;
                    GlobalConfig.UserID = tokenResponse.UserId;

                    MessageBox.Show("Login bem-sucedido!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Credenciais inválidas.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar com a API: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Modelo para deserializar a resposta da API contendo o token JWT.
        /// </summary>
        private class TokenResponse
        {
            public string Token { get; set; }
            public int UserId { get; set; }

        }

    }
}

