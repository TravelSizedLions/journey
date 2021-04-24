using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// A specific element of a puzzle room the player can solve.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public abstract class PuzzleElement : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// Whether or not this element can be "solved."
    /// </summary>
    [Tooltip("Whether or not this element can be solved.")]
    public bool Solvable;

    /// <summary>
    /// Whether or not this element has been solved.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    public bool Solved { get { return solved; } }

    /// <summary>
    /// Whether or not this element has been solved.
    /// </summary>
    [Tooltip("Whether or not this element has been solved.")]
    [SerializeField]
    [ReadOnly]
    private bool solved;

    /// <summary>
    /// The puzzle this element belongs to.
    /// </summary>
    private Puzzle puzzle;

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Inject the puzzle this element belongs to.
    /// </summary>
    /// <param name="puzzle">The puzzle.</param>
    public void SetPuzzle(Puzzle puzzle) {
      this.puzzle = puzzle;
    }

    /// <summary>
    /// Solve this puzzle element.
    /// </summary>
    public void Solve() {
      if (Solvable) {
        solved = true;
        OnSolved();
        if (puzzle != null) {
          puzzle.CheckSolved();
        }
      }
    }

    /// <summary>
    /// What happens when this element is solved.
    /// </summary>
    protected abstract void OnSolved();

    protected abstract bool IsSolved(object info);

    /// <summary>
    /// Reset this puzzle element to its starting position.
    /// </summary>
    public void ResetElement() {
      if (Solvable) {
        solved = false;
      }

      OnResetElement();
    }

    /// <summary>
    /// What happens when you reset this element.
    /// </summary>
    protected abstract void OnResetElement();
    #endregion
  }
}