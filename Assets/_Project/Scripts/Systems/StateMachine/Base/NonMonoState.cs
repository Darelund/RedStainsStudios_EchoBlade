//Normal StateMachine and State is used for scripts that use MonoBehaviour. While this is used for scripts that do not use MonoBehaviour
[System.Serializable]
public class NonMonoState
{
    public NonMonoBehaviourStateMachine nonMonoStateMachine;
    public NonMonoState(NonMonoBehaviourStateMachine nonMonoStateMachine)
    {
        this.nonMonoStateMachine = nonMonoStateMachine;
    }
    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
}
