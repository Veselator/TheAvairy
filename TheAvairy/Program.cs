namespace TheAvairy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AvairyManager avairyManager = new AvairyManager();
            avairyManager.StartSimulation();

            Console.ReadLine();
        }
    }
}
