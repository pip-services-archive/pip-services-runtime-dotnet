using PipServices.Runtime.Errors;

namespace PipServices.Runtime.Base
{
    /// <summary>
    ///     Component state manager
    /// </summary>
    public class ComponentStateManager
    {
        private readonly IComponent _component;

        public ComponentStateManager(IComponent component)
        {
            _component = component;
            CurrentState = State.Initial;
        }

        /// <summary>
        ///     Gets the current state of the component.
        /// </summary>
        public State CurrentState { get; private set; }

        /// <summary>
        ///     Checks if specified state matches to the current one.
        ///     It throws an exception if states don't match.
        /// </summary>
        /// <param name="state">The expected state.</param>
        protected void CheckCurrentState(State state)
        {
            if (CurrentState != state)
                throw new StateError(_component, "InvalidState", "Component is in wrong state")
                    .WithDetails(CurrentState, state);
        }

        /// <summary>
        ///     Checks if transition to the specified state is allowed from the current one.
        ///     It throws an exception when transition is not allowed.
        /// </summary>
        /// <param name="newState">the new state to make transition</param>
        public void CheckNewStateAllowed(State newState)
        {
            if (newState == State.Configured && CurrentState != State.Initial)
                throw new StateError(_component, "InvalidState", "Component cannot be configured")
                    .WithDetails(CurrentState, State.Configured);

            if (newState == State.Linked && CurrentState != State.Configured)
                throw new StateError(_component, "InvalidState", "Component cannot be linked")
                    .WithDetails(CurrentState, State.Linked);

            if (newState == State.Opening && CurrentState != State.Linked && CurrentState != State.Closed)
                throw new StateError(_component, "InvalidState", "Component cannot be opened")
                    .WithDetails(CurrentState, State.Opening);

            if (newState == State.Opened && CurrentState != State.Opening && CurrentState != State.Linked && CurrentState != State.Closed)
                throw new StateError(_component, "InvalidState", "Component cannot be opened")
                    .WithDetails(CurrentState, State.Opened);

            if (newState == State.Closed && CurrentState != State.Opened)
                throw new StateError(_component, "InvalidState", "Component cannot be closed")
                    .WithDetails(CurrentState, State.Closed);
        }

        /// <summary>
        ///     Transition to the specified state is allowed from the current one.
        ///     It throws an exception when transition is not allowed.
        /// </summary>
        /// <param name="newState">the new state to make transition</param>
        public void ChangeState(State newState)
        {
            CheckNewStateAllowed(newState);
            CurrentState = newState;
        }
    }
}