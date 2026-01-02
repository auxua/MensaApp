using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MensaPortable
{

    public class DishTag
    {
        public string Name { get; set; }
        //public Color DisplayColor => GetColorFor(Name);

        //private Color GetColorFor(string tagName)
        //{
        //    switch (tagName.ToLower())
        //    {
        //        case "vegan":
        //            return Colors.Green;
        //        case "veggie":
        //            return Colors.Orange;
        //        case "schwein":
        //            return Color.Red;
        //        case "rind":
        //            return Color.Brown;
        //        case "geflügel":
        //            return Color.Yellow;
        //        case "fisch":
        //            return Color.Blue;
        //        default:
        //            return Color.Gray;
        //    }
        //}

    }
    public partial class Dish
    {
        public string Name { get; set; }
        //public double Price { get; set; }
        public string Price { get; set; }
        public string Kind { get; set; }
        public DateTime Date { get; set; }
        public string Mensa { get; set; }

        public string Special { get; set; }

        public string Popup { get; set; }

        public List<DishTag> Tags { get; set; } = new List<DishTag>();

        public Nutrition NutritionInfo { get; set; }

        public bool IsSideDish => (Price == null);

        public string Icon
        {
            get
            {
                if (this.Name == "Info") return DishIcons.TextBoxMore;
                else if (this.IsSideDish) return DishIcons.LocationAdd;
                else return DishIcons.Food;
            }
        }
            

        public Dish(string name, string kind, DateTime date, string mensa, string price = null, string NutritionString = null)
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

        //public bool isSideDish()
        //{
        //    return (Price == null);
        //}


        /// <summary>
        /// Removes the HTML-Tags inside the text
        /// </summary>
        private string RemoveTags(string input)
        {

            while (input.Contains("<sup>"))
            {
                int start = input.IndexOf("<sup>");
                int end = input.IndexOf("</sup>");
                input = input.Remove(start, (end - start) + 6);
            }

            input = input.Replace("<span class=\"or\">", "");
            input = input.Replace("<span class=\"seperator\"></span>", " | ");
            input = input.Replace("<span class=\"seperator\">", " ");
            input = input.Replace("</span>", " ");
            input = input.Replace("  ", " ");
            input = input.Replace("<em>", "");
            input = input.Replace("</em>", "");
            input = input.Replace("<br>", "\n");
            input = input.Replace("<strong>", "");
            input = input.Replace("</strong>", "");
            input = input.Replace("<span class=\"menue-nutr\">", "");
            input = input.Replace("<br/>", "");
            input = input.Replace("<br />", "");
            input = input.Replace("<div class=\"nutr-info\">", "");
            input = input.Replace("<div>", "");
            input = input.Trim();
            if (input.StartsWith("+"))
                input = input.Substring(1);
            return input;
        }


        public void ExtractTags(string html)
        {
            List<string> tags = new List<string>();
            if (html.Contains(@"src=""resources/images/inhalt/Schwein.png""")) tags.Add("Schwein");
            if (html.Contains(@"src=""resources/images/inhalt/vegan.png""")) tags.Add("Vegan");
            if (html.Contains(@"src=""resources/images/inhalt/OLV.png""")) tags.Add("Veggie");
            if (html.Contains(@"src=""resources/images/inhalt/Geflügel.png""")) tags.Add("Geflügel");
            if (html.Contains(@"src=""resources/images/inhalt/Fisch.png""")) tags.Add("Fisch");
            if (html.Contains(@"src=""resources/images/inhalt/Rind.png""")) tags.Add("Rind");
            this.Tags.AddRange(tags.ConvertAll(t => new DishTag() { Name = t }));
        }

    }
}
