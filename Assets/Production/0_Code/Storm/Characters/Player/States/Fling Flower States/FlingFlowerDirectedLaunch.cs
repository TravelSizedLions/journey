
using UnityEngine;

namespace Storm.Characters.Player {
  public class FlingFlowerDirectedLaunch : PlayerState {

    private IFlingFlowerGuide guide; 

    private void Awake() {
      AnimParam = "fling_flower_directed_launch";
    }


    public override void OnFixedUpdate() {
      float a = powersSettings.FlingFlowerGravitation;
      physics.Velocity = Vector3.zero;
      physics.Position = physics.Position*a + guide.CurrentFlower.transform.position*(1-a);
    }

    public void OnDirectedLaunchFinished() {
      if (!exited) {
        ChangeToState<FlingFlowerDirectedProjectile>();
      }
    }

    public override void OnStateEnter() {
      guide = player.FlingFlowerGuide;
    }

  }
}