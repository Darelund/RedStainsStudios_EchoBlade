
public class EnemyAlertedState : NonMonoState
{
    //public bool alarmRaised = false;
    public EnemyAlertedState(NonMonoBehaviourStateMachine nonMonoStateMachine) : base(nonMonoStateMachine)
    {
    }

    public override void EnterState()
    {
        AlarmVisuals();
        nonMonoStateMachine.SwitchState<EnemyInvestigateState>();
    }
    public override void ExitState()
    {

    }
    public override void UpdateState()
    {

    }
    public override void FixedUpdateState()
    {

    }
    private void AlarmVisuals()
    {
        //if (alarmRaised) return; //prevents multiple alarms being raised
        //alarmRaised = true;
    }
}
