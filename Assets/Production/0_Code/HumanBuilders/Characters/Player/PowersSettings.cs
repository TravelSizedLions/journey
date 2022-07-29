using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A collection of settings for player movement. This class is meant to expose movement parameters to the Unity Editor.
  /// </summary>
  public class PowersSettings : MonoBehaviour {

    /// <summary>
    /// How fast the player moves while traveling from a directed fling flower.
    /// </summary>
    [FoldoutGroup("Fling Flowers")]
    [LabelText("Directed - Velocity")]
    [Tooltip("How fast the player moves while traveling from a directed fling flower.")]
    public float FlingFlowerDirectedVelocity = 20f;

    /// <summary>
    /// How quickly the player charges a fling flower launch. For example, a value of 10
    /// means after 1 second of charge, the player will get flung at an additional
    /// 10 units/second.
    /// </summary>
    [FoldoutGroup("Fling Flowers")]
    [Tooltip("How quickly the player charges a fling flower launch. For example, a value of 10 means after 1 second of charge, the player will get flung at an additional 10 units/second.")]
    [LabelText("Aimable - Charge Speed")]
    public float AimableFlingFlowerChargeSpeed = 40f;

    /// <summary>
    /// The maximum velocity of an aimable fling flower launch, in units/second.
    /// </summary>
    [FoldoutGroup("Fling Flowers")]
    [LabelText("Aimable - Max Velocity")]
    [Tooltip("The maximum velocity of an aimable fling flower launch, in units/second.")]
    public float AimableFlingFlowerMaxVelocity = 40f;

    /// <summary>
    /// How much input control the player has when they're in a fling flower
    /// ballistic trajectory.
    /// </summary>
    [FoldoutGroup("Fling Flowers")]
    [LabelText("Aimable - Air Control")]
    [Tooltip("How much input control the player has when they're in a fling flower ballistic trajectory.")]
    public Vector2 FlingFlowerAirControl = Vector2.zero;

    /// <summary>
    /// How quickly the player decelerates once air control is used during a
    /// fling flower ballistic trajectory.
    /// </summary>
    [FoldoutGroup("Fling Flowers")]
    [LabelText("Aimable - Air Control Decel")]
    [Tooltip("How quickly the player decelerates once air control is used during a fling flower ballistic trajectory.")]
    public float FlingFlowerAirControlDeceleration = 0.8f;

    /// <summary>
    /// The force of the hop the player preforms after landing from a fling
    /// flower trip.
    /// </summary>
    [FoldoutGroup("Fling Flowers")]
    [LabelText("Landing Hop")]
    [Tooltip("The force of the hop the player preforms after landing from a fling flower trip.")]
    public float FlingFlowerLandingHopForce = 20f;

    /// <summary>
    /// How quickly the player gravitates towards fling flowers once touched.
    /// </summary>
    [FoldoutGroup("Fling Flowers")]
    [Tooltip("How quickly the player gravitates towards fling flowers once touched.")]
    [LabelText("Gravitation")]
    public float FlingFlowerGravitation = 0.5f;

    [FoldoutGroup("Grappling Thistle")]
    [Tooltip("Whether or not the player can use the grappling thistle.")]
    [LabelText("Enabled")]
    public bool GrapplingThistleEnabled = false;
  }
}