using UnityEngine;

namespace Storm.Services {
  public interface IPhysics {
    float Vx { get; set; }

    float Vy { get; set; }

    Vector2 Velocity { get; set; }

    float Px { get; set; }

    float Py { get; set; }

    float Pz { get; set; }

    Vector3 Position { get; set; }
  }

  [RequireComponent(typeof(Rigidbody2D))]
  public class UnityPhysics : MonoBehaviour, IPhysics {

    private new Rigidbody2D rigidbody;

    public void Awake() {
      this.rigidbody = GetComponent<Rigidbody2D>();
    }

    public float Vx {
      get { return rigidbody.velocity.x; }
      set { rigidbody.velocity = new Vector2(value, rigidbody.velocity.y); }
    }

    public float Vy {
      get { return rigidbody.velocity.y; }
      set { rigidbody.velocity = new Vector2(rigidbody.velocity.x, value); }
    }
    public Vector2 Velocity {
      get { return rigidbody.velocity; }
      set { rigidbody.velocity = value; }
    }

    public float Px {
      get { return transform.position.x; }
      set { transform.position = new Vector3(value, transform.position.y, transform.position.z); }
    }
    public float Py {
      get { return transform.position.y; }
      set { transform.position = new Vector3(transform.position.x, value, transform.position.z); }
    }

    public float Pz {
      get { return transform.position.z; }
      set { transform.position = new Vector3(transform.position.x, transform.position.y, value); }
    }

    public Vector3 Position {
      get { return transform.position; }
      set { transform.position = value; }
    }
  }
}