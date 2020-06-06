using Storm.LevelMechanics.Platforms;
using UnityEngine;

namespace Storm.Characters.Player {


  public class CoyoteTimer : MonoBehaviour {


    private float timer;

    private float coyoteTime = 0;


    private void Start() {
      MovementSettings settings = GetComponent<MovementSettings>();
      coyoteTime = settings.CoyoteTime;
    }

    private void FixedUpdate() {
      Tick();
    }


    public void Tick() {
      timer += Time.fixedDeltaTime;
    }

    public void Reset() {
      timer = 0;
    }

    public bool InCoyoteTime() {
      return timer < coyoteTime;
    }

    public void UseCoyoteTime() {
      timer = coyoteTime;
    }

    public void SetCoyoteTime(float timer) {
      this.coyoteTime = timer;
    }
  }
}