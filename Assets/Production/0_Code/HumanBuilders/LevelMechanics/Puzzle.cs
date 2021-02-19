using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A puzzle with specific elements that need to be reset when the player
  /// exits the room. Once solved, this type of puzzle will stay solved even if
  /// the player leaves and comes back.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public class Puzzle : Resetting, IStorable {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The elements in this puzzle.
    /// </summary>
    public List<PuzzleElement> Elements;

    /// <summary>
    /// Whether or not the puzzle's been solved.
    /// </summary>
    public bool Solved { get { return solved; } }

    /// <summary>
    /// Whether or not the puzzle's been solved.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether or not this puzzle's been solved.")]
    private bool solved;

    /// <summary>
    /// A reference to the game objects unique ID.
    /// </summary>
    private GuidComponent guid;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      guid = GetComponent<GuidComponent>();
      foreach (PuzzleElement element in Elements) {
        element.SetPuzzle(this);
      }
    }

    private void Start() {
      Retrieve();
    }
    #endregion


    #region Storable API
    //-------------------------------------------------------------------------
    // Storable API
    //-------------------------------------------------------------------------
    public void Store() {
      string folder = StaticFolders.BEHAVIOR;
      string key = guid.ToString()+Keys.SOLVED;
      VSave.Set(folder, key, solved);
    }

    public void Retrieve() {
      string folder = StaticFolders.BEHAVIOR;
      string key = guid.ToString()+Keys.SOLVED;

      if (VSave.Get(folder, key, out bool value)) {
        solved = value;

        // Resolve the puzzle if it's already been solved.
        if (solved) {
          foreach (PuzzleElement element in Elements) {
            if (element.Solvable) {
              element.Solve();
            }
          }
        }
      }
    }
    #endregion

    #region Resetting API
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------
    /// <summary>
    /// Reset this puzzle if it hasn't been completely solved.
    /// </summary>
    public override void ResetValues() {
      if (!solved) {
        foreach (PuzzleElement element in Elements) {
          element.ResetElement();
        }
      }
    }
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void CheckSolved() {
      foreach (PuzzleElement element in Elements) {
        if (element.Solvable && !element.Solved) {
          return;
        }
      }

      solved = true;
      Store();
    }
    #endregion
  }
}