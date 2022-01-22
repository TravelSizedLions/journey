using UnityEngine;

namespace HumanBuilders {
  public class PlayerCompanion : MonoBehaviour {

    [Range(0, 1)]
    [Tooltip("How quickly should the companion float back to the player's position.")]
    public float rubberBandStrength = 0.05f;

    [Tooltip("The minimum distance the companion should be from the player.")]
    public float minDistance = 1f;
    private Transform playerTransform;

    private void Start() {
      playerTransform = GameManager.Player.transform;
    }

    // Update is called once per frame
    private void FixedUpdate() {
      transform.position = Rubberband();
    }

    private Vector3 GetPosition() {
      return Rubberband();
    }

    private Vector3 GetTargetPosition() {
      Vector3 diff = transform.position - playerTransform.position;
      Vector3 directionToCompanion = diff.normalized;

      Vector3 rawPosition;
      if (diff.magnitude < minDistance) {
        // Don't push the companion away if the player walks into them.
        rawPosition = transform.position;
      } else {
        rawPosition = playerTransform.position + minDistance*directionToCompanion;
      }

      return new Vector3(rawPosition.x, playerTransform.position.y, playerTransform.position.z);
    }

    private Vector3 Rubberband() {
      Vector3 diff = GetTargetPosition() - transform.position;
      return transform.position + diff*rubberBandStrength;
    }
  }
}