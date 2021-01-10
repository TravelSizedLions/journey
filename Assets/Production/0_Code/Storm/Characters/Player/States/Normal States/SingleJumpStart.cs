using System.Collections;
using System.Collections.Generic;


using UnityEngine;




namespace HumanBuilders {
  /// <summary>
  /// When the player prepares to do a single jump.
  /// </summary>
  public class SingleJumpStart : HorizontalMotion {
    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "jump_1_start";
    #endregion


    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (player.PressedJump()) {
        ChangeToState<DoubleJumpStart>();
      } else if (player.PressedAction() || player.PressedAltAction()) { 
        player.Interact();
      }
    }

    public override void OnSignal(GameObject obj) {
      if (CanCarry(obj)) {
        Carriable item = obj.GetComponent<Carriable>();
        item.OnPickup();
        ChangeToState<CarryJumpRise>();
      } else if (IsAimableFlingFlower(obj)) {
        ChangeToState<FlingFlowerAim>();
      } else if (IsDirectionalFlingFlower(obj)) {
        ChangeToState<FlingFlowerDirectedLaunch>();
      }
    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      player.DisablePlatformMomentum();
    }

    /// <summary>
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public override void OnStateExit() {
      MovementSettings settings = GetComponent<MovementSettings>();
      physics.Vy = settings.SingleJumpForce;
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnSingleJumpFinished() {
      if (!exited) {
        ChangeToState<SingleJumpRise>();
      }
    }
    #endregion
  }

}