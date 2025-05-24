using System.Diagnostics.Metrics;

namespace TheAvairy
{
    internal class AvairyManager
    {
        private Animal[] animals = new Animal[64];
        private int count = 0;

        private int tick = 0; // 1 тік = 1 тиждень

        public int CurrentWeek => tick % 4;
        public int CurrentMonth => (tick / 4) % 12;
        public int CurrentYear => tick / 48;

        public AvairyManager() 
        {
            AddAnimals( new Gorilla("Dora", 6, 3, 24, this) );
        }

        public AvairyManager(params Animal[] newAnimals)
        {
            AddAnimals(newAnimals);
        }

        private void AddAnimals(params Animal[] newAnimals)
        {
            for (int i = 0; i < newAnimals.Length; i++)
            {
                animals[count] = newAnimals[i];
                animals[count].UID = count;
                count++;
            }
        }

        public void EliminateAnimal(int AnimalUID)
        {

        }

        private void GlobalEvents()
        {
            // Якісь глобальні фактори, наприклад погода
        }

        private void AnimalsStates()
        {
            // Те, із чим звірята входять в день
            foreach (Animal animal in animals)
            {
                animal.StateTick();
            }
        }

        private void SystemTick()
        {
            GlobalEvents();
            AnimalsStates();
        }

        public void StartSimulation(int NumberOfTicks = 96, int StartingYear = 2025)
        {
            for (int i = 0; i < NumberOfTicks; i++)
            {
                NoteManager.AddNote($"Рік {StartingYear + CurrentYear}, місяць {CurrentMonth + 1}, тиждень {CurrentWeek}");
                SystemTick();
            }
            Console.WriteLine(NoteManager.AllNotes);
        }
    }
}
