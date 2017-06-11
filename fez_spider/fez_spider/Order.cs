using System;
using Microsoft.SPOT;
using System.Collections;

namespace fez_spider
{

    class Orders
    {
        private ArrayList _list;
        private int       _total;
        private double    _price;

        public ArrayList List { get { return _list; } set { if (value != _list) _list = value; } }
        public int Total { get { return _total; }set { if (value != _total) _total = value; } }
        public double Price { get { return _price; }set { if (value != _price) _price = value; } }


        public Orders()
        {
            _list = new ArrayList();
        }

        public void printStatus()
        {
            foreach (Order o in _list)
                Debug.Print("Order: " + o.Product.nome + " Qty: " + o.Quantity);
        }
        
        public bool Contains(Product p)
        {
            foreach(Order o in _list)
            {
                if (o.Product.id.Equals(p.id))
                    return true;
            }

            return false;
        }

        public Order Get(Product p)
        {
            foreach (Order o in _list)
                if (o.Product.id.Equals(p.id))
                    return o;
            return null;

        }

        public Product GetProduct(String id)
        {
            
            foreach (Order o in _list)
                if (o.Product.id.Equals(id))
                    return o.Product;
            return null;
        }

        public void Clear()
        {
            if (_list != null)
                _list.Clear();

        }

        public int Size()
        {
            return _list.Count;
        }
        
        public void Add(Product p,int qty)
        {
            if (!this.Contains(p))
                _list.Add(new Order(p,qty));
            
            
        }


        public void Add(Product p)
        {
            this.Add(p, 1);
        }


        public int Increment(String id,int qty)
        {

            bool found = false;
            foreach (Order o in _list)
                if (o.Product.id.Equals(id)) {
                    o.Quantity += qty;
                    _total += qty;
                    _price += o.Product.prezzo * qty;
                    found = true;
                    return o.Quantity;
                }

            if (!found)
                throw new OrderNotFoundException("Order Not Found!!");

            return 0; //DEAD CODE

        }

        public int Increment(string id)
        {
            return this.Increment(id, 1);
        }
        
        public void Remove(Order o, int qty)
        {
            if (this.Contains(o.Product))
                _list.Remove(o);
            else throw new OrderNotFoundException("Order Not Found Exception");
        
        }

        public void Remove(Order o)
        {
            this.Remove(o, 1);
        }

        public int Decrement(String id, int qty)
        {
            bool found = false;
            foreach (Order o in _list)
                if (o.Product.id.Equals(id)) {
                    o.Quantity -= qty;
                    if (o.Quantity < 0)
                        o.Quantity = 0;

                    _total -= qty;

                    if (_total < 0)
                        _total = 0;

                    _price -= o.Product.prezzo * qty;
                    if (_price < 0)
                        _price = 0;

                    found = true;
                    return o.Quantity;
                }

            if(!found)
             throw new OrderNotFoundException("Order Not Found!!");

            return 0; // DEAD CODE
        
        }

        public int Decrement(String id)
        {
           return this.Decrement(id, 1);
        }

    }




    class Order
    {
        private Product _product;
        private int _qty;


        public Order(Product p,int qty)
        {
            _product = p;
            _qty = qty;
        }

        public Order(Product p):this(p,1)
        {
            
        }

        public Product Product { get { return _product; } set { if (value != _product) _product = value; } }
        public int Quantity
        {
            get { return _qty; }
            set { if (_qty != value) _qty = value; }
        }
    }


    class OrderNotFoundException : Exception
    {

        public OrderNotFoundException() { }
        public OrderNotFoundException(string message) : base(message) { }


    }


}
