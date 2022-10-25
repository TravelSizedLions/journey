
using System.Collections;




using UnityEngine;

namespace HumanBuilders.Graphing {

  /// <summary>
  /// Shared logic for having the player move to a particular position.
  /// </summary>
  public class WalkToPosition {

    /// <summary>
    /// Walk to in one direction until reaching the target.
    /// </summary>
    public static UnityTask WalkTo(Transform target, float speed, GraphEngine graphEngine, bool pauseGraph,  float delayAfter) {
      return new UnityTask(_MoveTo(target, speed, graphEngine, pauseGraph, delayAfter));
    }


    /// <summary>
    /// Walk to in one direction until reaching the target.
    /// </summary>
    public static IEnumerator _MoveTo(Transform target, float speed, GraphEngine graphEngine, bool pauseGraph,  float delayAfter) {
      GameManager.Player.EnableMove(DialogManager.Instance);
      bool walkLeft = GameManager.Player.Physics.Px > target.position.x;

      // Temporarily store the real player input.
      IInputComponent playerInput = GameManager.Player.PlayerInput;
      GameManager.Player.PlayerInput = new VirtualInput();

      StartWalking(walkLeft, speed);

      while (!ShouldStop(target, walkLeft)) {
        yield return null;
      }

      StopWalking(playerInput);
      GameManager.Player.Physics.Px = target.position.x;
      GameManager.Player.Physics.Vx = 0;
      GameManager.Player.DisableMove(DialogManager.Instance);
    }

    /// <summary>
    /// Have the player begin walking either left or right.
    /// </summary>
    /// <param name="walkLeft">Whether or not the player should walk left or right.</param>
    private static void StartWalking(bool walkLeft, float speed) {
      float input = walkLeft ? -speed : speed;
      GameManager.GamePad.SetHorizontalAxis(input);
    }

    /// <summary>
    /// Have the player stop walking.
    /// </summary>
    /// <param name="playerInput">The player's original input component</param>
    private static void StopWalking(IInputComponent playerInput) {
      GameManager.GamePad.SetHorizontalAxis(0);
      GameManager.Player.PlayerInput = playerInput;
    }

    /// <summary>
    /// Whether or not the player should stop moving towards the target.
    /// </summary>
    private static bool ShouldStop(Transform target, bool walkLeft) {
      if (walkLeft) {
        return GameManager.Player.Physics.Px <= target.position.x; 
      } else {
        return GameManager.Player.Physics.Px >= target.position.x;
      }
    }
  }
}