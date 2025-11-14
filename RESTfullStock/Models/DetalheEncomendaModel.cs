namespace RESTfullStock.Models
{
    /// <summary>
    /// Modelo que representa um detalhe de uma encomenda no sistema RESTful.
    /// </summary>
    public class DetalheEncomendaModel
    {
        public int ProdutoID { get; set; }
        public int Quantidade { get; set; }
        public float PrecoTotal { get; set; }
    }
}