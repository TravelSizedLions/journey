using System.Collections;
using System.Collections.Generic;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// When the player tosses an item in the middle of a jump or fall.
  /// </summary>
  public class MidAirThrowItem : HorizontalMotion {

    #region Unity API
    private void Awake() {
      AnimParam = "carry_jump_throw";
    }
    #endregion

    #region State API
    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {
      Facing facing = MoveHorizontally();
      player.SetFacing(facing);

      if (player.IsTouchingGround()) {
        if (player.TryingToMove()) {
          ChangeToState<Running>();
        } else {
          ChangeToState<Land>();
        }
      } else if (player.IsTouchingLeftWall() || player.IsTouchingRightWall()) {
        if (player.IsRising()) {
          ChangeToState<WallRun>();
        } else  {
          ChangeToState<WallSlide>();
        }
      }
    }

    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      if (player.IsTouchingGround()) {
        player.Physics.Vy = settings.SingleJumpForce;
      }

      if (player.HoldingAltAction()) {
        player.Drop(player.CarriedItem);
      } else {
        player.Throw(player.CarriedItem);
        // // This will throw the item up and towards the horizontal direction of
        // // the mouse. This is different from the main throw, which is always in
        // // the exact direction of the mouse.
        
        // Carriable item = player.CarriedItem;
        // item.OnThrow();
        // item.Physics.Velocity = player.Physics.Velocity;

        // if (player.HoldingUp()) {
        //   item.Physics.Vy = settings.VerticalThrowForce + player.Physics.Vy;
        // } else {
        //   Vector3 mouse = player.GetMouseWorldPosition();

        //   // Throw right if the mouse is to the right of the player. Else throw left.
        //   if (mouse.x > player.transform.position.x) {
        //     item.Physics.Vx += settings.ThrowForce.x;
        //   } else {
        //     item.Physics.Vx -= settings.ThrowForce.x;
        //   }
        //   item.Physics.Vy += settings.ThrowForce.y;
        //}
      }


    }

    /// <summary>
    /// Animation event hook.
    /// </summary>
    public void OnMidAirThrowItemFinished() {
      if (!exited) {
        if (player.IsRising()) {
          ChangeToState<SingleJumpRise>();
        } else {
          ChangeToState<SingleJumpFall>();
        }
      }
    }
    #endregion
  }
}