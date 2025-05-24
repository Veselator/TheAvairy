using System.Diagnostics.Metrics;

namespace TheAvairy
{
    internal class AvairyManager
    {
        private Animal[] animals = new Animal[64];
        private int count = 0;

        public int tick { get; private set; } = 0; // 1 тік = 1 тиждень

        public int CurrentWeek => tick % 4;
        public int CurrentMonth => (tick / 4) % 12;
        public int CurrentYear => tick / 48;
        public delegate void SomebodyDiedHandler();
        public SomebodyDiedHandler SomebodyDied;

        private bool IsAnybodyLeft = true;

        public AvairyManager() 
        {
            AddAnimals( 
                new Gorilla("Дора", 6, 3, 24, this),
                new Squirell("Ніка", 4, 1, 6, this),
                new Hedgehog("Сонік", 10, 2, 3, this),
                new Cat("Василій", 5, 1, 8, this)
            );
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
                SomebodyDied += () => newAnimals[i].HappinessFactor -= Animal.DeathEventDebuff;
                animals[count].UID = count;
                count++;
            }
        }

        private void GlobalEvents()
        {
            // Якісь глобальні фактори, наприклад погода
        }

        private void AnimalsStates()
        {
            // Те, із чим звірята входять в день
            for (int i = 0; i < count; i++)
            {
                if (!animals[i].IsDead) animals[i].StateTick();
            }
        }

        private void AnimalActions()
        {
            // Що вирішили зробити звірята
            for (int i = 0; i < count; i++)
            {
                if (!animals[i].IsDead && !animals[i].IsEscaped) animals[i].ActionTick();
            }
        }

        private void SystemTick()
        {
            GlobalEvents();
            AnimalsStates();

            AnimalActions();
            tick++;
        }

        public Animal? LookForSpecifiedAnimalType(AnimalType specifiedType)
        {
            for (int i = 0; i < count; i++)
            {
                if (!animals[i].IsEscaped && !animals[i].IsDead && animals[i].AnimalType == specifiedType) return animals[i];
            }

            return null;
        }

        public Animal? LookForSpecifiedAnimalType(AnimalType specifiedType, int IgnoreUID)
        {
            for (int i = 0; i < count; i++)
            {
                if (!animals[i].IsEscaped && !animals[i].IsDead && animals[i].AnimalType == specifiedType && animals[i].UID != IgnoreUID) return animals[i];
            }

            return null;
        }

        public Animal? LookForAnybody(int IgnoreUID)
        {
            for (int i = 0; i < count; i++)
            {
                if (!animals[i].IsEscaped && !animals[i].IsDead && animals[i].UID != IgnoreUID) return animals[i];
            }

            return null;
        }

        public void StartSimulation(int NumberOfTicks = 49, int StartingYear = 2025)
        {
            int i;

            //for (i = 0; i < count; i++)
            //{
            //    NoteManager.AddNote(animals[i].ToString());
            //}

            for (i = 0; i < NumberOfTicks; i++)
            {
                NoteManager.AddNote($"\n ~~~~  Рік {StartingYear + CurrentYear}, місяць {CurrentMonth}, тиждень {CurrentWeek}  ~~~~ \n");
                SystemTick();
            }

            Console.WriteLine(NoteManager.AllNotes);
            NoteManager.SaveToFile();
        }
    }
}
