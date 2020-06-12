using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using UnityEngine;

namespace Storm.UI {

  /// <summary>
  /// An arrow that stores and displays a charge value up to a specified Maximun.
  /// </summary>
  public class ChargingArrow : MonoBehaviour {
    #region Variables
    #region Debugging Info
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
    #endregion


    /// <summary>
    /// The charging arrow's animator component.
    /// </summary>
    private Animator animator;

    #endregion

    #region Unity API

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      animator = GetComponentInChildren<Animator>();
      currentCharge = 0f;
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

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

    #endregion

  }

}