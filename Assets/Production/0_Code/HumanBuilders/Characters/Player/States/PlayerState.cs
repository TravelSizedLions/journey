using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// The base class for player states.
  /// </summary>
  [RequireComponent(typeof(PlayerCharacter))]
  public abstract class PlayerState : State {

    #region Fields

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    protected IPlayer player;

    /// <summary>
    /// Information about the player's physics.
    /// </summary>
    protected IPhysics physics;

    /// <summary>
    /// Settings about the player's movement.
    /// </summary>
    protected MovementSettings settings;

    /// <summary>
    /// Settings about the player's powers.
    /// </summary>
    protected PowersSettings powersSettings;

    /// <summary>
    /// The position of the mouse in screen space.
    /// </summary>
    protected Vector3 mousePositionScreen;

    /// <summary>
    /// The position of the mouse in world space.
    /// </summary>
    protected Vector3 mousePositionWorld;

    /// <summary>
    /// Whether or not the player has released the action or alt action button
    /// </summary>
    protected bool releasedAction;
    #endregion


    /// <summary>
    /// Injection point for state dependencies.
    /// </summary>
    public void Inject(IPlayer player, IPhysics physics, MovementSettings settings) {
      this.player = player;
      this.physics = physics;
      this.settings = settings;
    }


    /// <summary>
    /// Pre-hook called by the Player Character when a player state is first added to the player.
    /// </summary>
    public override void OnStateAddedGeneral() {
      player = GetComponent<PlayerCharacter>();
      physics = player.Physics;

      if (player.MovementSettings != null) {
        settings = player.MovementSettings;
      } else {
        settings = GetComponent<MovementSettings>();
      }

      if (player.PowersSettings != null) {
        powersSettings = player.PowersSettings;
      } else {
        powersSettings = GetComponent<PowersSettings>();
      }
    }


    public override void OnUpdateGeneral() {
      mousePositionScreen = player.GetMouseScreenPosition();
      mousePositionWorld = player.GetMouseWorldPosition();
      Vector3 playerHead = new Vector3(player.transform.position.x, player.transform.position.y + player.Collider.bounds.size.y);

      Debug.DrawLine(playerHead, mousePositionWorld, Color.blue);
    }


    public override void OnStateEnterGeneral() {
      releasedAction = player.ReleasedAction() || player.ReleasedAltAction() || !(player.HoldingAction() || player.HoldingAltAction());
    }

    /// <summary>
    /// Whether or not the player can carry this game object.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object can be carried. False otherwise.</returns>
    public bool CanCarry(GameObject obj) {
      PhysicalInteractible interactible = obj.GetComponent<PhysicalInteractible>();
      return (interactible != null) && (interactible is Carriable);
    }

    public Facing ProjectToWall() {
      float distToRight = player.DistanceToRightWall();
      float distToLeft = player.DistanceToLeftWall();
      
      Facing whichWall;

      if (distToLeft < distToRight) {
        whichWall = Facing.Left;
        player.Physics.Px -= Mathf.Min(distToLeft, 0.25f);
        
      } else {
        whichWall = Facing.Right;
        player.Physics.Px += Mathf.Min(distToRight, 0.25f);
      }

      return whichWall;
    }

    /// <summary>
    /// Whether or not the object is an aimable fling flower.
    /// </summary>
    /// <param name="obj">The object to check</param>
    /// <returns>True if the object is an aimable fling flower.</returns>
    public bool IsAimableFlingFlower(GameObject obj) {
      FlingFlower flower = obj.GetComponentInChildren<AimableFlingFlower>();
      if(flower != null) {
        player.FlingFlowerGuide.SetFlingFlower(flower);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Whether or not the object is a directional fling flower.
    /// </summary>
    /// <returns>True if the object is a directional fling flower.</returns>
    public bool IsDirectionalFlingFlower(GameObject obj) {
      FlingFlower flower = obj.GetComponentInChildren<DirectionalFlingFlower>();
      if(flower != null) {
        player.FlingFlowerGuide.SetFlingFlower(flower);
        return true;
      }
      return false;
    }
  }

}