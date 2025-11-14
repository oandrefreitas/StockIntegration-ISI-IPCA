using System;
using System.Collections.Generic;

namespace StockClient.Models
{
    /// <summary>
    /// Representa um movimento de stock no sistema.
    /// </summary>
    public class MovimentoModel
    {
        public int MovimentoID { get; set; }
        public DateTime Data { get; set; }
        public int ProdutoID { get; set; }
        public int UtilizadorID { get; set; }
        public string TipoInOut { get; set; } // "I" para entrada, "O" para saída
        public int Quantidade { get; set; }
    }

    /// <summary>
    /// Representa o modelo detalhado de um movimento de stock.
    /// </summary>
    public class MovimentoDetalhadoModel
    {
        public int MovimentoID { get; set; }
        public DateTime Data { get; set; }
        public int ProdutoID { get; set; }
        public string ProdutoNome { get; set; } // Nome do produto (opcional, para exibição)
        public int UtilizadorID { get; set; }
        public string UtilizadorNome { get; set; } // Nome do utilizador (opcional, para exibição)
        public string TipoInOut { get; set; }
        public int Quantidade { get; set; }
    }

    /// <summary>
    /// Representa os dados necessários para criar um novo movimento de stock.
    /// </summary>
    public class NovoMovimentoModel
    {
        public DateTime Data { get; set; }
        public int ProdutoID { get; set; }
        public int UtilizadorID { get; set; }
        public string TipoInOut { get; set; }
        public int Quantidade { get; set; }
    }

    /// <summary>
    /// Representa um modelo agregado para calcular o stock total de um produto.
    /// </summary>
    public class MovimentoStockAgregadoModel
    {
        public int ProdutoID { get; set; }
        public string ProdutoNome { get; set; } // Nome do produto (opcional)
        public int StockAtual { get; set; } // Soma de entradas menos saídas
    }
}
