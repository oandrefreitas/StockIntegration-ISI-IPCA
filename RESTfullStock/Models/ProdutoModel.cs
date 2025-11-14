namespace RESTfullStock.Models
{
    /// <summary>
    /// Representa um produto no sistema RESTful API.
    /// </summary>
    public class ProdutoModel
    {
        /// <summary>
        /// Identificador único do produto.
        /// </summary>
        public int ProdutoID { get; set; }

        /// <summary>
        /// Nome do produto.
        /// </summary>
        public string ProdutoNome { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Preço do produto.
        /// </summary>
        public float Preco { get; set; }

        /// <summary>
        /// Stock disponível do produto.
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        /// Estado do produto (Ativo, Inativo, etc.).
        /// </summary>
        public string Estado { get; set; }
    }
}
