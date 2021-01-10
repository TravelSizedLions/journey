using UnityEngine;

namespace HumanBuilders {
  public interface IPhysics {
    float GravityScale { get; set; }

    float Vx { get; set; }

    float Vy { get; set; }

    Vector2 Velocity { get; set; }

    float Px { get; set; }

    float Py { get; set; }

    float Pz { get; set; }

    Vector3 Position { get; set; }

    void Disable();

    void Enable();

    void Freeze(bool xPosition, bool yPosition, bool zRotation);

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

    private Rigidbody2D rb;

    public void Awake() {
      this.rb = GetComponent<Rigidbody2D>();
    }

    public void Inject(Rigidbody2D rigidbody) {
      this.rb = rigidbody;
    }

    public float GravityScale {
      get { return rb.gravityScale; }
      set { rb.gravityScale = value; }
    }

    public float Vx {
      get { return rb.velocity.x; }
      set { rb.velocity = new Vector2(value, rb.velocity.y); }
    }

    public float Vy {
      get { return rb.velocity.y; }
      set { rb.velocity = new Vector2(rb.velocity.x, value); }
    }
    public Vector2 Velocity {
      get { return rb.velocity; }
      set { rb.velocity = value; }
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
      rb.simulated = false;
    }

    public void Enable() {
      rb.simulated = true;
    }

    public void Freeze(bool xPosition, bool yPosition, bool zRotation = true) {
      rb.constraints = RigidbodyConstraints2D.None;
      if (xPosition) {
        rb.constraints |= RigidbodyConstraints2D.FreezePositionX;
      }

      if (yPosition) {
        rb.constraints |= RigidbodyConstraints2D.FreezePositionY;
      }

      if (zRotation) {
        rb.constraints |= RigidbodyConstraints2D.FreezeRotation;
      }

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