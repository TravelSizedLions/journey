using Storm.Attributes;
using Storm.Characters.Player;
using Storm.Components;
using Storm.Flexible.Interaction;
using Storm.LevelMechanics.Platforms;
using Storm.Math;
using Storm.UI;
using UnityEngine;


namespace Storm.Flexible {
  /// <summary>
  /// An object that the player can pick up and carry around.
  /// </summary>
  public class Carriable : PhysicalInteractible {

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
    /// Whether or not the object is considered organic. This determines whether or not the object can travel through live wires.
    /// </summary>
    [Tooltip("Whether or not the object is considered organic. This determines whether or not the object can travel through live wires.")]
    public bool IsOrganic;

    private bool thrown;

    /// <summary>
    /// Physics information (position, velocity) for this object.
    /// </summary>
    public IPhysics Physics;

    /// <summary>
    /// The original scale of the carriable object (so it can be reset after a player state animation gets interrupted).
    /// </summary>
    private Vector3 originalScale;

    private ICollision collisionSensor;

    /// <summary>
    /// The sprite for this carriable.
    /// </summary>
    public SpriteRenderer Sprite;

    private string spriteRenderLayer;

    private int spriteRenderOrder;


    [SerializeField]
    [ReadOnly]
    private bool freeze;


    #endregion
      
    #region Unity API
    protected new void Awake() {
      base.Awake();

      PlayerCharacter player = FindObjectOfType<PlayerCharacter>();

      Physics = gameObject.AddComponent<PhysicsComponent>();
      originalScale = transform.localScale;

      if (Sprite != null) {
        spriteRenderLayer = Sprite.sortingLayerName;
        spriteRenderOrder = Sprite.sortingOrder;
      }
    }



    private void Start() {
      collisionSensor = new CollisionComponent(col);

      // Allow carriable items to be thrown up through one-way platforms.
      OneWayPlatform.RegisterCollider(col);
    }

    private void FixedUpdate() {
      if (!freeze && 
          !collisionSensor.IsTouchingLeftWall(col.bounds.center, col.bounds.size) &&
          !collisionSensor.IsTouchingRightWall(col.bounds.center, col.bounds.size)) {
            
        if (Mathf.Abs(Physics.Vx) < 0.01f) {
          FreezePosition();
        }
      } else if (freeze && Mathf.Abs(Physics.Vx) > 0.01f) {
        UnfreezePosition();
      }
    }

    protected new void OnDestroy() {
      base.OnDestroy();
      OneWayPlatform.UnregisterCollider(col);
    }


    private void FreezePosition() {
      freeze = true;
      Physics.Freeze(true, false, true);
    }


    private void UnfreezePosition() {
      freeze = false;
      Physics.Freeze(false, false, true);
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Pick up this object.
    /// </summary>
    public void OnPickup() {
      GameManager.Mouse.Swap("aim");

      interacting = true;
      thrown = false;
      UnfreezePosition();

      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }

      // Put the carriable on the same layer as the player.
      if (Sprite != null) {
        Sprite.sortingLayerName = player.Sprite.sortingLayerName;
        Sprite.sortingOrder = player.Sprite.sortingOrder;
      }

      player.CarriedItem = this;
      player.Physics.AddChild(transform);

      Physics.Disable();
      Physics.SetParent(player.transform.GetChild(0));
      Physics.ResetLocalPosition();
      
      col.enabled = false;
    }
    
    /// <summary>
    /// Put down this object.
    /// </summary>
    public void OnPutDown() {
      GameManager.Mouse.Swap("default");
      
      thrown = false;
      interacting = false;
      UnfreezePosition();

      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }

      player.CarriedItem = null;

      // Restore the carriable sprite's original layer settings.
      if (Sprite != null) {
        Sprite.sortingLayerName = spriteRenderLayer;
        Sprite.sortingOrder = spriteRenderOrder;
      }

      Physics.Enable();
      Physics.ClearParent();
      col.enabled = true;
      transform.localScale = originalScale;
    }

    /// <summary>
    /// Throw this object.
    /// </summary>
    public void OnThrow() {
      GameManager.Mouse.Swap("default");
      
      thrown = true;
      interacting = false;
      UnfreezePosition();

      if (player == null) {
        player = FindObjectOfType<PlayerCharacter>();
      }

      player.CarriedItem = null;

      // Restore the carriable sprite's original layer settings.
      if (Sprite != null) {
        Sprite.sortingLayerName = spriteRenderLayer;
        Sprite.sortingOrder = spriteRenderOrder;
      }

      Physics.Enable();
      Physics.ClearParent();
      col.enabled = true;
      transform.localScale = originalScale;
    }


    /// <summary>
    /// Make the carriable invisible.
    /// </summary>
    public void Hide() {
      Sprite.enabled = false;
    }

    /// <summary>
    /// Make the carriable visible.
    /// </summary>
    public void Show() {
      Sprite.enabled = true;
    }

    public bool IsVisible() {
      return Sprite.enabled;
    }

    #endregion

    #region Interactible API
    
    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    public override void OnInteract() {
      if (!interacting) {
        OnPickup();
      } else {
        interacting = false;
      }
    }
    
    /// <summary>
    /// Whether or not the indicator for this interactible should be shown.
    /// </summary>
    /// <remarks>
    /// This is used when this particular interactive object is the closest to the player. If the indicator can be shown
    /// that usually means it can be interacted with.
    /// </remarks>
    public override bool ShouldShowIndicator() {
      return (!thrown &&
              !player.IsCrawling() && 
              !player.IsDiving() &&
              !player.IsInWallAction() &&
              player.CarriedItem == null);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      thrown = false;
    }
    #endregion
  }
}
