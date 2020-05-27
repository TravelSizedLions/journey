using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace Storm.Characters.Player {
  /// <summary>
  /// When the player prepares to do a single jump.
  /// </summary>
  public class Jump1Start : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "jump_1_start";
    } 
    #endregion


    #region Player State API
    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {
      if (Input.GetButtonDown("Jump") && player.CanJump()) {
        ChangeToState<Jump2Start>();
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
    /// Fires when the state exits, before the next state is entered into.
    /// </summary>
    public override void OnStateExit() {
      MovementSettings settings = GetComponent<MovementSettings>();

      Rigidbody2D rigidbody = player.GetComponent<Rigidbody2D>();
      rigidbody.velocity = (rigidbody.velocity * Vector2.right) + new Vector2(0, settings.SingleJumpForce);
    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnSingleJumpFinished() {
      ChangeToState<Jump1Rise>();
    }
    #endregion
  }

}