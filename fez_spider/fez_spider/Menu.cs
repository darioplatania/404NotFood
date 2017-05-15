using System;
using Microsoft.SPOT;

namespace fez_spider
{
    public class Menu
    {
        public MenuInfo[] menuItems { get; set; }

    }


    public class MenuInfo
    {
        public String id { get; set; }
        public String name { get; set; }
        public String[] ingredients { get; set; }
        public float price { get; set; }
    }

}
