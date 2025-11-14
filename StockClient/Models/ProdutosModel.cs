using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockClient.Models
{
    public class Produto
    {
        public int ProdutoID { get; set; }
        public string ProdutoNome { get; set; }
        public string Descricao { get; set; }
        public float Preco { get; set; }
        public int Stock { get; set; }
        public string Estado { get; set; }
    }
}

