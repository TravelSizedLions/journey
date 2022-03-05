using UnityEngine;

namespace HumanBuilders {
  public class SnakePiece : MonoBehaviour {
    private HingeJoint2D connection;

    public void Awake() {
      connection = GetComponent<HingeJoint2D>();
    }
    public SnakePiece Extend(float distance) {
      Vector3 cur = gameObject.transform.position;
      Vector3 pos = cur + GetDirection()*distance;

      GameObject nextPiece = Instantiate(gameObject, pos, Quaternion.identity);
      SnakePiece tail = nextPiece.GetComponent<SnakePiece>();
      tail.name = "snake_piece";
      tail.Connect(this, distance);

      return tail;
    }

    public void Connect(SnakePiece snakePiece, float distance) {
      connection.connectedBody = snakePiece.GetComponent<Rigidbody2D>();
      connection.connectedAnchor = new Vector2(-distance, 0);
    }

    public Vector3 GetDirection() {
      return (transform.position - connection.connectedBody.transform.position).normalized;
    }
  }
}