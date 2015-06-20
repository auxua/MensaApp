using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensa
{
    public class DataTypes
    {
        public class Dish
        {
            public string Name { get; set; }
            //public double Price { get; set; }
            public string Price { get; set; }
            public string Kind { get; set; }
            public DateTime Date { get; set; }
            public string Mensa { get; set; }

            public Dish(string name, string kind, DateTime date, string mensa)
            {
                Name = RemoveTags(name);
                Kind = kind;
                Date = date;
                Mensa = mensa;
            }

            public Dish(string name, string kind, DateTime date, string mensa, string price) : this(name,kind,date,mensa)
            {
                Price = price;
            }

            public bool isSideDish()
            {
                return (Price == null);
            }

            private string RemoveTags(string input)
            {
                while (input.Contains("<sup>"))
                {
                    int start = input.IndexOf("<sup>");
                    int end = input.IndexOf("</sup>");
                    input = input.Remove(start, (end - start)+6);
                }
                input = input.Replace("<span class=\"or\">", "");
                input = input.Replace("</span>", "");
                input = input.Replace("  ", " ");
                input = input.Trim();
                return input;
            }
        }        
    }
}
