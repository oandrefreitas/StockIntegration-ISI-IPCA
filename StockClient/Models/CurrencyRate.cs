using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockClient.Models
{
    public class CurrencyRate
    {
        public string Currency { get; set; } // Código da moeda (ex.: USD, GBP)
        public decimal Rate { get; set; } // Taxa de câmbio em relação ao EUR
        public DateTime RetrievedAt { get; set; } // Data e hora da consulta
    }
}
