using System;
using Microsoft.SPOT;

namespace fez_spider
{
    class Product
    {
        public string id { get; set; }
        public string nome { get; set; }
        public Double prezzo { get; set; }
        public string ingredients { get; set; }

        public Product(string i, string n, Double p,string ing)
        {
            id = i;
            nome = n;
            prezzo = p;
            ingredients = ing;

        }


        //TODO DA RIMUOVERE
        public Product(string i, string n, Double p, int q)
        {
            id = i;
            nome = n;
            prezzo = p;
            
        }
    }
}

