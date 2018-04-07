﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MensaPortable
{
    public class DataTypes
    {
        public class Nutrition
        {
            /// <summary>
            /// Caloric Value (kcal/kj)
            /// "Brennwert"
            /// </summary>
            public string Caloric { get; set; } = null;

            /// <summary>
            /// "Fette"
            /// </summary>
            public string Fat { get; set; } = null;

            /// <summary>
            /// "Kohlenhydrate"
            /// </summary>
            public string Carbohydrates { get; set; } = null;

            /// <summary>
            /// "Eiweiß"
            /// </summary>
            public string Proteins { get; set; } = null;
        }

        public class Dish
        {
            public string Name { get; set; }
            //public double Price { get; set; }
            public string Price { get; set; }
            public string Kind { get; set; }
            public DateTime Date { get; set; }
            public string Mensa { get; set; }

            public string Special { get; set; }

            public Nutrition NutritionInfo { get; set; }
            
            public Dish(string name, string kind, DateTime date, string mensa, string price=null, string NutritionString=null)
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
                Price = price;
                
            }

            /*public Dish(string name, string kind, DateTime date, string mensa, string price) : this(name,kind,date,mensa)
            {
                Price = price;
            }*/

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
                input = input.Replace("<span class=\"seperator\"></span>", " | ");
                input = input.Replace("<span class=\"seperator\">", " ");
                input = input.Replace("</span>", " ");
                input = input.Replace("  ", " ");
                input = input.Trim();
                return input;
            }
        }        
    }
}
