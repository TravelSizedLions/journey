using UnityEngine;

namespace Storm.Components {
  public interface IPhysics {
    float Vx { get; set; }

    float Vy { get; set; }

    Vector2 Velocity { get; set; }

    float Px { get; set; }

    float Py { get; set; }

    float Pz { get; set; }

    Vector3 Position { get; set; }

    void Disable();

    void Enable();

    void ClearParent();

    void SetParent(Transform parent);

    void AddChild(Transform child);

    void ClearChildren();

    void ResetPosition();

    void ResetLocalPosition();

    void Inject(Rigidbody2D rigidbody);
  }

  [RequireComponent(typeof(Rigidbody2D))]
  public class PhysicsComponent : MonoBehaviour, IPhysics {

    private new Rigidbody2D rigidbody;

    public void Awake() {
      this.rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Inject(Rigidbody2D rigidbody) {
      this.rigidbody = rigidbody;
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

    public void Disable() {
      rigidbody.simulated = false;
    }

    public void Enable() {
      rigidbody.simulated = true;
    }


    public void ClearParent() {
      transform.parent = null;
    }

    public void SetParent(Transform parent) {
      transform.parent = parent;
    }

    public void AddChild(Transform child) {
      child.parent = transform;
    }

    public void ClearChildren() {
      transform.DetachChildren();
    }

    public void ResetPosition() {
      transform.position = Vector3.zero;
    }

    public void ResetLocalPosition() {
      transform.localPosition = Vector3.zero;
    }
  }
}