using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;
using Storm.Attributes;
using Storm.ResetSystem;

namespace Storm.Flexible {
  
  public enum TravelStyle {
    Cyclical,
    BackAndForth,
    OneTime
  }
  
  ///<summary>
  /// A flexible style class that makes a game object move from point to point.
  ///</summary>
  public class Moving : Resetting {
    
    #region Travel Points
    [Header("Travel Points", order=0)]
    [Space(5, order=1)]
    
    
    ///<summary>The points you'd like the object to visit in chronological order.</summary>
    [Tooltip("The points you'd like the object to visit in chronological order.")]
    public List<Transform> travelPoints;
    
    [Space(15, order=2)]
    #endregion
    
    
    #region Travel Settings
    [Header("Travel Settings", order=3)]
    [Space(5, order=4)]
    
    
    ///<summary>How fast the object travels between points.</summary>
    [Tooltip("How fast the object travels between points.")]
    public float speed;

    private float currentSpeed;

    /// <summary>How fast the platform accelerates (as a percent of the desired speed).</summary>
    [Tooltip("How fast the platform accelerates as a percent of the desired speed - in units per physics tick^2.")]
    [Range(0,1)]
    public float acceleration;

    /// <summary>Used to determine when the object should begin decelerating.false </summary>
    private float accumulatedAcceleration;
    
    
    ///<summary>How long the object should pause at each point in seconds.</summary>
    [Tooltip("How long the object should pause at each point in senconds.")]
    public float pauseTime;
    
    /// <summary>
    /// How the object moves from point to point. 
    /// Cyclical - Travels straight back to the first point after hitting the last point.
    /// BackAndForth - Travels forward through the points, then backward through the points.
    /// OneTime - Travels forward through the points only once.
    /// </summary>
    [Tooltip("")]
    public TravelStyle travelStyle;
    #endregion
    
    ///<summary>The current point the object is heading towards.</summary>
    [SerializeField]
    [ReadOnly]
    private Transform currentPoint;
    
    ///<summary>The last point the object was.</summary>
    [SerializeField]
    [ReadOnly]
    private Transform previousPoint;
    
    ///<summary>The index of the current point the object is heading towards.</summary>
    private int currentPointIndex;
    
    ///<summary>A counter used to pause at each travel point for pauseTime seconds.</summary>
    private float waitTimer;
    
    ///<summary>Starts the object moving when the player stands on it.</summary>
    [SerializeField]
    public bool startMoving;

    ///<summary>Whether or not the object is finished moving (only applies to OneTime moving objects).</summary>
    [SerializeField]
    [ReadOnly]
    private bool isDoneMoving;
    
    ///<summary>Whether or not the object is moving backwards through the point point list</summary>
    [SerializeField]
    [ReadOnly]
    private bool isMovingBackwards;
    
    ///<summary>Called once, the first time the object is enabled.</summary>
    public void Start() {
      // Start the cycle at the first point in the list.
      Reset();
      acceleration = acceleration*speed;
    }
    
    ///<summary>Called once every Time.FixedDeltaTime seconds.</summary>
    public void FixedUpdate() {
      if (startMoving && !isDoneMoving && !IsWaiting()) {
        
        UpdateTransform();

        if (HasArrivedAtPoint()) {
          // Don't speed up to infinity, please!
          accumulatedAcceleration = 0;

          previousPoint = currentPoint;
          currentPoint = GetNextTravelPoint();
          waitTimer = pauseTime;
        }
      }
    }
    
    /// <summary>
    /// Calculate the object's next position, factoring in any easing.
    /// </summary>
    /// <param name="p0">The starting position of the object</param>
    /// <param name="p1">The current position of the object</param>
    /// <param name="p2">The ending position of the object</param>
    /// <returns>The next position of the object.</returns>
    public Vector2 CalculatePosition(Vector2 p0, Vector2 p1, Vector2 p2) {
      Vector2 distTotal = p2-p0;
      Vector2 distCovered = p1-p0;

      // don't divide by 0.
      if (distTotal.magnitude == 0) return p1;

      float percentCovered = distCovered.magnitude/distTotal.magnitude;

      // Make acceleration buttery smoooooooth.
      percentCovered = EaseInOut(percentCovered);

      // negative linear slope from acceleration to -acceleration.
      // Allows object to slow down as it approaches its destination without really
      // having to "predict" how close it is.
      float actualAcceleration = acceleration - 2*(acceleration*percentCovered);

      // Think of this as "uncapped velocity."
      accumulatedAcceleration += actualAcceleration;

      // Only go as fast as the top speed, but don't suddenly stop or go in reverse!
      currentSpeed = Mathf.Max(Mathf.Min(accumulatedAcceleration, speed), 1e-2f);

      Vector2 direction = distTotal.normalized;
      Vector2 velocity = direction*currentSpeed;

      Vector2 newPos = p1+velocity;

      return newPos;
    }

    public void UpdateTransform() {
      Vector3 distTotal = currentPoint.position - previousPoint.position;
      Vector3 distCovered = transform.position - previousPoint.position;
      Vector3 rotTotal = currentPoint.rotation.eulerAngles - previousPoint.rotation.eulerAngles;

      // don't divide by zero.
      if (distTotal.sqrMagnitude == 0) return;

      float percentCovered = distCovered.magnitude/distTotal.magnitude;

      // Make acceleration buttery smoooooooth.
      percentCovered = EaseInOut(percentCovered);

      // negative linear slope from acceleration to -acceleration.
      // Allows object to slow down as it approaches its destination without really
      // having to "predict" how close it is.
      float actualAcceleration = acceleration - 2*(acceleration*percentCovered);

      // Think of this as "uncapped velocity."
      accumulatedAcceleration += actualAcceleration;

      // Only go as fast as the top speed, but don't suddenly stop or go in reverse!
      currentSpeed = Mathf.Max(Mathf.Min(accumulatedAcceleration, speed), 1e-2f);

      // Update position.
      Vector3 direction = distTotal.normalized;
      Vector3 velocity = direction*currentSpeed;
      transform.position += velocity;

      // Update rotation.
      Vector3 newRot = previousPoint.rotation.eulerAngles + rotTotal*percentCovered;
      transform.rotation = Quaternion.Euler(newRot);
    }
    
    public float EaseInOut(float t) {
      return (t < 0.5f) ? (2*t*t) : (-1+(4-2*t)*t);
    }

    ///<summary>Whether or not the object is stopped/waiting at a current point.</summary>
    public bool IsWaiting() {
      waitTimer -= Time.fixedDeltaTime;
      return waitTimer >= 0;
    }
    
    ///<summary>Whether or not the object has arrived at the current travel point.</summary>
    public bool HasArrivedAtPoint() {
      Vector2 total = currentPoint.position - previousPoint.position;
      Vector2 current = transform.position - previousPoint.position;
      return total.magnitude <= current.magnitude;
    }
    
    ///<summary>Gets the next point to travel to depending on the travel style.</summary>
    public Transform GetNextTravelPoint() {
      switch(travelStyle) {
        case TravelStyle.Cyclical:
          currentPointIndex = (currentPointIndex+1)%travelPoints.Count;
          break;
        case TravelStyle.BackAndForth:
          currentPointIndex += (isMovingBackwards ? -1 : 1);
          if (currentPointIndex == 0) {
            isMovingBackwards = false;
          } else if (currentPointIndex == travelPoints.Count-1) {
            isMovingBackwards = true;
          }
          break;
        case TravelStyle.OneTime:
          currentPointIndex += 1;
          if (currentPointIndex == travelPoints.Count) {
            currentPointIndex -= 1;
            isDoneMoving = true;
          }
          break;
      }
      return travelPoints[currentPointIndex];
    }
    
    ///<summary>Resets the object back to the beginning of the sequence.</summary>
    public override void Reset() {
      isDoneMoving = false;
      isMovingBackwards = false;
      if (travelStyle != TravelStyle.OneTime) {
        startMoving = true;
      } else {
        startMoving = false;
      }
      waitTimer = 0;
      currentPointIndex = 0;
      currentPoint = travelPoints[currentPointIndex];
      previousPoint = currentPoint;
      transform.position = currentPoint.transform.position;
    }
    

    public void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player") && !startMoving) {
        PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();
        
        // Make sure the object doesn't take off without the player on board!
        if (travelStyle == TravelStyle.OneTime && player.sensor.isTouchingFloor()) {
          startMoving = true;
        }
      }
    }

  }

}