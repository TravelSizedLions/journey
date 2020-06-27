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
    /// <seealso cref="PlayerCharacter.Physics" />
    IPhysics Physics { get; set; }

    /// <summary>
    /// The center of the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.Center" />
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
    /// <seealso cref="PlayerCharacter.StartCoyoteTime" />
    void StartCoyoteTime();

    /// <summary>
    /// Whether or not the player can still register a jump input.
    /// </summary>
    /// <seealso cref="PlayerCharacter.InCoyoteTime" />
    /// <returns>True if the player is still close enough to the ledge to
    /// register a jump. False otherwise.</returns>
    bool InCoyoteTime();

    /// <summary>
    /// Use up the remaining coyote time to perform a junmp.
    /// </summary>
    /// <seealso cref="PlayerCharacter.UseCoyoteTime" />
    void UseCoyoteTime();
  }
  #endregion

  #region Collisions/Distance Checking.
  /// <summary>
  /// The segment of the player interface that deals with collision/distance checking.
  /// </summary>
  public interface IPlayerCollision {

    /// <summary>
    /// Delegate class for collision/distance sensing.
    /// </summary>
    /// <seealso cref="PlayerCharacter.CollisionSensor">
    ICollision CollisionSensor { get; set; }

    /// <summary>
    /// How far the object is from the ground.
    /// </summary>
    /// <seealso cref="PlayerCharacter.DistanceToGround" />
    /// <returns>The distance between the object's feet and the closest piece of ground.</returns>
    float DistanceToGround();

    /// <summary>
    /// How far the object is from a left-hand wall.
    /// </summary>
    /// <seealso cref="PlayerCharacter.DistanceToLeftWall" />
    /// <returns>The distance between the object's left side and the closest left-hand wall.</returns>
    float DistanceToLeftWall();

    /// <summary>
    /// How far the object is from a right-hand wall.
    /// </summary>
    /// <seealso cref="PlayerCharacter.DistanceToRightWall" />
    /// <returns>The distance between the object's right side and the closest right-hand wall.</returns>
    float DistanceToRightWall();

    /// <summary>
    /// How far the object is from the closest wall.
    /// </summary>
    /// <seealso cref="PlayerCharacter.DistanceToWall" />
    /// <returns>The distance between the object and the closest wall.</returns>
    float DistanceToWall();

    /// <summary>
    /// Whether or not the object is touching the ground.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsTouchingGround" />
    bool IsTouchingGround();

    /// <summary>
    /// Whether or not the object is touching a left-hand wall.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsTouchingLeftWall" />
    bool IsTouchingLeftWall();

    /// <summary>
    /// Whether or not the object is touching a right-hand wall.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsTouchingRightWall" />
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
    /// <seealso cref="PlayerCharacter.Interaction" />
    IInteractionComponent Interaction { get; set; }

    /// <summary>
    /// The current item the player is carrying.
    /// </summary>
    /// <seealso cref="PlayerCharacter.CarriedItem" />
    Carriable CarriedItem { get; set; }

    /// <summary>
    /// Add an object to the list of objects the player is close enough to interact with.
    /// </summary>
    /// <seealso cref="PlayerCharacter.AddInteractible" />
    /// <param name="interactible">The object to add.</param>
    void AddInteractible(Interactible interactible);

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <seealso cref="PlayerCharacter.RemoveInteractible" />
    /// <param name="interactible">The object to remove.</param>
    void RemoveInteractible(Interactible interactible);

    /// <summary>
    /// Try to interact with the closest interactive object.
    /// </summary>
    /// <seealso cref="PlayerCharacter.Interact" />
    void Interact();

    /// <summary>
    /// Register an interaction indicator with the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.RegisterIndicator" />
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
    /// <seealso cref="PlayerCharacter.PressedJump" />
    /// <returns>True if the player pressed the jump button.</returns>
    bool PressedJump();

    /// <summary>
    /// Checks if the player is holding the jump button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.HoldingJump" />
    /// <returns>True if the player is holding the jump button.</returns>
    bool HoldingJump();

    /// <summary>
    /// Checks whether or not the player is trying to move horizontally, and whether or not they're allowed to.
    /// </summary>
    /// <seealso cref="PlayerCharacter.TryingToMove" />
    /// <returns>True if the player should move.</returns>
    bool TryingToMove();

    /// <summary>
    /// Checks if the player has pressed the up button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.PressedJump" />
    /// <returns>True if the player pressed up in the current frame.</returns>
    bool PressedUp();

    /// <summary>
    /// Checks if the player is holding down the up button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.HoldingUp" />
    /// <returns>True if the player is holding down the up button</returns>
    bool HoldingUp();

    /// <summary>
    /// Checks if the player has released the up button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.ReleasedUp" />
    /// <returns>True if the player has released up.</returns>
    bool ReleasedUp();

    /// <summary>
    /// Checks if the player has pressed the down button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.PressedDown" />
    /// <returns>True if the player pressed down in the current frame.</returns>
    bool PressedDown();

    /// <summary>
    /// Checks if the player is holding down the down button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.HoldingDown" />
    /// <returns>True if the player is holding down the down button</returns>
    bool HoldingDown();

    /// <summary>
    /// Checks if the player has released the down button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.ReleasedDown" />
    /// <returns>True if the player has released down.</returns>
    bool ReleasedDown();

    /// <summary>
    /// Gets the horizontal input for the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.GetHorizontalInput" />
    /// <returns>The horizontal input for the player. < 0 means left, > 0 means right, 0 means no movement.</returns>
    float GetHorizontalInput();

    /// <summary>
    /// Whether or not the player has pressed the action button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.PressedAction" />
    bool PressedAction();

    /// <summary>
    /// Whether or not the player is holding the action button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.HoldingAction" />
    bool HoldingAction();

    /// <summary>
    /// Whether or not the player released the action button.
    /// </summary>
    /// <seealso cref="PlayerCharacter.ReleasedAction" />
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
    /// <seealso cref="PlayerCharacter.Facing" />
    Facing Facing { get; }

    /// <summary>
    /// Sets the direction that the player is facing.
    /// </summary>
    /// <seealso cref="PlayerCharacter.SetFacing" />
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
    /// <seealso cref="PlayerCharacter.CanJump" />
    bool CanJump();

    /// <summary>
    /// Disable jumping for the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.DisableJump" />
    void DisableJump();

    /// <summary>
    /// Enable jumping for the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.EnableJump" />
    void EnableJump();

    /// <summary>
    /// Whether or not movement is enabled for the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.CanMove" />
    bool CanMove();

    /// <summary>
    /// Disable movement for the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.DisableJump" />
    void DisableMove();

    /// <summary>
    /// Enable movement for the player.
    /// </summary>
    /// <seealso cref="PlayerCharacter.EnableMove" />
    void EnableMove();

    /// <summary>
    /// Signal that the player detached from a platform.
    /// </summary>
    /// <seealso cref="PlayerCharacter.DisablePlatformMomentum" />
    void DisablePlatformMomentum();

    /// <summary>
    /// Signal that the player is attached to a platform.
    /// </summary>
    /// <seealso cref="PlayerCharacter.EnablePlatformMomentum" />
    void EnablePlatformMomentum();

    /// <summary>
    /// Whether or not the player is attached to a moving platform.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsPlatformMomentumEnabled" />
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
    /// <seealso cref="PlayerCharacter.IsWallJumping" />
    bool IsWallJumping();

    /// <summary>
    /// Whether or not the player is rising.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsRising" />
    bool IsRising();

    /// <summary>
    /// Whether or not the player is falling.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsFalling" />
    bool IsFalling();

    /// <summary>
    /// Whether or not the player is crouching.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsCrouching" />
    /// <returns>True if the player is crouching or starting/ending a crouch,
    /// false otherwise.</returns>
    bool IsCrouching();

    /// <summary>
    /// Whether or not the player is crawling.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsCrawling" />
    bool IsCrawling();

    /// <summary>
    /// Whether or not the player is diving into a crawl.
    /// </summary>
    /// <seealso cref="PlayerCharacter.IsDiving" />
    bool IsDiving();
  }
  #endregion
}