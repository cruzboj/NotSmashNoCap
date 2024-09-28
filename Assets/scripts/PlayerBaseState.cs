using UnityEngine; // Ensure this is included
public abstract class PlayerBaseState
{
    private bool _isRootState = false;
    private PlayerStateMachine _ctx;
    private PlayerStateFactory _factory;
    private PlayerBaseState _currentSubState;
    private PlayerBaseState _currentSuperState;
    protected bool IsRootState { set { _isRootState = value; } }
    protected PlayerStateMachine Ctx { get { return _ctx; } }
    protected PlayerStateFactory Factory { get { return _factory; } }

    protected float fixedtime { get; set; }
    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
        fixedtime = 0;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitalizeSubState();

    // Update current state and any sub-state
    public void UpdateStates()
    {
        fixedtime += Time.deltaTime;
        UpdateState();
        _currentSubState?.UpdateStates(); // Recursively update sub-states
    }

    // Switch to a new state
    protected void SwitchState(PlayerBaseState newState)
    {
        // Exit the current state
        ExitState();

        // Enter the new state
        newState.EnterState();

        // If this is the root state, switch the current state of the context
        if (_isRootState)
        {
            _ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            // If not the root state, set the new state as the sub-state of the current super state
            _currentSuperState.SetSubState(newState);
        }
    }

    // Set the current state's super state
    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    // Set the current state's sub-state without recursive loop
    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this); // Set this state as the super state of the new sub-state
    }
}
