
namespace ExplosionGame
{
    public class TransitionsPresenterFactory
    {
        private readonly StatesPresenter _states;
        private readonly TransitionsConditions _conditions;

        public TransitionsPresenterFactory(StatesPresenter states, TransitionsConditions conditions)
        {
            _conditions = conditions;
            _states = states;
        }

        public TransitionsPresenter Create()
        {
            TransitionsPresenter transitions = new();

            transitions.AddTransition(new Transition(_states.Idle, _states.Run, _conditions.CheckIfRun));
            transitions.AddTransition(new Transition(_states.Idle, _states.Fall, _conditions.CheckIfFall));
            transitions.AddTransition(new Transition(_states.Idle, _states.Die, _conditions.CheckIfDroppedIntoWater));

            transitions.AddTransition(new Transition(_states.Run, _states.Idle, _conditions.CheckIfStaying));
            transitions.AddTransition(new Transition(_states.Run, _states.Fall, _conditions.CheckIfFall));
            transitions.AddTransition(new Transition(_states.Run, _states.Die, _conditions.CheckIfDroppedIntoWater));

            transitions.AddTransition(new Transition(_states.Fall, _states.Idle, _conditions.CheckIfGrounded));
            transitions.AddTransition(new Transition(_states.Fall, _states.Die, _conditions.CheckIfDroppedIntoWater));

            return transitions;
        }
    }
}