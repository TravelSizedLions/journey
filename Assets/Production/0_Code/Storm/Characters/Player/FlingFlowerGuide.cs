using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.Characters;
using Storm.Characters.Player;
using Storm.LevelMechanics;
using UnityEngine;

namespace Storm.Characters.Player {

  public interface IFlingFlowerGuide : IMonoBehaviour {
    
    /// <summary>
    /// The current fling flower the player is attached to.
    /// </summary>
    /// <seealso cref="FlingFlowerGuide.CurrentFlower" />
    FlingFlower CurrentFlower { get; }

    /// <summary>
    /// Add some amount to the charging arrow.
    /// </summary>
    /// <param name="chargeAmount">The ammoun to add to the total charge.</param>
    /// <seealso cref="FlingFlowerGuide.Charge" />
    void Charge(float chargeAmount);

    /// <summary>
    /// Get the current charge of the arrow.
    /// </summary>
    /// <returns> The raw charge (not a percentage).</returns>
    /// <seealso cref="FlingFlowerGuide.GetCharge" />
    float GetCharge();

    /// <summary>
    /// Resets the current charge back to 0.
    /// </summary>
    /// <seealso cref="FlingFlowerGuide.ResetCharge" />
    void ResetCharge();

    /// <summary>
    /// Set the maximum
    /// </summary>
    /// <param name="max">The maximum value this can charge up to.</param>
    /// <seealso cref="FlingFlowerGuide.SetMaxCharge" />
    void SetMaxCharge(float max);

    /// <summary>
    /// The amount the arrow has charged as a percentage.
    /// </summary>
    /// <returns>A percentage (0 - 1).</returns>
    /// <seealso cref="FlingFlowerGuide.GetPercentCharged" />
    float GetPercentCharged();

    /// <summary>
    /// Display this guide.
    /// </summary>
    /// <seealso cref="FlingFlowerGuide.Show" />
    void Show();

    /// <summary>
    /// Hide this guide.
    /// </summary>
    /// <seealso cref="FlingFlowerGuide.Hide" />
    void Hide();

    /// <summary>
    /// Set the currently active fling flower.
    /// </summary>
    /// <param name="flower">The flower.</param>
    /// <seealso cref="FlingFlowerGuide.SetFlingFlower" />
    void SetFlingFlower(FlingFlower flower);


    /// <summary>
    /// Recalculates the rotation of the guide based on the position of the
    /// player and the mouse. 
    /// </summary>
    /// <seealso cref="FlingFlowerGuide.RotateGuide" />
    void RotateGuide();
  }

  /// <summary>
  /// An arrow that stores and displays a charge value up to a specified Maximun.
  /// </summary>
  public class FlingFlowerGuide : MonoBehaviour, IFlingFlowerGuide {
    #region Variables

    /// <summary>
    /// The current fling flower the player is attached to.
    /// </summary>
    public FlingFlower CurrentFlower { get { return currentFlingFlower; } }

    [Header("Debugging Info", order = 0)]
    [Space(5, order = 1)]
    /// <summary>
    /// The maximum charge the arrow can hold.
    /// </summary>
    [Tooltip("The maximum charge of the arrow.")]
    [SerializeField]
    [ReadOnly]
    private float maxCharge;


    /// <summary>
    /// The current charge of the arrow.
    /// </summary>
    [Tooltip("The current charge of the arrow.")]
    [SerializeField]
    [ReadOnly]
    private float currentCharge;

    /// <summary>
    /// The charging arrow's animator component.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private PlayerCharacter player;

    /// <summary>
    /// The currently active fling flower.
    /// </summary>
    private FlingFlower currentFlingFlower;


    /// <summary>
    /// The sprite that displays this guide.
    /// </summary>
    private SpriteRenderer sprite;
    #endregion

    #region Unity API

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      animator = GetComponentInChildren<Animator>();
      currentCharge = 0f;
      player = GetComponentInParent<PlayerCharacter>();
      sprite = GetComponent<SpriteRenderer>();
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Recalculates the rotation of the guide based on the position of the
    /// player and the mouse. 
    /// </summary>
    public void RotateGuide() {
      Vector2 direction = player.GetMouseDirection();
      float angleDeg = Mathf.Rad2Deg*Mathf.Atan2(direction.y, direction.x);
      transform.rotation = Quaternion.Euler(0, 0, angleDeg);
    }


    /// <summary>
    /// Add some amount to the charging arrow.
    /// </summary>
    /// <param name="chargeAmount">The ammoun to add to the total charge.</param>
    public void Charge(float chargeAmount) {
      currentCharge += chargeAmount;
      if (currentCharge > maxCharge) {
        currentCharge = maxCharge;
      }

      animator.SetFloat("Charge", currentCharge / maxCharge);
    }

    /// <summary>
    /// Get the current charge of the arrow.
    /// </summary>
    /// <returns> The raw charge (not a percentage).</returns>
    public float GetCharge() {
      return currentCharge;
    }

    /// <summary>
    /// Resets the current charge back to 0.
    /// </summary>
    public void ResetCharge() {
      currentCharge = 0;
      animator.SetFloat("Charge", 0);
    }

    /// <summary>
    /// Set the maximum
    /// </summary>
    /// <param name="max"></param>
    public void SetMaxCharge(float max) {
      maxCharge = max;
    }

    /// <summary>
    /// The amount the arrow has charged as a percentage.
    /// </summary>
    /// <returns>A percentage (0 - 1).</returns>
    public float GetPercentCharged() {
      return currentCharge / maxCharge;
    }

    /// <summary>
    /// Display this guide for the player.
    /// </summary>
    public void Show() {
      enabled = true;
      sprite.enabled = true;
    }

    /// <summary>
    /// Hide this guide from the player.
    /// </summary>
    public void Hide() {
      enabled = false;
      if (sprite != null) {
        sprite.enabled = false;
      }
    }

    /// <summary>
    /// Set currently active fling flower.
    /// </summary>
    /// <param name="flower">The flower.</param>
    public void SetFlingFlower(FlingFlower flower) {
      currentFlingFlower = flower;
    }
    #endregion
  }

}