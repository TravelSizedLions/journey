using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// A collection of settings for player movement. This class is meant to expose movement parameters to the Unity Editor.
  /// </summary>
  public class MovementSettings : MonoBehaviour {

    #region Running
    [Header("Running", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The maximum speed the player can move horizontally.
    /// </summary>
    [Tooltip("The maximum speed the player can move horizontally.")]
    public float MaxSpeed = 44f;

    /// <summary>
    /// How quickly the player accelerates to max speed. Higher = faster acceleration.
    /// </summary>
    [Tooltip("How quickly the player accelerates to max speed. Higher = faster acceleration.")]
    [Range(0, 1)]
    public float Acceleration = 0.25f;

    /// <summary>
    /// How quickly the player decelerates. Higher = faster deceleration.
    /// </summary>
    [Tooltip("How quickly the player decelerates. Higher = faster deceleration.")]
    [Range(0, 1)]
    public float Deceleration = 0.2f;

    /// <summary>
    /// How quickly the player turns around while in motion. Higher = faster turn around time.
    /// </summary>
    [Tooltip("How quickly the player turns around while in motion. Higher = faster turn around time.")]
    public float Agility = 4f;


    /// <summary>
    /// The speed at which the player is considered idle for animation purposes.
    /// </summary>
    [Tooltip("The speed at which the player is considered idle for animation purposes.")]
    public float IdleThreshold = 0.05f;

    [Space(10, order=2)]
    #endregion


    #region Jumping  
    [Header("Jumping", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// The strength of the player's first jump.
    /// </summary>
    [Tooltip("The strength of the player's first jump.")]
    public float SingleJumpForce = 48f;

    /// <summary>
    /// The strength of the player's second jump.
    /// </summary>
    [Tooltip("The strength of the player's second jump.")]
    public float DoubleJumpForce = 48f;

    /// <summary>
    /// How long the player needs to fall (in seconds) before transitioning with a roll.
    /// </summary>
    [Tooltip("How long the player needs to fall (in seconds) before transitioning with a roll.")]
    public float RollOnLand = 0.5f;

    [Space(10, order=5)]
    #endregion


    #region Crawling
    [Header("Crawling", order=6)]
    [Space(5, order=7)]

    /// <summary>
    /// How quickly the player can crawl. Acceleration and deceleration do not affect crawl speed.
    /// </summary>
    [Tooltip("How quickly the player can crawl. Acceleration and deceleration do not affect crawl speed.")]
    public float CrawlSpeed = 18f;

    /// <summary>
    /// The hop performed by the player when diving into a crawl.
    /// </summary>
    [Tooltip("The hop performed by the player when diving into a crawl.")]
    public Vector2 DiveHop;
    [Space(10, order=8)]
    #endregion

    #region Wall Run/Jump
    [Header("Wall Run/Jump", order=9)]
    [Space(5, order=10)]

    /// <summary>
    /// The strength of the player's wall jump (both vertical & horizontal forces).
    /// </summary>
    [Tooltip("The strength of the player's wall jump (both vertical & horizontal forces).")]
    public Vector2 WallJump;

    /// <summary>
    /// How easy it is for the player to get back to the wall after a
    /// wall jump. Higher is easier.
    /// </summary>
    [Tooltip("How easy it is for the player to get back to the wall after a wall jump. Higher is easier.")]
    public float WallJumpMuting = 0.08f;

    /// <summary>
    /// The speed the player runs up walls.
    /// </summary>
    [Tooltip("The speed the player runs up walls.")]
    public float WallRunSpeed = 0.125f;

    /// <summary>
    /// The ammount of time the player is allow to continue climbing the wall when wall running.
    /// </summary>
    [Tooltip("The ammount of time the player is allow to continue climbing the wall when wall running.")]
    public float WallRunAscensionTime = 48f;


    /// <summary>
    /// The initial burst of speed a player gets when running up a wall from the ground.
    /// </summary>
    [Tooltip("The initial burst of speed a player gets when running up a wall from the ground.")]
    public float WallRunBoost = 48f;


    /// <summary>
    /// How much wall sliding slows the player's fall. 0 - No deceleration. 1 - Complete stop.
    /// </summary>
    [Tooltip("How much wall sliding slows the player's fall. 0 - No deceleration. 1 - Complete stop.")]
    [Range(0, 1)]
    public float WallSlideDeceleration;

    [Space(10, order=11)]
    #endregion



    #region Input Buffers
    [Header("Input Buffers", order=12)]
    [Space(5, order=13)]

    /// <summary>
    /// How long the player is allowed to fall from a ledge before a jump is considered a double jump. 
    /// This gives the player a little wiggle room when performing a running jump off the edge of a platform.
    /// </summary>
    [Tooltip("How long the player is allowed to fall from a ledge before a jump is considered a double jump.\nThis gives the player a little wiggle room when performing a running jump off the edge of a platform.")]
    public float CoyoteTime = 0.15f;

    /// <summary>
    /// How close the player is allowed to be to the ground in order to register a jump input.
    /// Gives the player a little wiggle room when performing consecutive jumps from the ground.
    /// </summary>
    [Tooltip("How close the player is allow to be to the ground in order to register a jump input.\nGives the player a little wiggle room when performing consecutive jumps from the ground.")]
    public float GroundJumpBuffer = 0.5f;

    /// <summary>
    /// How close the player is allowed to be to a wall in order to register a wall jump.
    /// Gives the player a little wiggle room when performing consecutive wall jumps.
    /// When the wall jump buffer conflicts with the ground jump buffer, the jumping from the ground takes precedence.
    /// </summary>
    [Tooltip("How close the player is allowed to be to a wall in order to register a wall jump.\nGives the player a little wiggle room when performing consecutive wall jumps.\nWhen the wall jump buffer conflicts with the ground jump buffer, the jumping from the ground takes precedence.")]
    public float WallJumpBuffer = 0.5f;

    /// <summary>
    /// The how close to the floor the player needs to be to ascend a wall.
    /// </summary>
    [Tooltip("The how close to the floor the player needs to be to ascend a wall.")]
    public float WallRunBuffer = 2f;

    #endregion
  }
}