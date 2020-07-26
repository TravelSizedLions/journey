using System.Collections.Generic;
using UnityEngine;

namespace Storm.Components {

  #region Interface
  public interface ICollision {
    /// <summary>
    /// How far the object is from the ground.
    /// </summary>
    /// <returns>The distance between the object's feet and the closest piece of ground.</returns>
    float DistanceToGround(Vector2 center, Vector2 extents);

    /// <summary>
    /// How far the object is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the object's left side and the closest left-hand wall.</returns>
    float DistanceToLeftWall(Vector2 center, Vector2 extents);

    /// <summary>
    /// How far the object is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the object's right side and the closest right-hand wall.</returns>
    float DistanceToRightWall(Vector2 center, Vector2 extents);

    /// <summary>
    /// How far the object is from the closest wall.
    /// </summary>
    /// <returns>The distance between the object and the closest wall.</returns>
    float DistanceToWall(Vector2 center, Vector2 extents);

    /// <summary>
    /// How far the object is from the closest ceiling.
    /// </summary>
    /// <seealso cref="CollisionComponent.DistanceToCeiling" />
    /// <returns>The distance between the object and the closest ceiling.</returns>
    float DistanceToCeiling(Vector2 center, Vector2 size);

    /// <summary>
    /// Whether or not the object is touching the ground.
    /// </summary>
    /// <seealso cref="CollisionComponent.IsTouchingGround" />
    bool IsTouchingGround(Vector2 center, Vector2 size);

    /// <summary>
    /// Whether or not the object is touching a left-hand wall.
    /// </summary>
    /// <seealso cref="CollisionComponent.IsTouchingLeftWall" />
    bool IsTouchingLeftWall(Vector2 center, Vector2 size);

    /// <summary>
    /// Whether or not the object is touching a right-hand wall.
    /// </summary>
    /// <seealso cref="CollisionComponent.IsTouchingRightWall" />
    bool IsTouchingRightWall(Vector2 center, Vector2 size);

    
    /// <summary>
    /// Whether or not the object is touching the ceiling.
    /// </summary>
    /// <seealso cref="CollisionComponent.IsTouchingCeiling" />
    bool IsTouchingCeiling(Vector2 center, Vector2 size);
  }
  #endregion

  public class CollisionComponent : ICollision {
    #region Fields

    /// <summary>
    /// How thick overlap boxes should be when checking for collision direction.
    /// </summary>
    private float colliderWidth = 0.25f;

    /// <summary>
    /// The vertical & horizontal difference between the player's collider and the box cast.
    /// </summary>
    private float boxCastMargin = 0f;

    /// <summary>
    /// Layer mask that prevents collisions with anything aside from things on the ground layer.
    /// </summary>
    private LayerMask layerMask;

    /// <summary>
    /// The collider this collision component is attached to.
    /// </summary>
    private Collider2D parentCollider;
    #endregion


    #region Constructors
    public CollisionComponent() {
      layerMask = LayerMask.GetMask("Foreground");
    }

    public CollisionComponent(Collider2D parent) : this() {
      parentCollider = parent;
    }

    public CollisionComponent(float colliderWidth, float boxCastMargin, string[] layerNames) {
      this.colliderWidth = colliderWidth;
      this.boxCastMargin = boxCastMargin;
      layerMask = LayerMask.GetMask(layerNames);
    }

    public CollisionComponent(string[] layerNames) {
      layerMask = LayerMask.GetMask(layerNames);
    }

    #endregion

    #region Distance Checking
    /// <summary>
    /// How far the player is from the ground.
    /// </summary>
    /// <returns>The distance between the player's feet and the closest piece of ground.</returns>
    public float DistanceToGround(Vector2 center, Vector2 extents) {
      Vector2 startLeft = center-new Vector2(extents.x, extents.y+0.05f);
      RaycastHit2D hitLeft = Physics2D.Raycast(startLeft, Vector2.down, float.PositiveInfinity, layerMask);

      Vector2 startRight = center-new Vector2(-extents.x, extents.y+0.05f);
      RaycastHit2D hitRight = Physics2D.Raycast(startRight, Vector2.down, float.PositiveInfinity, layerMask);

      float[] distances = { float.PositiveInfinity, float.PositiveInfinity };
      if (IsHit(hitRight.collider, hitRight.normal, Vector2.up)) {
        distances[0] = hitRight.distance;
      }

      if (IsHit(hitLeft.collider, hitLeft.normal, Vector2.up)) {
        distances[1] = hitRight.distance;
      }
      
      return Mathf.Min(distances);
    }

    /// <summary>
    /// How far the player is from a left-hand wall.
    /// </summary>
    /// <returns>The distance between the player's left side and the closest left-hand wall.</returns>
    public float DistanceToLeftWall(Vector2 center, Vector2 extents) {
      float buffer = 0.0f;
      Vector2 horizontalDistance = new Vector2(10000, 0);

      Vector2 startTopLeft = center+new Vector2(-(extents.x+buffer), extents.y);
      RaycastHit2D hitTopLeft = Physics2D.Raycast(startTopLeft, Vector2.left, float.PositiveInfinity, layerMask);

      Vector2 startBottomLeft = center+new Vector2(-(extents.x+buffer), -extents.y);
      RaycastHit2D hitBottomLeft = Physics2D.Raycast(startBottomLeft, Vector2.left, float.PositiveInfinity, layerMask);


      float[] distances = { float.PositiveInfinity, float.PositiveInfinity };
      if (IsHit(hitTopLeft.collider, hitTopLeft.normal, Vector2.right)) {
        Debug.Log("A");
        distances[0] = hitTopLeft.distance;
      }

      if (IsHit(hitBottomLeft.collider, hitBottomLeft.normal, Vector2.right)) {
        Debug.Log("B");
        distances[1] = hitBottomLeft.distance;
      }

      return Mathf.Min(distances);
    }

    /// <summary>
    /// How far the player is from a right-hand wall.
    /// </summary>
    /// <returns>The distance between the player's right side and the closest right-hand wall.</returns>
    public float DistanceToRightWall(Vector2 center, Vector2 extents) {
      float buffer = 0.0f;
      Vector2 horizontalDistance = new Vector2(10000, 0);

      Vector2 startTopRight = center+new Vector2(extents.x+buffer, extents.y);
      RaycastHit2D hitTopRight = Physics2D.Raycast(startTopRight, Vector2.right, float.PositiveInfinity, layerMask);


      Vector2 startBottomRight = center+new Vector2(extents.x+buffer, -extents.y);
      RaycastHit2D hitBottomRight = Physics2D.Raycast(startBottomRight, Vector2.right, float.PositiveInfinity, layerMask);

      float[] distances = { float.PositiveInfinity, float.PositiveInfinity };
      if (IsHit(hitTopRight.collider, hitTopRight.normal, Vector2.left)) {
        Debug.Log("C");
        distances[0] = hitTopRight.distance;
      }

      if (IsHit(hitBottomRight.collider, hitBottomRight.normal, Vector2.left)) {
        Debug.Log("D");
        distances[1] = hitBottomRight.distance;
      }

      return Mathf.Min(distances);
    }

    /// <summary>
    /// How far the player is from the closest wall.
    /// </summary>
    /// <returns>The distance between the player and the closest wall.</returns>
    public float DistanceToWall(Vector2 center, Vector2 extents) {
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
        float.PositiveInfinity, 
        float.PositiveInfinity, 
        float.PositiveInfinity, 
        float.PositiveInfinity 
      };
      
      if (IsHit(hitTopLeft.collider, hitTopLeft.normal, Vector2.right)) {
        distances[0] = hitTopLeft.distance;
      }

      if (IsHit(hitTopRight.collider, hitTopRight.normal, Vector2.right)) {
        distances[1] = hitTopRight.distance;
      }

      if (IsHit(hitBottomLeft.collider, hitBottomLeft.normal, Vector2.left)) {
        distances[2] = hitBottomLeft.distance;
      }

      if (IsHit(hitBottomRight.collider, hitBottomRight.normal, Vector2.left)) {
        distances[3] = hitBottomRight.distance;
      }

      return Mathf.Min(distances);
    }


    
    /// <summary>
    /// How far the object is from the closest ceiling.
    /// </summary>
    /// <returns>The distance between the object and the closest ceiling.</returns>
    public float DistanceToCeiling(Vector2 center, Vector2 extents) {
      float buffer = 0.0f;
      Vector2 horizontalDistance = new Vector2(10000, 0);

      Vector2 startTopLeft = center+new Vector2(-(extents.x+buffer), extents.y);
      RaycastHit2D hitTopLeft = Physics2D.Raycast(startTopLeft, Vector2.up, float.PositiveInfinity, layerMask);

      Vector2 startTopRight = center+new Vector2(extents.x+buffer, extents.y);
      RaycastHit2D hitTopRight = Physics2D.Raycast(startTopRight, Vector2.up, float.PositiveInfinity, layerMask);

      float[] distances = { 
        float.PositiveInfinity, 
        float.PositiveInfinity, 
      };
      
      if (IsHit(hitTopLeft.collider, hitTopLeft.normal, Vector2.down)) {
        distances[0] = hitTopLeft.distance;
      }

      if (IsHit(hitTopRight.collider, hitTopRight.normal, Vector2.down)) {
        distances[1] = hitTopRight.distance;
      }

      float min = Mathf.Min(distances);

      Debug.DrawRay(center+new Vector2(0, extents.y), Vector2.up*min, Color.red, 0.5f);
      return min;
    }
    #endregion

    #region Collision Checking
    /// <summary>
    /// Whether or not the player is touching the ground.
    /// </summary>
    public bool IsTouchingGround(Vector2 center, Vector2 size) {
      Vector2 boxCast = size - new Vector2(boxCastMargin, 0);

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
    public bool IsTouchingLeftWall(Vector2 center, Vector2 size) {
      Vector2 boxCast = size - new Vector2(0, boxCastMargin);


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
    public bool IsTouchingRightWall(Vector2 center, Vector2 size) {
      Vector2 boxCast = size - new Vector2(0, boxCastMargin); 


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
    /// Whether or not the object is touching the ceiling.
    /// </summary>
    public bool IsTouchingCeiling(Vector2 center, Vector2 size) {
      Vector2 boxCast = size - new Vector2(0, boxCastMargin); 

      RaycastHit2D[] hits = Physics2D.BoxCastAll(
        center, 
        boxCast, 
        0, 
        Vector2.up, 
        colliderWidth,
        layerMask
      );

      return AnyHits(hits, Vector2.down);
    }


    /// <summary>
    /// Whether or not a list of raycast hits is in the desired direction.
    /// </summary>
    /// <param name="hits">The list of RaycastHits</param>
    /// <param name="normalDirection">The normal of the direction to check hits against.</param>
    /// <returns>Whether or not there are any ground contacts in the desired direction.</returns>
    public bool AnyHits(RaycastHit2D[] hits, Vector2 normalDirection) {
      for (int i = 0; i < hits.Length; i++) {
        if (IsHit(hits[i].collider, hits[i].normal, normalDirection)) {
          return true;
        }
      }

      return false;
    }


    public bool IsHit(Collider2D collider, Vector2 hitNormal, Vector2 checkNormal) {
      if (collider == null) {
        return false;
      }

      return (parentCollider == null || !Physics2D.GetIgnoreCollision(collider, parentCollider)) &&     // Collision isn't purposefully ignored,
             collider.CompareTag("Ground") &&                                                           // Collider is a ground object,
             !collider.isTrigger &&                                                                     // Collider isn't a triggers,
             hitNormal.normalized == checkNormal.normalized;                                            // & Collision is in the right direction.
    }
    #endregion


    #region Getters/Setters

    public void SetLayerMask(string[] layerNames) {
      layerMask = LayerMask.GetMask(layerNames);
    }

    public void SetBoxCastMargin(float margin) {
      boxCastMargin = margin;
    }

    public void SetColliderWidth(float width) {
      colliderWidth = width;
    }

    #endregion
  }

}