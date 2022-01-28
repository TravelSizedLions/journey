using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HumanBuilders {
  public class CompanionInterestZone : MonoBehaviour {
    public Transform InterestPoint;
    public void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player") && GameManager.Companion != null) {
        GameManager.Companion.SetInterestPoint(InterestPoint);
      }
    }

    public void OnTriggerExit2D(Collider2D col) {
      if (col.CompareTag("Player") && GameManager.Companion != null) {
        GameManager.Companion.ClearInterestPoint();
      }
    }
  }
}