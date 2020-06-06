using Storm.LevelMechanics.Platforms;
using Storm.Services;
using UnityEngine;

namespace Storm.Characters {

  public interface ICollisionSensor {
    /// <summary>
    /// How far the player is from the ground.
    /// </summary>
    /// <returns>The distance between the player's feet and the closest piece of ground.</returns>
    float DistanceToGround(Vector2 center, Vector2 extents, LayerMask layerMask);

    /// <summary>
    /// How far the player is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the player's left side and the closest left-hand wall.</returns>
    float DistanceToLeftWall(Vector2 center, Vector2 extents, LayerMask layerMask);

    /// <summary>
    /// How far the player is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the player's right side and the closest right-hand wall.</returns>
    float DistanceToRightWall(Vector2 center, Vector2 extents, LayerMask layerMask);

    /// <summary>
    /// How far the player is from the closest wall.
    /// </summary>
    /// <returns>The distance between the player and the closest wall.</returns>
    float DistanceToWall(Vector2 center, Vector2 extents, LayerMask layerMask);

    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    bool IsTouchingGround(Vector2 center, Vector2 size, float margin, float colliderWidth, LayerMask layerMask);

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    bool IsTouchingLeftWall(Vector2 center, Vector2 size, float margin, float colliderWidth, LayerMask layerMask);

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>

    bool IsTouchingRightWall(Vector2 center, Vector2 size, float margin, float colliderWidth, LayerMask layerMask);
  }

  public class CollisionSensor : ICollisionSensor {

    /// <summary>
    /// How far the player is from the ground.
    /// </summary>
    /// <returns>The distance between the player's feet and the closest piece of ground.</returns>
    public float DistanceToGround(Vector2 center, Vector2 extents, LayerMask layerMask) {
      Vector2 startLeft = center-new Vector2(extents.x, extents.y+0.05f);
      RaycastHit2D hitLeft = Physics2D.Raycast(startLeft, Vector2.down, float.PositiveInfinity, layerMask);

      Vector2 startRight = center-new Vector2(-extents.x, extents.y+0.05f);
      RaycastHit2D hitRight = Physics2D.Raycast(startRight, Vector2.down, float.PositiveInfinity, layerMask);

      float[] distances = {
        hitRight.collider != null ? hitRight.distance : float.PositiveInfinity,
        hitLeft.collider != null ? hitLeft.distance : float.PositiveInfinity
      };
      
      return Mathf.Min(distances);
    }

    /// <summary>
    /// How far the player is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the player's left side and the closest left-hand wall.</returns>
    public float DistanceToLeftWall(Vector2 center, Vector2 extents, LayerMask layerMask) {
      float buffer = 0.0f;
      Vector2 horizontalDistance = new Vector2(10000, 0);

      Vector2 startTopLeft = center+new Vector2(-(extents.x+buffer), extents.y);
      RaycastHit2D hitTopLeft = Physics2D.Raycast(startTopLeft, Vector2.left, float.PositiveInfinity, layerMask);

      Vector2 startBottomLeft = center+new Vector2(-(extents.x+buffer), -extents.y);
      RaycastHit2D hitBottomLeft = Physics2D.Raycast(startBottomLeft, Vector2.left, float.PositiveInfinity, layerMask);

      float[] distances = {
        hitTopLeft.collider != null ? hitTopLeft.distance : float.PositiveInfinity,
        hitBottomLeft.collider != null ? hitBottomLeft.distance : float.PositiveInfinity,
      };

      return Mathf.Min(distances);
    }

    /// <summary>
    /// How far the player is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the player's right side and the closest right-hand wall.</returns>
    public float DistanceToRightWall(Vector2 center, Vector2 extents, LayerMask layerMask) {
      float buffer = 0.0f;
      Vector2 horizontalDistance = new Vector2(10000, 0);

      Vector2 startTopRight = center+new Vector2(extents.x+buffer, extents.y);
      RaycastHit2D hitTopRight = Physics2D.Raycast(startTopRight, Vector2.right, float.PositiveInfinity, layerMask);


      Vector2 startBottomRight = center+new Vector2(extents.x+buffer, -extents.y);
      RaycastHit2D hitBottomRight = Physics2D.Raycast(startBottomRight, Vector2.right, float.PositiveInfinity, layerMask);

      float[] distances = {
        hitTopRight.collider != null ? hitTopRight.distance : float.PositiveInfinity,
        hitBottomRight.collider != null ? hitBottomRight.distance : float.PositiveInfinity,
      };

      return Mathf.Min(distances);
    }

    /// <summary>
    /// How far the player is from the closest wall.
    /// </summary>
    /// <returns>The distance between the player and the closest wall.</returns>
    public float DistanceToWall(Vector2 center, Vector2 extents, LayerMask layerMask) {
      float buffer = 0.0f;
      Vector2 horizontalDistance = new Vector2(10000, 0);

      Vector2 startTopLeft = center+new Vector2(-(extents.x+buffer), extents.y);
      RaycastHit2D hitTopLeft = Physics2D.Raycast(startTopLeft, Vector2.left, float.PositiveInfinity, layerMask);

      Vector2 startTopRight = center+new Vector2(extents.x+buffer, extents.y);
      RaycastHit2D hitTopRight = Physics2D.Raycast(startTopRight, Vector2.right, float.PositiveInfinity, layerMask);

      Vector2 startBottomLeft = center+new Vector2(-(extents.x+buffer), -extents.y);
      RaycastHit2D hitBottomLeft = Physics2D.Raycast(startBottomLeft, Vector2.left, float.PositiveInfinity, layerMask);

      Vector2 startBottomRight = center+new Vector2(extents.x+buffer, -extents.y);
      RaycastHit2D hitBottomRight = Physics2D.Raycast(startBottomRight, Vector2.right, float.PositiveInfinity, layerMask);

      float[] distances = {
        hitTopLeft.collider != null ? hitTopLeft.distance : float.PositiveInfinity,
        hitTopRight.collider != null ? hitTopRight.distance : float.PositiveInfinity,
        hitBottomLeft.collider != null ? hitBottomLeft.distance : float.PositiveInfinity,
        hitBottomRight.collider != null ? hitBottomRight.distance : float.PositiveInfinity,
      };

      return Mathf.Min(distances);
    }

    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    public bool IsTouchingGround(Vector2 center, Vector2 size, float margin, float colliderWidth, LayerMask layerMask) {
      Vector2 boxCast = size - new Vector2(margin, 0);

      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        center,
        boxCast, 
        0,
        Vector2.down, 
        colliderWidth,
        layerMask
      );

      return AnyHits(hits, Vector2.up);
    }

    /// <summary>
    /// Whether or not the player is touching a left-hand wall.
    /// </summary>
    public bool IsTouchingLeftWall(Vector2 center, Vector2 size, float margin, float colliderWidth, LayerMask layerMask) {
      Vector2 boxCast = size - new Vector2(0, margin);


      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        center, 
        boxCast, 
        0, 
        Vector2.left, 
        colliderWidth,
        layerMask
      );

      return AnyHits(hits, Vector2.right);
    }

    /// <summary>
    /// Whether or not the player is touching a right-hand wall.
    /// </summary>
    public bool IsTouchingRightWall(Vector2 center, Vector2 size, float margin, float colliderWidth, LayerMask layerMask) {
      Vector2 boxCast = size - new Vector2(0, margin); 

      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        center, 
        boxCast, 
        0, 
        Vector2.right, 
        colliderWidth,
        layerMask
      );
      
      return AnyHits(hits, Vector2.left);
    }


    /// <summary>
    /// Whether or not a list of raycast hits is in the desired direction.
    /// </summary>
    /// <param name="hits">The list of RaycastHits</param>
    /// <param name="normalDirection">The normal of the direction to check hits against.</param>
    /// <returns>Whether or not there are any ground contacts in the desired direction.</returns>
    private bool AnyHits(RaycastHit2D[] hits, Vector2 normalDirection) {
      for (int i = 0; i < hits.Length; i++) {
        if (hits[i].collider.CompareTag("Ground") && 
            (hits[i].normal.normalized == normalDirection.normalized)) {

          return true;
        }
      }

      return false;
    }
  }

}