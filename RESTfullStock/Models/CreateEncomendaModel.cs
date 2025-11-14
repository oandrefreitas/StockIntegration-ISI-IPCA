using System;
using System.Collections.Generic;

namespace RESTfullStock.Models
{
    /// <summary>
    /// Modelo para criação de uma encomenda.
    /// </summary>
    public class CreateEncomendaModel
    {
        public DateTime Data { get; set; }
        public int FornecedorID { get; set; }
        public int UtilizadorID { get; set; }
        public string Estado { get; set; }
        public List<DetalheEncomendaModel> Detalhes { get; set; }

        public CreateEncomendaModel()
        {
            Detalhes = new List<DetalheEncomendaModel>();
        }
    }
}
