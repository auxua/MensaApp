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

            public string Special { get; set; }
            
            public Dish(string name, string kind, DateTime date, string mensa)
            {
                if (name.Contains("<sup>vegan</sup>"))
                {
                    Special = "Vegan";
                    //name = name.Replace("<sup>vegan</sup>","");
                }
                Name = RemoveTags(name);
                Kind = kind;
                Date = date;
                Mensa = mensa;
                // Assign Special by searching for modifiers used by STW so far.
                if (Name.StartsWith("*"))
                {
                    int pos = Name.Substring(1).IndexOf("*") + 1; //Position of last "*"
                    if (String.IsNullOrEmpty(Special))
                        this.Special = Name.Substring(0, pos + 2); // +2 for last "*" and the whitespace
                    else
                        this.Special += " & " + Name.Substring(0, pos + 2); // +2 for last "*" and the whitespace
                    Name = Name.Replace(Name.Substring(0, pos + 2), "");
                }
            }

            public Dish(string name, string kind, DateTime date, string mensa, string price) : this(name,kind,date,mensa)
            {
                Price = price;
            }

            public Dish() { }

            public bool isSideDish()
            {
                return (Price == null);
            }


            /// <summary>
            /// Removes the HTML-Tags inside the text
            /// </summary>
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
