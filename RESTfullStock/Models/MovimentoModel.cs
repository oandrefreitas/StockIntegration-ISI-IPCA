namespace RESTfullStock.Models
{
    /// <summary>
    /// Modelo que representa um movimento de stock no sistema RESTful.
    /// </summary>
    public class MovimentoModel
    {
        /// <summary>
        /// Identificador único do movimento.
        /// </summary>
        public int MovimentoID { get; set; }

        /// <summary>
        /// Identificador do produto relacionado ao movimento.
        /// </summary>
        public int ProdutoID { get; set; }

        /// <summary>
        /// Data do movimento.
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Tipo do movimento (entrada "I" ou saída "O").
        /// </summary>
        public string TipoInOut { get; set; }

        /// <summary>
        /// Quantidade movimentada.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Identificador do utilizador que realizou o movimento.
        /// </summary>
        public int UtilizadorID { get; set; }
    }
}
