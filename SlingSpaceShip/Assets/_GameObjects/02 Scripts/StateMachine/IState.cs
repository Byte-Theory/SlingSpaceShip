public interface IState<in TContext>
{
    public void Enter(TContext context);
    public void UpdateState(TContext context, float deltaTime);
    public void Exit(TContext context);
}
