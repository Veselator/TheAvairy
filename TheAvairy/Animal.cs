using System.Text;

namespace TheAvairy
{
    internal abstract class Animal
    {
        public string Name { get; private set; }
        public static AnimalType animalType { get; }

        public DateTime DateOfBirth { get; private set; }
        public int CurrentAge { get; private set; }
        protected const int MaxAge = 20;
        protected static float[] dieFactors;
        protected static Random rnd = new Random();

        protected float happinessFactor;
        public float HappinessFactor
        {
            get
            {
                return happinessFactor;
            }
            set
            {
                happinessFactor = Math.Clamp(value, 0.00000f, 1.00000f);
            }
        }

        protected const int maxPointsPerMove = 20;
        protected const bool isLittle = false;

        public Animal()
        {
            Name = "Some test name";
            DateOfBirth = DateTime.Now;

            GenerateDieFactors();
        }

        private void GenerateDieFactors() // Вирогідність смерті на кожному році
        {
            dieFactors = new float[MaxAge + 1];
            for (int i = 0; i < MaxAge + 1; i++)
            {
                dieFactors[i] = Math.Clamp(i / (MaxAge) - 0.24f + rnd.Next() * 0.05f, 0.00000f, 1.00000f);
            }
        }

        public abstract void Tick();

        private string DieFactors2String()
        {
            StringBuilder outputString = new StringBuilder();
            for(int i = 0; i < dieFactors.Length; i++)
            {
                outputString.AppendLine($"{i}) {dieFactors[i]}");
            }
            return outputString.ToString();
        }

        public override string ToString()
        {
            return $"Тваринка виду {animalType}, ім'я {Name}, вік {CurrentAge}. dieFactors: \n{DieFactors2String}";
        }
    }
}
