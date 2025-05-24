using System.Text;

namespace TheAvairy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Симуляція Вольєру";

            AvairyManager avairyManager = new AvairyManager();
            avairyManager.StartSimulation();

            Console.WriteLine(" Натисніть будь-яку кнопку для виходу");
            Console.ReadLine();
        }
    }
}
