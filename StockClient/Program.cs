using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockClient
{
    static class Program
    {
        public static string JwtToken { get; set; } // Propriedade para armazenar o token JWT

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Exibir o formulário de login
            using (var loginForm = new Login())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Form1()); // Inicia o formulário principal após login bem-sucedido
                }
                else
                {
                    Application.Exit(); // Fecha a aplicação se o login for cancelado
                }
            }
        }
    }
}