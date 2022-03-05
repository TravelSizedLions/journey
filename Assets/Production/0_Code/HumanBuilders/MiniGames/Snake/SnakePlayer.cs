using UnityEngine;

namespace HumanBuilders {

  public class SnakePlayer : MonoBehaviour {

    public IInputComponent PlayerInput {get; set;}

    private FiniteStateMachine stateMachine;

    public SnakeMovement SnakeSettings {get; set;}

    public IPhysics Physics;

    public SnakePiece Tail;

    public void Extend() {
      Tail = Tail.Extend(SnakeSettings.TailDistance);
    }

    public void Awake() {
      SnakeSettings = GetComponent<SnakeMovement>();

      PlayerInput = new UnityInput();
      Physics = gameObject.AddComponent<PhysicsComponent>();

      stateMachine = GetComponent<FiniteStateMachine>();
      stateMachine.StartMachine(gameObject.AddComponent<SnakePlayerRight>());

      ScoreCard.Reset();
    }


    public bool PressedUp() => PlayerInput.GetVerticalInput() > 0;

    public bool PressedDown() => PlayerInput.GetVerticalInput() < 0;

    public bool PressedRight() => PlayerInput.GetHorizontalInput() > 0;

    public bool PressedLeft() => PlayerInput.GetHorizontalInput() < 0;
  }
}