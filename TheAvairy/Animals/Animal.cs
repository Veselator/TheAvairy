using System.CodeDom.Compiler;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace TheAvairy
{
    internal abstract class Animal
    {
        public string Name { get; private set; }
        public abstract AnimalType AnimalType { get; }
        public string AnimalTypeTranslated { get; private set; }
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
        protected abstract RandomAnimalAction[] AllPossibleRandomAnimalActions { get; set; }
        protected RandomAnimalState[] RandomAnimalStates { get; set; }
        protected RandomAnimalAction[] RandomAnimalActions { get; set; }
        protected float[] DieFactors;

        // Важливі змінні щодо поточного стану
        protected float happinessFactor;
        public float HappinessFactor
        {
            get => happinessFactor;
            set => happinessFactor = Math.Clamp(value, 0.0000f, 1.0000f);
        }

        public RandomAnimalState CurrentState { get; set; }
        public bool IsDead { get; private set; } = false;
        public bool IsEscaped { get; private set; } = false;
        public bool IsNutTaken { get; private set; } = false;

        protected static Random rnd = new Random();
        // Поля для розрахунку вирогідності смерті
        private static float LowerDeathFactor = 0.24f;
        private static float RandomFactor = 0.33f;
        private static float LogarithmMultiplier = 3.35f;

        // Константи, винесені в окремі поля
        public const float DeathEventDebuff = 0.42f;
        public const float EverydayHappinessIncome = 0.1f;
        private const int MonthsPerYear = 48;

        public Animal() : this("TestName", 7, 4)
        {
            
        }

        public Animal(string Name, int MonthOfBirth, int WeekOfBirth, AvairyManager manager = null)
        {
            this.Name = Name;
            this.MonthOfBirth = MonthOfBirth;
            this.WeekOfBirth = WeekOfBirth;
            this.manager = manager;
            AnimalTypeTranslated = TranslateDictionary.Translate(AnimalType.ToString().ToLower());

            HappinessFactor = 0.5f;

            GenerateDieFactors();
            GenerateRandomStates();
            GenerateRandomActions();
        }

        public Animal(string Name, int MonthOfBirth, int WeekOfBirth, int CurrentAge, AvairyManager manager) : 
            this(Name, MonthOfBirth, WeekOfBirth, manager)
        {
            this.CurrentAge = CurrentAge;
        }

        private void GenerateRandomActions() // Безпосередні дії тваринки
        {
            RandomAnimalActions = new RandomAnimalAction[(int)MaxAge * MonthsPerYear];
            RandomAnimalAction randomAсtion;
            for (int i = 0; i < RandomAnimalActions.Length; i++)
            {
                // Вибираємо випадкову дію із списку
                randomAсtion = AllPossibleRandomAnimalActions[rnd.Next(0, AllPossibleRandomAnimalActions.Length)];

                RandomAnimalActions[i] = randomAсtion;
            }
        }

        private void GenerateRandomStates() // Подія, із якою тваринка входить в тиждень
        {
            RandomAnimalStates = new RandomAnimalState[(int)MaxAge * MonthsPerYear + 1];
            bool LocalIsEscaped = false;
            RandomAnimalState randomEvent;
            for (int i = 0; i < RandomAnimalStates.Length; i++)
            {
                // Вибираємо випадкову подію із списку
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

                if (randomEvent == RandomAnimalState.Returned)
                {
                    RandomAnimalStates[i] = RandomAnimalState.None;
                    continue;
                }

                RandomAnimalStates[i] = randomEvent;
            }
        }

        private void GenerateDieFactors() // Вирогідність смерті на кожному році життя
        {
            DieFactors = new float[(int)MaxAge + 1];
            for (int i = 0; i < DieFactors.Length; i++)
            {
                DieFactors[i] = (float)Math.Clamp((Math.Log((float)(i / (MaxAge)) * LogarithmMultiplier)) - LowerDeathFactor + rnd.NextDouble() * RandomFactor, 0.00000f, 1.00000f);
            }

            DieFactors[(int)MaxAge] = 1;
        }

        private void IsDied()
        {
            if (DieFactors[CurrentAge] - rnd.NextDouble() >= 0.0001f || CurrentAge == MaxAge)
            {
                IsDead = true;
                manager.SomebodyDied?.Invoke();
                NoteManager.AddNote($" Всі звіри сьогодні сумують - {AnimalTypeTranslated} {Name} покинула нас.");
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
            CurrentState = RandomAnimalStates[manager.tick];
            if (CurrentState == RandomAnimalState.None) return;
            else if (CurrentState == RandomAnimalState.Hungry)
            {
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} голодає!");
                HappinessFactor -= 0.12f;
            }
            else if (CurrentState == RandomAnimalState.BadMood)
            {
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} в поганому настрої!");
                HappinessFactor -= 0.24f;
            }
            else if (CurrentState == RandomAnimalState.GoodMood)
            {
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} в гарному настрої!");
                HappinessFactor += 0.21f;
            }
            else if (CurrentState == RandomAnimalState.WellFed)
            {
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} дуже добре поїла!");
                HappinessFactor += 0.17f;
            }
            else if (CurrentState == RandomAnimalState.Escaped && !IsEscaped)
            {
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} втекла з вольєру! Надіємось, вона скоро повернеться");
                IsEscaped = true;
            }
            else if (CurrentState == RandomAnimalState.Returned && IsEscaped)
            {
                NoteManager.AddNote($" Ура! {AnimalTypeTranslated} {Name} повернулась до вольєру!");
                IsEscaped = false;
            }
            else if (CurrentState == RandomAnimalState.TookNut && IsPossibleToHoldItems && !IsNutTaken)
            {
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} підібрала горішок. Цікаво, що вона буде із ним робити?");
                IsNutTaken = true;
            }
        }

        public virtual void StateTick()
        {
            HappinessFactor += EverydayHappinessIncome;
            CheckAge();
            if (IsDead) return;
            CheckState();
        }

        public virtual void ActionTick() // Коригуємо бажання тваринки та дійсність
        {
            RandomAnimalAction currentAction = RandomAnimalActions[manager.tick];
            if (currentAction == RandomAnimalAction.None) return;
            else if (currentAction == RandomAnimalAction.PetSquirell && IsPossibleToHoldItems)
            {
                Squirell? SomeSquirell = (Squirell?)manager.LookForSpecifiedAnimalType(AnimalType.Squirell);
                if (SomeSquirell == null)
                {
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} сумна. Вона хотіла полгадити білку, але білки у вольєрі немає!");
                    HappinessFactor -= 0.15f;
                    return;
                }

                if (IsNutTaken)
                {
                    IsNutTaken = false;
                    HappinessFactor += 0.12f;
                    SomeSquirell.HappinessFactor += 0.12f;
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} весела! Вона дала білці горішок і та дозволила погладити її!");
                    return;
                }

                HappinessFactor -= 0.15f;
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} сумна - білка не дала себе погладити. Можливо, їй треба щось дати щоб вона дозволила себе погладити?");
            }
            else if (currentAction == RandomAnimalAction.PetHedgehog && IsPossibleToHoldItems)
            {
                Hedgehog? SomeHedgehog = (Hedgehog?)manager.LookForSpecifiedAnimalType(AnimalType.Hedgehog);
                if (SomeHedgehog == null)
                {
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} сумна. Вона хотіла полгадити їжака, але у вольєрі немає їжаків!");
                    HappinessFactor -= 0.19f;
                    return;
                }

                if (SomeHedgehog.HappinessFactor > 0.2f && SomeHedgehog.HappinessFactor < 0.5f)
                {
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} сумна. Вона хотіла полгадити їжака, але він не захотів цього через поганий настрій!");
                    HappinessFactor -= 0.18f;
                    return;
                }
                else if (SomeHedgehog.HappinessFactor < 0.2f)
                {
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} дуже сумна. Вона спробувала полгадити їжака, але він поколов її своїми колючками!");
                    HappinessFactor -= 0.23f;
                    return;
                }

                HappinessFactor += 0.18f;
                SomeHedgehog.HappinessFactor += 0.1f;
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} погладила їжака, який навіть поколов її свої колючки!");
            }
            else if (currentAction == RandomAnimalAction.PetCat && IsPossibleToHoldItems)
            {
                Cat? SomeCat = (Cat?)manager.LookForSpecifiedAnimalType(AnimalType.Cat);
                if (SomeCat == null)
                {
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} сумна. Вона хотіла полгадити кота, але у вольєрі немає котів!");
                    HappinessFactor -= 0.11f;
                    return;
                }

                HappinessFactor += 0.12f;
                SomeCat.HappinessFactor += 0.12f;
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} весела - вона погладила кошеня!");
            }
            else if (currentAction == RandomAnimalAction.PlayWithCongener)
            {
                Animal? Congener = manager.LookForSpecifiedAnimalType(AnimalType, UID);
                if (Congener == null)
                {
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} сумна. Вона хотіла пограти із родичем, але таких у вольєрі немає!");
                    HappinessFactor -= 0.12f;
                    return;
                }

                HappinessFactor += 0.12f;
                Congener.HappinessFactor += 0.12f;
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} весела - вона пограла із родичем!");
            }
            else if (currentAction == RandomAnimalAction.PlayWithAnotherAnimal)
            {
                Animal? AnotherAnimal = manager.LookForAnybody(UID);
                if (AnotherAnimal == null)
                {
                    NoteManager.AddNote($" {AnimalTypeTranslated} {Name} сумна. Вона хотіла пограти із кимось, але у вольєрі тільки вона!");
                    HappinessFactor -= 0.1f;
                    return;
                }

                HappinessFactor += 0.1f;
                AnotherAnimal.HappinessFactor += 0.1f;
                NoteManager.AddNote($" {AnimalTypeTranslated} {Name} весела - вона пограла із {AnotherAnimal.AnimalTypeTranslated} {AnotherAnimal.Name}!");
            }
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
        private string RandomEvents2String()
        {
            StringBuilder outputString = new StringBuilder();
            for (int i = 0; i < RandomAnimalStates.Length; i++)
            {
                outputString.AppendLine($"{i}) {RandomAnimalStates[i]}");
            }
            return outputString.ToString();
        }

        public override string ToString()
        {
            return $"{AnimalTypeTranslated} ім'я {Name}, вік {CurrentAge}. dieFactors: \n{DieFactors2String()}, events: \n{RandomEvents2String()}";
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
