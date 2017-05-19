using System;
using Microsoft.SPOT;

namespace fez_spider
{
    class Product
    {
        public Double id { get; set; }
        public string nome { get; set; }
        public Double prezzo { get; set; }
        public int quantita { get; set; }

        public Product(Double i, string n, Double p, int q)
        {
            id = i;
            nome = n;
            prezzo = p;
            quantita = q;
            
        }
    }
}

