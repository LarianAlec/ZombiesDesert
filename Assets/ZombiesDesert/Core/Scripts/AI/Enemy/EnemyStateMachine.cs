public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }

    public void Initialize(EnemyState startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
