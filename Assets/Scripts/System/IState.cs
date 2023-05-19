//this belongs to state machine
public interface IState
{
    void Tick();
    void OnEnter();
    void OnExit();
}