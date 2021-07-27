

using System.Collections;
using UnityEngine;

namespace HumanBuilders {
  public class FlingFlowerDirectedLaunch : PlayerState {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return "fling_flower_directed_launch"; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The GUI guide for this fling flower.
    /// </summary>
    private IFlingFlowerGuide guide; 


    public override void OnUpdate() {
      if (player.PressedJump()) {
        if (player.CarriedItem == null) {
          ChangeToState<DoubleJumpStart>();
        } else {
          ChangeToState<CarryJumpStart>();
        }
      }
    }


    public override void OnFixedUpdate() {
      float a = powersSettings.FlingFlowerGravitation;
      physics.Velocity = Vector3.zero;
      physics.Position = physics.Position*a + guide.CurrentFlower.transform.position*(1-a);
    }

    public void OnDirectedLaunchFinished() {
      if (!exited) {
        new UnityTask(WaitToLaunch(guide.CurrentFlower.DelayBeforeLaunch));
        exited = true;
      }
    }

    public IEnumerator WaitToLaunch(float seconds) {
      yield return new WaitForSeconds(seconds);

      guide.CurrentFlower.Fling(player);
      ChangeToState<FlingFlowerDirectedProjectile>();
    }
    
    public override void OnStateEnter() {
      guide = player.FlingFlowerGuide;
      guide.CurrentFlower.PickDirection(player);
      
      if (player.CarriedItem != null) {
        player.CarriedItem.Hide();
      }
    }

    public override void OnStateExit() {
      if (player.CarriedItem != null) {
        player.CarriedItem.Show();
      }
    }

  }
}