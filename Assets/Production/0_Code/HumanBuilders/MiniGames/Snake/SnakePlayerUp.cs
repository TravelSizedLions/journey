namespace HumanBuilders {
  public class SnakePlayerUp : SnakeState {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "up";
    public override void OnUpdate() {
      if (player.PressedLeft()) {
        ChangeToState<SnakePlayerLeft>();
      } else if (player.PressedRight()) {
        ChangeToState<SnakePlayerRight>();
      }
    }

    public override void OnFixedUpdate() {
      physics.Vx = 0;
      physics.Vy = settings.Speed;
    }
  }
}