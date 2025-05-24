using System.CodeDom.Compiler;
using System.Text;

namespace TheAvairy
{
    internal abstract class Animal
    {
        public string Name { get; private set; }
        public abstract AnimalType AnimalType { get; }
        protected abstract float MaxAge { get; }
        protected abstract bool IsLittle { get; }
        protected abstract bool IsPossibleToHoldItems { get; }

        private AvairyManager manager;
        public int UID { get; set; }
        public int MonthOfBirth { get; private set; }
        public int WeekOfBirth { get; private set; }
        public int CurrentAge { get; private set; }

        // Масиви, що реалізують послідовність визначених подій у житті тваринки,
        // що, фактично, є долею тваринки

        protected abstract RandomAnimalState[] AllPossibleRandomAnimalStates { get; set; }
        protected RandomAnimalState[] RandomAnimalStates { get; set; }
        protected float[] DieFactors;

        // Важливі змінні щодо поточного стану
        protected float happinessFactor;
        public float HappinessFactor
        {
            get => happinessFactor;
            set => happinessFactor = Math.Clamp(value, 0.00000f, 1.00000f);
        }
        public RandomAnimalState CurrentState { get; set; }
        public bool IsDead { get; private set; } = false;
        public bool IsEscaped { get; private set; } = false;
        public bool IsNutTaken { get; private set; } = false;

        protected static Random rnd = new Random();
        // Змінні для розрахунку вирогідності смерті
        private static float LowerDeathFactor = 0.24f;
        private static float RandomFactor = 0.33f;
        private static float LogarithmMultiplier = 3.35f;

        public Animal() : this("TestName", 7, 4)
        {
            
        }

        public Animal(string Name, int MonthOfBirth, int WeekOfBirth, AvairyManager manager = null)
        {
            this.Name = Name;
            this.MonthOfBirth = MonthOfBirth;
            this.WeekOfBirth = WeekOfBirth;
            this.manager = manager;

            HappinessFactor = 0.5f;

            GenerateDieFactors();
            GenerateRandomStates();
        }

        public Animal(string Name, int MonthOfBirth, int WeekOfBirth, int CurrentAge, AvairyManager manager) : 
            this(Name, MonthOfBirth, WeekOfBirth, manager)
        {
            this.CurrentAge = CurrentAge;
        }

        private void GenerateRandomStates()
        {
            RandomAnimalStates = new RandomAnimalState[(int)MaxAge + 1];
            bool LocalIsEscaped = false;
            RandomAnimalState randomEvent;
            for (int i = 0; i < MaxAge + 1; i++)
            {
                // Вибираємо випадкову подіє із списку
                randomEvent = AllPossibleRandomAnimalStates[rnd.Next(0, AllPossibleRandomAnimalStates.Length)];
                if (LocalIsEscaped)
                {
                    if (randomEvent != RandomAnimalState.Returned)
                    {
                        RandomAnimalStates[i] = RandomAnimalState.None;
                        continue;
                    }
                    RandomAnimalStates[i] = RandomAnimalState.Returned;
                    LocalIsEscaped = false;
                    continue;
                }
                if (randomEvent == RandomAnimalState.Escaped)
                {
                    RandomAnimalStates[i] = RandomAnimalState.Escaped;
                    LocalIsEscaped = true;
                    continue;
                }
                RandomAnimalStates[i] = randomEvent;
            }
        }

        private void GenerateDieFactors() // Вирогідність смерті на кожному році життя
        {
            DieFactors = new float[(int)MaxAge + 1];
            for (int i = 0; i < MaxAge + 1; i++)
            {
                DieFactors[i] = (float)Math.Clamp((Math.Log((float)(i / (MaxAge)) * LogarithmMultiplier)) - LowerDeathFactor + rnd.NextDouble() * RandomFactor, 0.00000f, 1.00000f);
            }

            DieFactors[(int)MaxAge] = 1;
        }

        private void IsDied()
        {
            if (DieFactors[CurrentAge] - rnd.NextDouble() >= 0.0001f || CurrentAge == MaxAge)
            {
                manager.EliminateAnimal(UID);
                IsDead = true;
            }
        }

        private void CheckAge()
        {
            if (manager == null) return;

            if(manager.CurrentWeek == WeekOfBirth && manager.CurrentMonth == MonthOfBirth)
            {
                // Happy birthday to you, {Name}!
                CurrentAge++;
                IsDied();
            }
        }

        private void CheckState()
        {
            CurrentState = RandomAnimalStates[CurrentAge];
            if (CurrentState == RandomAnimalState.Hungry)
            {
                NoteManager.AddNote($"{Name} голодає!");
                HappinessFactor -= 0.12f;
            }
            else if (CurrentState == RandomAnimalState.BadMood)
            {
                HappinessFactor -= 0.24f;
            }
            else if (CurrentState == RandomAnimalState.GoodMood)
            {
                HappinessFactor += 0.21f;
            }
            else if (CurrentState == RandomAnimalState.WellFed)
            {
                HappinessFactor += 0.17f;
            }
            else if (CurrentState == RandomAnimalState.Escaped)
            {
                IsEscaped = true;
            }
            else if (CurrentState == RandomAnimalState.Returned)
            {
                IsEscaped = false;
            }
            else if (CurrentState == RandomAnimalState.TookNut && IsPossibleToHoldItems)
            {
                IsNutTaken = true;
            }
        }

        public virtual void StateTick()
        {
            CheckAge();
            if (IsDead) return;
            CheckState();
        }

        public virtual void ActionTick()
        {

        }

        private string DieFactors2String()
        {
            StringBuilder outputString = new StringBuilder();
            for(int i = 0; i < DieFactors.Length; i++)
            {
                outputString.AppendLine($"{i}) {DieFactors[i]}");
            }
            return outputString.ToString();
        }

        public override string ToString()
        {
            return $"Тваринка виду {AnimalType}, ім'я {Name}, вік {CurrentAge}. dieFactors: \n{DieFactors2String()}";
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not Animal someAnimal) return false;
            return UID == someAnimal.UID;
        }

        public override int GetHashCode()
        {
            return UID.GetHashCode();
        }
    }
}
