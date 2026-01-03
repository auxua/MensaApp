using MensaPortable;
using Newtonsoft.Json;
using ObjectDumping;

namespace MensaConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            string text = File.ReadAllText(@"R:\Repos\MensaApp2\supplemental\Mensa Ahornstraße - Wochenplan - Studierendenwerk Aachen.html");

            MenuDB.Instance.ImportFromSource(text, "Ahornstraße");

            MenuDB.QueryBuilder query = new MenuDB.QueryBuilder().ByMensa("Ahornstraße");
            var results = query.ExecuteQuery();

            foreach (var dish in results)
            {
                Console.WriteLine(dish.Dump());
            }

            var json = JsonConvert.SerializeObject(results);
            var rresults = JsonConvert.DeserializeObject<List<Dish>>(json);

            Console.WriteLine("fin.");
            Console.ReadLine();

        }
    }

    public class MemoryStoreLoader : MenuDB.IStoreLoader
    {
        private List<Dish> dishes = new List<Dish>();

        public List<Dish> LoadDishes()
        {
            return dishes;
        }

        public bool StoreDishes(List<Dish> dishes)
        {
            this.dishes = dishes;
            return true;
        }
    }
}
