using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheAvairy
{
    internal class Squirell : Animal
    {
        public override AnimalType AnimalType { get; } = AnimalType.Squirell;
        protected override float MaxAge { get; } = 13.00f;
        protected override bool IsLittle { get; } = true;
        protected override bool IsPossibleToHoldItems { get; } = false;

        public Squirell(string Name, int MonthOfBirth, int WeekOfBirth, int CurrentAge, AvairyManager manager) :
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
            RandomAnimalState.Returned,
            RandomAnimalState.Returned,
        };

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
