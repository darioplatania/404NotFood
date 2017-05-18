using System;
using Microsoft.SPOT;

namespace fez_spider
{
    class Product
    {
        public int id { get; set; }
        public string nome { get; set; }
        public double prezzo { get; set; }
        public int quantita { get; set; }

        public Product(int i, string n, double p, int q)
        {
            id = i;
            nome = n;
            prezzo = p;
            quantita = q;
            
        }
    }
}

