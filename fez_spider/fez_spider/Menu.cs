using System;
using Microsoft.SPOT;

namespace fez_spider
{
    public class Menu
    {

        private MenuInfo[] _menuItems;
        public MenuInfo[] menuItems
        {

            get { return _menuItems; }
            set { if (value != null && value != _menuItems) _menuItems = value; }

        }


        public class MenuInfo
        {


            private String _id;
            private String _name;
            private String[] _ingredients;
            private float _price;

            public String id { get { return _id; } set { if (value != null && value != _id) _id = value; } }
            public String name { get { return _name; } set { if (value != null && value != _name) _name = value; } }
            public String[] ingredients { get { return _ingredients; } set { if (value != null && value != _ingredients) _ingredients = value; } }
            public float price { get { return _price; } set { if (value != _price) _price = value; } }
        }

    }


}
