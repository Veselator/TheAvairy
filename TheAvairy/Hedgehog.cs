namespace TheAvairy
{
    internal class Hedgehog : Animal
    {
        public override AnimalType AnimalType { get; } = AnimalType.Hedgehog;
        protected override float MaxAge { get; } = 5.00f;
        protected override bool IsLittle { get; } = true;
        protected override bool IsPossibleToHoldItems { get; } = false;

        public Hedgehog(string Name, int MonthOfBirth, int WeekOfBirth, int CurrentAge, AvairyManager manager) :
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

            RandomAnimalState.BadMood,
            RandomAnimalState.GoodMood,
            RandomAnimalState.GoodMood,
            RandomAnimalState.GoodMood,

            RandomAnimalState.Hungry,
            RandomAnimalState.GotIll,
            RandomAnimalState.WellFed,
            RandomAnimalState.WellFed,
            RandomAnimalState.WellFed,
            RandomAnimalState.WellFed,
            RandomAnimalState.WellFed,

            RandomAnimalState.Escaped,
            RandomAnimalState.Escaped,
            RandomAnimalState.Escaped,
            RandomAnimalState.Escaped,
            RandomAnimalState.Returned,
        };

        public override void StateTick()
        {
            base.StateTick();
        }
    }
}