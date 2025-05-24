namespace TheAvairy
{
    internal class Gorilla : Animal
    {
        public override AnimalType AnimalType { get; } = AnimalType.Gorilla;
        protected override float MaxAge { get; } = 50.00f;
        protected override bool IsLittle { get; } = false;
        protected override bool IsPossibleToHoldItems { get; } = true;

        public Gorilla(string Name, int MonthOfBirth, int WeekOfBirth, int CurrentAge, AvairyManager manager) :
            base(Name, MonthOfBirth, WeekOfBirth, manager)
        {

        }
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

            RandomAnimalState.TookNut,
            RandomAnimalState.TookNut,
            RandomAnimalState.TookNut,
        };
        protected override RandomAnimalAction[] AllPossibleRandomAnimalActions { get; set; } =
        {
            RandomAnimalAction.None,
            RandomAnimalAction.None,
            RandomAnimalAction.None,

            RandomAnimalAction.PetSquirell,
            RandomAnimalAction.PetSquirell,

            RandomAnimalAction.PetHedgehog,

            RandomAnimalAction.PetCat,

            RandomAnimalAction.PlayWithCongener,

            RandomAnimalAction.PlayWithAnotherAnimal
        };
    }
}
