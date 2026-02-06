using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingState : State /*ISavable*/
{
    [SerializeField] private Movement Movement; //Replace with a PlayerController later
    [SerializeField] private AIManager AIManager;

    public override void EnterState()
    {

    }
    public override void UpdateState()
    {
        AIManager.UpdateManager();
        Movement.UpdateMovement();
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            GameManager.Instance.SwitchState<PauseState>();
        }
    }
    public override void FixedUpdateState()
    {

    }

    //public void Save(GameData gameData)
    //{
    //    throw new System.NotImplementedException();
    //}

    //public void Load(GameData gameData)
    //{
    //    throw new System.NotImplementedException();
    //}
}
