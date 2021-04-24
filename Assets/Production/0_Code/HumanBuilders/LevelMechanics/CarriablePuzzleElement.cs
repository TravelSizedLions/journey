using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A component that turns a carriable object into a puzzle element.
  /// </summary>
  public class CarriablePuzzleElement : PuzzleElement {
    protected override void OnResetElement() {
      SavePosition pos = GetComponent<SavePosition>();
      pos.Clear();
      Rigidbody2D rb = pos.GetComponent<Rigidbody2D>();
      rb.velocity = Vector3.zero;
    }

    protected override bool IsSolved(object info) {
      return false;
    }

    protected override void OnSolved() {
      // Left intentionally blank.
    }
  }
}