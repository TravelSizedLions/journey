using UnityEngine;

namespace Storm.Math {
  public static class Pixels {
    public static float pixelsPerUnit = 16f;

    private static float pixelWidth = 1/16f;

    /// <summary>
    /// Map a position to pixel-friendly coordinates.
    /// </summary>
    /// <param name="position">The position to map</param>
    /// <returns>The position that's divisible by the game's pixels per unit.</returns>
    public static Vector3 ToPixel(Vector3 position) {
      
      position.x = ToPixel(position.x);
      position.y = ToPixel(position.y);
      position.z = ToPixel(position.z);

      return position;
    }

    /// <summary>
    /// Map a position to pixel-friendly coordinates.
    /// </summary>
    /// <param name="position">The value to map</param>
    /// <param name="pixelWidth">The width of a single pixel in standard Unity
    /// units. </param>
    /// <returns>The position that's divisible by the game's pixels per unit.</returns>
    public static float ToPixel(float position) {
      return (position-(position % pixelWidth));
    }

  }
}