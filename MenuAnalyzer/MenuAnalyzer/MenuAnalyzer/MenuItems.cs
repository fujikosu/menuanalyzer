using System;
using System.Collections.Generic;
using System.Text;

namespace MenuAnalyzer
{
    public class Menu
    {
        public string name { get; set; }
        public string contentURL { get; set; }
    }

    public class MenuItems
    {
        public List<Menu> menus { get; set; }
    }
}
