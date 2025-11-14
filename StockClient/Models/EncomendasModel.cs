using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace StockClient.Models
{
    public class EncomendaModel
    {
    
        public int EncomendaID { get; set; }
        public DateTime Data { get; set; }
        public int FornecedorID { get; set; }
        public int UtilizadorID { get; set; }
        public string Estado { get; set; }
        public List<DetalheEncomendaModel> Detalhes { get; set; }
    }

    public class DetalheEncomendaModel
    {
    
        public int ProdutoID { get; set; }
        public int Quantidade { get; set; }
        public float PrecoTotal { get; set; }
    }

    public class EncomendaDetalhadaModel
    {
        public int EncomendaID { get; set; }
        public DateTime Data { get; set; }
        public int FornecedorID { get; set; }
        public int UtilizadorID { get; set; }
        public string Estado { get; set; }
        public int ProdutoID { get; set; }
        public int Quantidade { get; set; }
        public float PrecoTotal { get; set; }
    }

    public class NovaEncomendaModel
    {
        public DateTime Data { get; set; }
        public int FornecedorID { get; set; }
        public int UtilizadorID { get; set; }
        public string Estado { get; set; }
        public List<DetalheEncomendaModel> Detalhes { get; set; }
    }



}
