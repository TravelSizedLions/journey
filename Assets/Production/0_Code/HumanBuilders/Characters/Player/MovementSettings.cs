using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A collection of settings for player movement. This class is meant to expose movement parameters to the Unity Editor.
  /// </summary>
  public class MovementSettings : MonoBehaviour {

    #region Running

    /// <summary>
    /// The maximum speed the player can move horizontally.
    /// </summary>
    [FoldoutGroup("Running")]
    [Tooltip("The maximum speed the player can move horizontally.")]
    public float MaxSpeed = 24f;

    /// <summary>
    /// How quickly the player accelerates to max speed. Higher = faster acceleration.
    /// </summary>
    [FoldoutGroup("Running")]
    [Tooltip("How quickly the player accelerates to max speed. Higher = faster acceleration.")]
    [Range(0, 1)]
    public float Acceleration = 0.25f;

    /// <summary>
    /// How quickly the player decelerates. Higher = faster deceleration.
    /// </summary>
    [FoldoutGroup("Running")]
    [Tooltip("How quickly the player decelerates. Higher = faster deceleration.")]
    [Range(0, 1)]
    public float Deceleration = 0.2f;

    /// <summary>
    /// How quickly the player turns around while in motion. Higher = faster turn around time.
    /// </summary>
    [FoldoutGroup("Running")]
    [Tooltip("How quickly the player turns around while in motion. Higher = faster turn around time.")]
    public float Agility = 4f;


    /// <summary>
    /// The speed at which the player is considered idle for animation purposes.
    /// </summary>
    [FoldoutGroup("Running")]
    [Tooltip("The speed at which the player is considered idle for animation purposes.")]
    public float IdleThreshold = 0.05f;

    #endregion


    #region Jumping  

    /// <summary>
    /// The strength of the player's first jump.
    /// </summary>
    [FoldoutGroup("Jumping")]
    [Tooltip("The strength of the player's first jump.")]
    public float SingleJumpForce = 48f;

    /// <summary>
    /// The strength of the player's second jump.
    /// </summary>
    [FoldoutGroup("Jumping")]
    [Tooltip("The strength of the player's second jump.")]
    public float DoubleJumpForce = 48f;

    /// <summary>
    /// The maximum speed the player can fall.
    /// </summary>
    [FoldoutGroup("Jumping")]
    [Tooltip("The maximum speed the player can fall.")]
    public float MaxFallSpeed = 48f;


    /// <summary>
    /// How long the player needs to fall (in seconds) before transitioning with a roll.
    /// </summary>
    [FoldoutGroup("Jumping")]
    [Tooltip("How long the player needs to fall (in seconds) before transitioning with a roll.")]
    public float RollOnLand = 0.5f;

    #endregion


    #region Crawling

    /// <summary>
    /// How quickly the player can crawl. Acceleration and deceleration do not affect crawl speed.
    /// </summary>
    [FoldoutGroup("Crawling")]
    [Tooltip("How quickly the player can crawl. Acceleration and deceleration do not affect crawl speed.")]
    public float CrawlSpeed = 18f;

    /// <summary>
    /// The hop performed by the player when diving into a crawl.
    /// </summary>
    [FoldoutGroup("Crawling")]
    [Tooltip("The hop performed by the player when diving into a crawl.")]
    public Vector2 DiveHop;

    /// <summary>
    /// Minimum space away from a wall required to crawl from
    /// a crouch.
    /// </summary>
    [FoldoutGroup("Crawling")]
    [Tooltip("Minimum space away from a wall required to crawl from a crouch.")]
    public float MinCrawlSpace = 0.95f;

    /// <summary>
    /// The minimum space away from a wall required to performed a dive.
    /// </summary>
    [FoldoutGroup("Crawling")]
    [Tooltip("The minimum space away from a wall required to performed a dive.")]
    public float MinDiveSpace = 0.95f;

    #endregion

    #region Wall Run/Jump

    /// <summary>
    /// The strength of the player's wall jump (both vertical & horizontal forces).
    /// </summary>
    [FoldoutGroup("Wall Run + Jump")]
    [Tooltip("The strength of the player's wall jump (both vertical & horizontal forces).")]
    public Vector2 WallJump;

    /// <summary>
    /// How easy it is for the player to get back to the wall after a
    /// wall jump. Higher is easier.
    /// </summary>
    [FoldoutGroup("Wall Run + Jump")]
    [Tooltip("How easy it is for the player to get back to the wall after a wall jump. Higher is easier.")]
    public float WallJumpMuting = 0.08f;

    /// <summary>
    /// The speed the player runs up walls.
    /// </summary>
    [FoldoutGroup("Wall Run + Jump")]
    [Tooltip("The speed the player runs up walls.")]
    public float WallRunSpeed = 0.125f;

    /// <summary>
    /// The ammount of time the player is allow to continue climbing the wall when wall running.
    /// </summary>
    [FoldoutGroup("Wall Run + Jump")]
    [Tooltip("The ammount of time the player is allow to continue climbing the wall when wall running.")]
    public float WallRunAscensionTime = 48f;


    /// <summary>
    /// The initial burst of speed a player gets when running up a wall from the ground.
    /// </summary>
    [FoldoutGroup("Wall Run + Jump")]
    [Tooltip("The initial burst of speed a player gets when running up a wall from the ground.")]
    public float WallRunBoost = 48f;


    /// <summary>
    /// How much wall sliding slows the player's fall. 0 - No deceleration. 1 - Complete stop.
    /// </summary>
    [FoldoutGroup("Wall Run + Jump")]
    [Tooltip("How much wall sliding slows the player's fall. 0 - No deceleration. 1 - Complete stop.")]
    [Range(0, 1)]
    public float WallSlideDeceleration;

    /// <summary>
    /// How much wall sliding slows the player's fall when pressing down. 0 - No deceleration. 1 - Complete stop.
    /// </summary>
    [FoldoutGroup("Wall Run + Jump")]
    [Tooltip("How much wall sliding slows the player's fall when pressing down. 0 - No deceleration. 1 - Complete stop.")]
    [Range(0, 1)]
    public float FastWallSlideDeceleration;

    #endregion



    #region Input Buffers

    /// <summary>
    /// How long the player is allowed to fall from a ledge before a jump is considered a double jump. 
    /// This gives the player a little wiggle room when performing a running jump off the edge of a platform.
    /// </summary>
    [FoldoutGroup("Input Buffers")]
    [Tooltip("How long the player is allowed to fall from a ledge before a jump is considered a double jump.\nThis gives the player a little wiggle room when performing a running jump off the edge of a platform.")]
    public float CoyoteTime = 0.15f;

    /// <summary>
    /// How close the player is allowed to be to the ground in order to register a jump input.
    /// Gives the player a little wiggle room when performing consecutive jumps from the ground.
    /// </summary>
    [FoldoutGroup("Input Buffers")]
    [Tooltip("How close the player is allow to be to the ground in order to register a jump input.\nThis gives the player a little wiggle room when performing consecutive jumps from the ground.")]
    public float GroundJumpBuffer = 0.5f;

    /// <summary>
    /// How close the player is allowed to be to a wall in order to register a wall jump.
    /// Gives the player a little wiggle room when performing consecutive wall jumps.
    /// When the wall jump buffer conflicts with the ground jump buffer, the jumping from the ground takes precedence.
    /// </summary>
    [FoldoutGroup("Input Buffers")]
    [Tooltip("How close the player is allowed to be to a wall in order to register a wall jump.\nThis gives the player a little wiggle room when performing consecutive wall jumps.\nWhen the wall jump buffer conflicts with the ground jump buffer, the jumping from the ground takes precedence.")]
    public float WallJumpBuffer = 0.5f;

    /// <summary>
    /// The how close to the floor the player needs to be to ascend a wall.
    /// </summary>
    [FoldoutGroup("Input Buffers")]
    [Tooltip("The how close to the floor the player needs to be to ascend a wall.")]
    public float WallRunBuffer = 2f;

    /// <summary>
    /// How long the player is allowed to fall away from a wall before they can
    /// no longer perform a wall jump.
    /// </summary>
    [FoldoutGroup("Input Buffers")]
    [Tooltip("How long the player is allowed to fall away from a wall before they can no longer perform a wall jump.\nThis gives the player a little wiggle room when performing a wall jump while moving away from a wall.")]
    public float WallJumpCoyoteTime = 0.15f;

    #endregion

    #region Carry Settings

    /// <summary>
    /// How quickly the character moves while carrying something.
    /// </summary>
    [FoldoutGroup("Carry Movement")]
    [Tooltip("How quickly the character moves while carrying something.")]
    public float MaxCarrySpeed = 24f;

    /// <summary>
    /// The strength of the player's jump when carrying something.
    /// </summary>
    [FoldoutGroup("Carry Movement")]
    [Tooltip("The strength of the player's jump when carrying something.")] 
    public float CarryJumpForce = 50f;

    
    /// <summary>
    /// The force that nudges an item a certain direction when it's dropped.
    /// </summary>
    [FoldoutGroup("Carry Movement")]
    [Tooltip("The force that nudges an item a certain direction when it's dropped.")]
    public Vector2 DropForce;


    /// <summary>
    /// The maximum throwing force for carried items.
    /// </summary>
    [FoldoutGroup("Carry Movement")]
    [Tooltip("The maximum throwing force for carried items.")]
    public float ThrowingForce;


    /// <summary>
    /// The number of points along the throwing arc indicator.
    /// </summary>
    [FoldoutGroup("Carry Movement")]
    [Tooltip("The number of points along the throwing arc indicator.")]
    public int ThrowingArcResolution;
    #endregion
  }
}