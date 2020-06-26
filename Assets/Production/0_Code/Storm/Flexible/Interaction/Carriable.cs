using Storm.Characters.Player;
using Storm.Components;
using UnityEngine;


namespace Storm.Flexible.Interaction {
  /// <summary>
  /// An object that the player can pick up and carry around.
  /// </summary>
  public class Carriable : Interactible {

    #region Fields

    /// <summary>
    /// The name of the indicator that will display over the player's head when
    /// they can interact with this object.
    /// </summary>
    private const string QUESTION_MARK = "QuestionMark";

    /// <summary>
    /// Threshold for whether or not the player is immobile on the ground.
    /// </summary>
    private const float SITTING_THRESHOLD = 0.1f;

    /// <summary>
    /// Whether or not the object is considered organic.
    /// </summary>
    public bool IsOrganic;

    private bool releasedAction;

    private BoxCollider2D collisionBox;

    /// <summary>
    /// Physics information (position, velocity) for this object.
    /// </summary>
    public IPhysics Physics;

    /// <summary>
    /// The original scale of the carriable object (so it can be reset after a player state animation gets interrupted).
    /// </summary>
    private Vector3 originalScale;
    #endregion

    #region Interactible API
    
    public override void OnInteract() {
      OnPickup();
    }
    
    /// <summary>
    /// Whether or not the player should have an interaction indicator placed
    /// over their head.
    /// </summary>
    public override bool ShouldShowIndicator() {
      return (Mathf.Abs(Physics.Vy) < SITTING_THRESHOLD &&
              !player.IsCrawling() && 
              !player.IsDiving());
    }
    #endregion
      
    #region Unity API
    private void Awake() {
      PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
      if (player != null) {
        BoxCollider2D[] cols = GetComponents<BoxCollider2D>();
        collisionBox = cols[0];
        Physics2D.IgnoreCollision(collisionBox, player.GetComponent<BoxCollider2D>());
      }

      Physics = gameObject.AddComponent<PhysicsComponent>();
      originalScale = transform.localScale;
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Pick up this object.
    /// </summary>
    public void OnPickup() {
      interacting = true;

      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }

      player.CarriedItem = this;
      player.Physics.AddChild(transform);

      Physics.Disable();
      Physics.SetParent(player.transform.GetChild(0));
      Physics.ResetLocalPosition();
      
      collisionBox.enabled = false;
      releasedAction = !player.HoldingAction() || player.ReleasedAction();
    }
    
    /// <summary>
    /// Put down this object.
    /// </summary>
    public void OnPutDown() {
      interacting = false;
      
      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }

      player.CarriedItem = null;

      Physics.Enable();
      Physics.ClearParent();
      collisionBox.enabled = true;
      transform.localScale = originalScale;
    }

    /// <summary>
    /// Throw this object.
    /// </summary>
    public void OnThrow() {
      interacting = false;

      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }

      player.CarriedItem = null;

      Physics.Enable();
      Physics.ClearParent();
      collisionBox.enabled = true;
      transform.localScale = originalScale;
    }

    #endregion
  }
}
