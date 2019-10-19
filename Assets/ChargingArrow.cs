using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingArrow : MonoBehaviour
{
    /// <summary>
    /// The charging arrow's animator component.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The current charge of the arrow.
    /// </summary>
    private float charge;

    /// <summary>
    /// The maximum charge of the arrow.
    /// </summary>
    private float maxCharge;

    // Start is called before the first frame update
    void Awake() {
        animator = GetComponentInChildren<Animator>();
        charge = 0f;
    }

    // Update is called once per frame
    void Update() {
        
    }

    /// <summary>
    /// Add some amount to the charging arrow.
    /// </summary>
    /// <param name="chargeAmount">The ammoun to add to the total charge.</param>
    public void Charge(float chargeAmount) {
        charge += chargeAmount;
        if (charge > maxCharge) {
            charge = maxCharge;
        }
        Debug.Log("Charging: "+charge);
        animator.SetFloat("Charge", charge/maxCharge);
    }

    /// <summary>
    /// Get the current charge of the arrow.
    /// </summary>
    /// <returns> The raw charge (not a percentage).</returns>
    public float GetCharge() {
        return charge;
    }

    /// <summary>
    /// Set the maximum
    /// </summary>
    /// <param name="max"></param>
    public void SetMaxCharge(float max) {
        maxCharge = max;
    }

    public float GetChargePercentage() {
        return charge/maxCharge;
    }

}
