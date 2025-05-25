using System.Diagnostics.Metrics;
using System.Text;

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
        private int NumOfDeath = 0;

        public AvairyManager()
        {
            AddAnimals( 
                new Gorilla("Дора", 6, 3, 24, this),
                new Squirell("Ніка", 4, 1, 6, this),
                new Hedgehog("Сонік", 10, 2, 3, this),
                new Cat("Василій", 5, 1, 8, this)
            );
            InitDelegates();
        }

        public AvairyManager(params Animal[] newAnimals)
        {
            AddAnimals(newAnimals);
            InitDelegates();
        }

        private void InitDelegates()
        {
            SomebodyDied += () => NumOfDeath++;
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
            bool IsAnybodyTicked = false;
            // Те, із чим звірята входять в день
            for (int i = 0; i < count; i++)
            {
                if (!animals[i].IsDead)
                {
                    IsAnybodyTicked = true;
                    animals[i].StateTick();
                }
            }

            if (!IsAnybodyTicked)
            {
                NoteManager.AddNote($" Симуляція зупинена. Причина: смерть всіх тварин.");
                IsAnybodyLeft = false;
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
            //GlobalEvents();
            AnimalsStates();
            if (!IsAnybodyLeft) return;

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

        private string GenerateHapinnesReport()
        {
            StringBuilder outputString = new StringBuilder("\n Поточні показники щастя:\n");

            for (int i = 0; i < count; i++)
            {
                if (!animals[i].IsEscaped && !animals[i].IsDead)
                {
                    outputString.AppendLine($"    {animals[i].AnimalTypeTranslated} {animals[i].Name} - {animals[i].HappinessFactor,4:f}");
                }
            }

            return outputString.ToString();
        }

        private string GenerateEndStatistics()
        {
            StringBuilder outputString = new StringBuilder("\n Статистика:\n");

            outputString.AppendLine($"    Кількість смертей: {NumOfDeath}");

            return outputString.ToString();
        }

        public static string GenerateGraph(float[] inputArray, int step = 2)
        {
            if (inputArray == null || inputArray.Length == 0)
                return string.Empty;

            // Найдем максимальное значение для масштабирования
            float maxValue = inputArray.Max();
            if (maxValue == 0) maxValue = 1; // избегаем деления на ноль

            int graphHeight = 10; // высота графика в строках
            int graphWidth = inputArray.Length;

            var result = new List<string>();

            // Создаем график сверху вниз
            for (int row = graphHeight; row >= 1; row--)
            {
                string line = "";

                // Добавляем процентную метку
                if (row == graphHeight || row == graphHeight * 4 / 5 ||
                    row == graphHeight * 3 / 5 || row == graphHeight * 2 / 5 || row == 1)
                {
                    int percent = (int)((float)row / graphHeight * 100);
                    line += $"{percent,3}% |";
                }
                else
                {
                    line += "     |";
                }

                // Добавляем столбцы графика
                for (int col = 0; col < inputArray.Length; col++)
                {
                    float normalizedValue = inputArray[col] / maxValue;
                    int columnHeight = (int)(normalizedValue * graphHeight);

                    if (columnHeight >= row)
                    {
                        line += " ###";
                    }
                    else
                    {
                        line += "    ";
                    }
                }

                result.Add(line);
            }

            // Добавляем нижнюю границу
            string bottomLine = "     *";
            for (int i = 0; i < inputArray.Length; i++)
            {
                bottomLine += "----";
            }
            result.Add(bottomLine);

            // Добавляем числовые метки снизу
            string numbersLine = "      ";
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (i % step == 0)
                {
                    numbersLine += $"{i,3} ";
                }
                else
                {
                    numbersLine += "    ";
                }
            }
            result.Add(numbersLine);

            return string.Join("\n", result);
        }

        public void StartSimulation(int NumberOfTicks = 49, int StartingYear = 2025)
        {
            int i;

            for (i = 0; i < count; i++)
            {
                NoteManager.AddNote($"\n {animals[i].AnimalTypeTranslated} {animals[i].Name} - графік вирогідностей вмерти:\n" + GenerateGraph(animals[i].DieFactors));
            }

            for (i = 0; i < NumberOfTicks; i++)
            {
                NoteManager.AddNote($"\n ~~~~  Рік {StartingYear + CurrentYear}, місяць {CurrentMonth}, тиждень {CurrentWeek}  ~~~~ \n");
                SystemTick();
                if (!IsAnybodyLeft) break;
                NoteManager.AddNote(GenerateHapinnesReport());
            }
            NoteManager.AddNote(GenerateEndStatistics());

            Console.WriteLine(NoteManager.AllNotes);
            NoteManager.SaveToFile();
        }
    }
}
