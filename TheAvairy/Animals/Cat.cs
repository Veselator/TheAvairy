namespace TheAvairy
{
    internal class Cat : Animal
    {
        public override AnimalType AnimalType { get; } = AnimalType.Cat;
        protected override float MaxAge { get; } = 20.00f;
        protected override bool IsLittle { get; } = true;
        protected override bool IsPossibleToHoldItems { get; } = false;
        protected override RandomAnimalState[] AllPossibleRandomAnimalStates { get; set; } =
        {
            RandomAnimalState.None,
            RandomAnimalState.None,
            RandomAnimalState.None,
            RandomAnimalState.None,
            RandomAnimalState.None,
            RandomAnimalState.None,
            RandomAnimalState.None,
            RandomAnimalState.None,

            RandomAnimalState.BadMood,
            RandomAnimalState.BadMood,
            RandomAnimalState.GoodMood,
            RandomAnimalState.GoodMood,
            RandomAnimalState.GoodMood,

            RandomAnimalState.Hungry,
            RandomAnimalState.GotIll,
            RandomAnimalState.WellFed,
            RandomAnimalState.WellFed,
            RandomAnimalState.WellFed,

            RandomAnimalState.Escaped,
            RandomAnimalState.Escaped,
            RandomAnimalState.Returned,
            RandomAnimalState.Returned,

            RandomAnimalState.Shedding,
        };

        public Cat(string Name, int MonthOfBirth, int WeekOfBirth, int CurrentAge, AvairyManager manager) :
            base(Name, MonthOfBirth, WeekOfBirth, manager)
        {

        }

        protected override RandomAnimalAction[] AllPossibleRandomAnimalActions { get; set; } =
        {
            RandomAnimalAction.None,
            RandomAnimalAction.None,
            RandomAnimalAction.None,
            RandomAnimalAction.None,
            RandomAnimalAction.None,
            RandomAnimalAction.None,

            RandomAnimalAction.PlayWithCongener,
            RandomAnimalAction.PlayWithCongener,

            RandomAnimalAction.PlayWithAnotherAnimal
        };
    }
}
