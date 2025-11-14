using System.ComponentModel.DataAnnotations;

namespace RESTfullStock.Models
{
    /// <summary>
    /// Representa um fornecedor no sistema RESTful API.
    /// </summary>
    public class FornecedorModel
    {
    

        public int FornecedorID { get; set; }

        /// <summary>
        /// Nome do fornecedor.
        /// </summary>
        [MaxLength(100, ErrorMessage = "O nome do fornecedor deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Número de contacto do fornecedor.
        /// </summary>
        [Range(100000000, 999999999, ErrorMessage = "O contacto deve conter exatamente 9 dígitos.")]
        public int Contacto { get; set; }

        /// <summary>
        /// Endereço ou localização do fornecedor.
        /// </summary>
        [MaxLength(200, ErrorMessage = "A morada deve ter no máximo 200 caracteres.")]
        public string Morada { get; set; }
    }
}