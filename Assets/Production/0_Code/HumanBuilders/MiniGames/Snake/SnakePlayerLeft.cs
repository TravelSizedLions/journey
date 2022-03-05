namespace HumanBuilders {
  public class SnakePlayerLeft : SnakeState {
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
    private string param = "left";

    public override void OnUpdate() {
      if (player.PressedUp()) {
        ChangeToState<SnakePlayerUp>();
      } else if (player.PressedDown()) {
        ChangeToState<SnakePlayerDown>();
      }
    }

    public override void OnFixedUpdate() {
      physics.Vx = -settings.Speed;
      physics.Vy = 0;
    }
  }
}