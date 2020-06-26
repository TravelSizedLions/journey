using Storm;
using Storm.Components;
using Storm.Flexible.Interaction;
using UnityEngine;

namespace Storm.Characters.Player {
  /// <summary>
  /// The player interface.
  /// </summary>
  public interface IPlayer : 
    IMonoBehaviour,
    IPlayerInput, 
    IPlayerPhysics, 
    IPlayerInteraction, 
    IPlayerCollision, 
    IPlayerFacing,
    IPlayerCoyoteTime, 
    IPlayerToggles,
    IPlayerStateCheck {}

  #region Player Physics
  /// <summary>
  /// The physics segment of the player interface.
  /// </summary>
  public interface IPlayerPhysics {
    /// <summary>
    /// The delegate class for handling physics.
    /// </summary>
    IPhysics Physics { get; set; }

    /// <summary>
    /// The center of the player.
    /// </summary>
    Vector2 Center { get; }
  }
  #endregion

  #region Coyote Time
  /// <summary>
  /// The segment of the player interface that deals with coyote time.
  /// </summary>
  public interface IPlayerCoyoteTime {
    /// <summary>
    /// Reset the coyote timer.
    /// </summary>
    void StartCoyoteTime();

    /// <summary>
    /// Whether or not the player can still register a jump input.
    /// </summary>
    /// <returns>True if the player is still close enough to the ledge to
    /// register a jump. False otherwise.</returns>
    bool InCoyoteTime();

    /// <summary>
    /// Use up the remaining coyote time to perform a junmp.
    /// </summary>
    void UseCoyoteTime();
  }
  #endregion

  #region Collisions/Distance Checking.
  /// <summary>
  /// The segment of the player interface that deals with collision/distance checking.
  /// </summary>
  public interface IPlayerCollision {
    ICollision CollisionSensor { get; set; }

    /// <summary>
    /// How far the object is from the ground.
    /// </summary>
    /// <returns>The distance between the object's feet and the closest piece of ground.</returns>
    float DistanceToGround();

    /// <summary>
    /// How far the object is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the object's left side and the closest left-hand wall.</returns>
    float DistanceToLeftWall();

    /// <summary>
    /// How far the object is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the object's right side and the closest right-hand wall.</returns>
    float DistanceToRightWall();

    /// <summary>
    /// How far the object is from the closest wall.
    /// </summary>
    /// <returns>The distance between the object and the closest wall.</returns>
    float DistanceToWall();

    /// <summary>
    /// Whether or not the object is touching the ground.
    /// </summary>
    bool IsTouchingGround();

    /// <summary>
    /// Whether or not the object is touching a left-hand wall.
    /// </summary>
    bool IsTouchingLeftWall();

    /// <summary>
    /// Whether or not the object is touching a right-hand wall.
    /// </summary>
    bool IsTouchingRightWall();
  }
  #endregion

  #region Interaction
  /// <summary>
  /// The segment of the player interface that deals with interacting with the environment.
  /// </summary>
  public interface IPlayerInteraction {
    /// <summary>
    /// The delegate class for handling environment interaction.
    /// </summary>
    /// <value></value>
    IInteractionComponent Interaction { get; set; }

    /// <summary>
    /// The current item the player is carrying.
    /// </summary>
    Carriable CarriedItem { get; set; }

    /// <summary>
    /// Add an object to the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to add.</param>
    void AddInteractible(Interactible interactible);

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to remove.</param>
    void RemoveInteractible(Interactible interactible);

    /// <summary>
    /// Try to interact with the closest interactive object.
    /// </summary>
    void Interact();

    /// <summary>
    /// Register an interaction indicator with the player.
    /// </summary>
    /// <param name="indicator">The indicator to register</param>
    void RegisterIndicator(Indicator indicator);
  }
  #endregion

  #region Player Input
  /// <summary>
  /// The segment of the player interface that deals with input checks.
  /// </summary>
  public interface IPlayerInput {
    /// <summary>
    /// Checks if the player pressed the jump button.
    /// </summary>
    /// <returns>True if the player pressed the jump button.</returns>
    bool PressedJump();

    /// <summary>
    /// Checks if the player is holding the jump button.
    /// </summary>
    /// <returns>True if the player is holding the jump button.</returns>
    bool HoldingJump();

    /// <summary>
    /// Checks whether or not the player is trying to move horizontally, and whether or not they're allowed to.
    /// </summary>
    /// <returns>True if the player should move.</returns>
    bool TryingToMove();

    /// <summary>
    /// Checks if the player has pressed the up button.
    /// </summary>
    /// <returns>True if the player pressed up in the current frame.</returns>
    bool PressedUp();

    /// <summary>
    /// Checks if the player is holding down the up button.
    /// </summary>
    /// <returns>True if the player is holding down the up button</returns>
    bool HoldingUp();

    /// <summary>
    /// Checks if the player has released the up button.
    /// </summary>
    /// <returns>True if the player has released up.</returns>
    bool ReleasedUp();

    /// <summary>
    /// Checks if the player has pressed the down button.
    /// </summary>
    /// <returns>True if the player pressed down in the current frame.</returns>
    bool PressedDown();

    /// <summary>
    /// Checks if the player is holding down the down button.
    /// </summary>
    /// <returns>True if the player is holding down the down button</returns>
    bool HoldingDown();

    /// <summary>
    /// Checks if the player has released the down button.
    /// </summary>
    /// <returns>True if the player has released down.</returns>
    bool ReleasedDown();

    /// <summary>
    /// Gets the horizontal input for the player.
    /// </summary>
    /// <returns>The horizontal input for the player. < 0 means left, > 0 means right, 0 means no movement.</returns>
    float GetHorizontalInput();

    /// <summary>
    /// Whether or not the player has pressed the action button.
    /// </summary>
    bool PressedAction();

    /// <summary>
    /// Whether or not the player is holding the action button.
    /// </summary>
    bool HoldingAction();

    /// <summary>
    /// Whether or not the player released the action button.
    /// </summary>
    bool ReleasedAction();
  }
  #endregion

  #region Player Facing
  /// <summary>
  /// The segment of the player interface dealing with which way the player faces.
  /// </summary>  
  public interface IPlayerFacing {
    /// <summary>
    /// Which way the player's facing.
    /// </summary>
    Facing Facing { get; }

    /// <summary>
    /// Sets the direction that the player is facing.
    /// </summary>
    /// <param name="facing">The direction enum</param>
    void SetFacing(Facing facing);
  }
  #endregion

  #region Player Toggles
  /// <summary>
  /// The segment of the player interface that deals with basic on/off toggles,
  /// like whether or not the player can move and jump.
  /// </summary>
  public interface IPlayerToggles {

    /// <summary>
    /// Whether or not jumping is enabled for the player.
    /// </summary>
    bool CanJump();

    /// <summary>
    /// Disable jumping for the player.
    /// </summary>
    void DisableJump();

    /// <summary>
    /// Enable jumping for the player.
    /// </summary>
    void EnableJump();

    /// <summary>
    /// Whether or not movement is enabled for the player.
    /// </summary>
    bool CanMove();

    /// <summary>
    /// Disable movement for the player.
    /// </summary>
    void DisableMove();

    /// <summary>
    /// Enable movement for the player.
    /// </summary>
    void EnableMove();

    /// <summary>
    /// Signal that the player detached from a platform.
    /// </summary>
    void DisablePlatformMomentum();

    /// <summary>
    /// Signal that the player is attached to a platform.
    /// </summary>
    void EnablePlatformMomentum();

    /// <summary>
    /// Whether or not the player is attached to a moving platform.
    /// </summary>
    bool IsPlatformMomentumEnabled();
  }

  #endregion

  #region Player State Checking
  /// <summary>
  /// The segment of the player interface that deals with checking information
  /// about the player's state.
  /// </summary>
  public interface IPlayerStateCheck {
    /// <summary>
    /// Whether or not the player is in the middle of a wall jump.
    /// </summary>
    bool IsWallJumping();

    /// <summary>
    /// Whether or not the player is rising.
    /// </summary>
    bool IsRising();

    /// <summary>
    /// Whether or not the player is falling.
    /// </summary>
    bool IsFalling();

    /// <summary>
    /// Whether or not the player is crouching.
    /// </summary>
    /// <returns>True if the player is crouching or starting/ending a crouch,
    /// false otherwise.</returns>
    bool IsCrouching();

    /// <summary>
    /// Whether or not the player is crawling.
    /// </summary>
    bool IsCrawling();

    /// <summary>
    /// Whether or not the player is diving into a crawl.
    /// </summary>
    bool IsDiving();
  }
  #endregion
}