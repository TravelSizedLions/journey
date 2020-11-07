
using UnityEngine;

namespace Storm.Characters.Player {
  public class FlingFlowerDirectedProjectile : PlayerState {

    private void Awake() {
      AnimParam = "fling_flower_directed_projectile";
    }
    
    public override void OnStateEnter() {
      physics.GravityScale = 0;
      physics.Velocity = new Vector2(powersSettings.FlingFlowerDirectedVelocity, 0);
    }

  }
}