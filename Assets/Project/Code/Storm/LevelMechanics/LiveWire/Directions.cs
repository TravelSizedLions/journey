using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.LevelMechanics.LiveWire {

  public enum Direction { Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft }

  public static class Directions2D {

    #region Vector Direction Shortcuts
    public static readonly Vector2 Up = Vector2.up;
    public static readonly Vector2 UpRight = new Vector2(1, 1).normalized;
    public static readonly Vector2 Right = Vector2.right;
    public static readonly Vector2 DownRight = new Vector2(1, -1).normalized;
    public static readonly Vector2 Down = Vector2.down;
    public static readonly Vector2 DownLeft = new Vector2(-1, -1).normalized;
    public static readonly Vector2 Left = Vector2.left;
    public static readonly Vector2 UpLeft = new Vector2(-1, 1).normalized;

    #endregion

    /// <summary>
    /// Maps Direction enumerations to the corresponding direction vector.
    /// </summary>
    /// <param name="d">The direction enum</param>
    /// <returns>The corresponding direction vector.</returns>
    public static Vector2 toVector(Direction d) {
      switch (d) {
        case Direction.Up:
          return Directions2D.Up;
        case Direction.UpRight:
          return Directions2D.UpRight;
        case Direction.Right:
          return Directions2D.Right;
        case Direction.DownRight:
          return Directions2D.DownRight;
        case Direction.Down:
          return Directions2D.Down;
        case Direction.DownLeft:
          return Directions2D.DownLeft;
        case Direction.Left:
          return Directions2D.Left;
        case Direction.UpLeft:
          return Directions2D.UpLeft;
        default:
          return Directions2D.Up;
      }
    }

    /// <summary>
    /// Checks if two vectors are going roughly opposite directions
    /// </summary>
    /// <param name="v1">the first vector</param>
    /// <param name="v2">the second vector</param>
    /// <returns>true if the vectors are pointing roughly opposite directions (i.e. their dot product is less that 0).</returns>
    public static bool AreOppositeDirections(Vector2 v1, Vector2 v2) {
      Debug.Log("Dot product: " + Vector2.Dot(v1, v2) + ", magnitude: " + v2.magnitude);
      return Mathf.Approximately(Vector2.Dot(v1, v2) + v2.magnitude, 0) ;
    }

    /// <summary>
    /// Returns the angle between two vectors in degrees.
    /// </summary>
    /// <param name="v1">the first vector</param>
    /// <param name="v2">the second vector</param>
    public static float AngleBetween(Vector2 v1, Vector2 v2) {
      float cosTheta = Vector2.Dot(v1, v2) / (v1.magnitude * v2.magnitude);
      float thetaRads = Mathf.Acos(cosTheta);
      return Mathf.Rad2Deg * thetaRads;
    }

    /// <summary>
    /// Reverses a direction vector.
    /// </summary>
    public static Vector2 Reverse(Vector2 d) {
      return new Vector2(-d.x, -d.y);
    }
  }
}
