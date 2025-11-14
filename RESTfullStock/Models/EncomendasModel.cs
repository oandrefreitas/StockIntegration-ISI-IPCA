

namespace RESTfullStock.Models
{
    /// <summary>
    /// Modelo que representa uma encomenda no sistema RESTful.
    /// </summary>
    public class EncomendaModel
    {
        public int EncomendaID { get; set; }
        public DateTime Data { get; set; }
        public int FornecedorID { get; set; }
        public int UtilizadorID { get; set; }
        public string Estado { get; set; }
        public List<DetalheEncomendaModel> Detalhes { get; set; }

        public EncomendaModel()
        {
            Detalhes = new List<DetalheEncomendaModel>();
        }
    }
}