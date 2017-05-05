using System;
using Microsoft.SPOT;

namespace fez_spider
{
    public class Menu
    {
        public MenuInfo[] menu { get; set; }

    }


    public class MenuInfo
    {
        public String id;
        public String name;
        public String[] ingredients;
        public float price;


    }

}
