using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockClient.Models
{
    public class Supplier
    {
        public int FornecedorID { get; set; }
        public string Nome { get; set; }
        public int Contacto { get; set; }
        public string Morada { get; set; }
    }
}