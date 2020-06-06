using UnityEngine;

namespace Storm.Subsystems.FSM {
  public interface IStateMachine {
    void StartMachine(State startState);

    void OnStateChange(State oldState, State newState);

    void SetAnimParam(string name);

    State GetState();
  }


  public class FiniteStateMachine : MonoBehaviour, IStateMachine {

    private State state;

    private Animator animator;

    private void Start() {
      animator = GetComponent<Animator>();
    }

    private void Update() {
      state.OnUpdate();
    }

    private void FixedUpdate() {
      state.OnFixedUpdate();
    }

    public void StartMachine(State startState) {
      state = startState;
      state.HiddenOnStateAdded();
      state.EnterState();
      animator.ResetTrigger(state.GetAnimParam());
    }

    public void OnStateChange(State oldState, State newState) {
      state = newState;
      oldState.ExitState();
      newState.EnterState();
    }

    public void SetAnimParam(string name) {
      animator.SetTrigger(name);
    }

    public State GetState() {
      return state;
    }
  }
}