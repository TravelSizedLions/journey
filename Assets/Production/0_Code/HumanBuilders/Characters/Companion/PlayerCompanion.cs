using UnityEngine;

namespace HumanBuilders {
  public class PlayerCompanion : MonoBehaviour {

    public Animator Animator { 
      get { 
        if (animator == null) {
          animator = GetComponentInChildren<Animator>();
        }

        return animator; 
      } 
    }

    [Range(0, 1)]
    [Tooltip("How quickly should the companion float back to the player's position.")]
    public float rubberBandStrength = 0.05f;

    [Tooltip("The minimum distance the companion should be from the player.")]
    public float minDistance = 1f;
    private Transform playerTransform;
    private Transform interestPoint;
    private Animator animator;

    private void Start() {
      playerTransform = GameManager.Player.transform;
      SetInterestPoint(playerTransform);
    }

    // Update is called once per frame
    private void FixedUpdate() {
      transform.position = Rubberband();
    }

    private Vector3 GetPosition() {
      return Rubberband();
    }

    private Vector3 GetTargetPosition() {
      Vector3 rawPosition = (interestPoint == playerTransform) ? HandlePlayerTargetPosition() : interestPoint.position;
      return new Vector3(rawPosition.x, interestPoint.position.y, interestPoint.position.z);
    }

    private Vector3 HandlePlayerTargetPosition() {
      Vector3 diff = transform.position - interestPoint.position;
      Vector3 directionToCompanion = diff.normalized;

      if (diff.magnitude < minDistance) {
        // Don't push the companion away if the player walks into them.
        return transform.position;
      } else {
        return interestPoint.position + minDistance*directionToCompanion;
      }
    }

    private Vector3 Rubberband() {
      Vector3 diff = GetTargetPosition() - transform.position;
      return transform.position + diff*rubberBandStrength;
    }

    public void SetInterestPoint(Transform target) {
      interestPoint = target;
    }

    public void ClearInterestPoint() {
      interestPoint = playerTransform;
    }
  }
}