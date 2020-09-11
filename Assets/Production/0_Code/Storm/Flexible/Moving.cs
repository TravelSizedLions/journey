using System.Collections.Generic;
using Storm.Attributes;
using Storm.Characters.Player;
using Storm.Subsystems.Reset;
using UnityEngine;

namespace Storm.Flexible {

  /// <summary>
  /// How a moving object travels from point to point:
  /// - Cyclical: Move from point A, to point B, to point C, back to point A.
  /// - BackAndForth: Move from point A, to point B, to point C, then reverse back to point B, then to point A.
  /// - OneTime: Move from point A, to point B, to point C, then remain stationary.
  /// </summary>
  public enum TravelStyle {

    // Move from point A, to point B, to point C, back to point A.
    Cyclical,

    // Move from point A, to point B, to point C, then reverse back to point B, then to point A.
    BackAndForth,

    // Move from point A, to point B, to point C, then remain stationary.
    OneTime
  }

  ///<summary>
  /// A flexible style class that makes any game object move from point to point.
  ///</summary>
  public class Moving : Resetting {

    #region Travel Points
    [Header("Travel Points", order = 0)]
    [Space(5, order = 1)]


    /// <summary>
    /// The points you'd like the object to visit in chronological order.
    /// </summary>
    [Tooltip("The points you'd like the object to visit in chronological order.")]
    public List<Transform> travelPoints;

    [Space(15, order = 2)]
    #endregion

    #region Travel Settings
    [Header("Travel Settings", order = 3)]
    [Space(5, order = 4)]


    /// <summary>
    /// How fast the object travels between points.
    /// </summary>
    [Tooltip("How fast the object travels between points.")]
    public float speed;

    private float currentSpeed;

    /// <summary>
    /// How fast the platform accelerates (as a percent of the desired speed).
    /// </summary>
    [Tooltip("How fast the platform accelerates as a percent of the desired speed. (ex. if speed is set to 5, then setting acceleration to 0.25 amounts to 1.25 units/tick^2 of acceleration.")]
    [Range(0, 1)]
    public float acceleration;

    /// <summary>
    /// The "uncapped velocity." If the object continued accelerating past its max speed, this is how fast it would be going.
    /// </summary>
    private float accumulatedAcceleration;


    /// <summary>
    /// How long the object should pause at each point in seconds.
    /// </summary>
    [Tooltip("How long the object should pause at each point in seconds.")]
    public float pauseTime;

    ///<summary>
    /// A counter used to pause at each travel point for pauseTime seconds.
    /// </summary>
    private float waitTimer;

    /// <summary>
    /// How the object moves from point to point. 
    /// Cyclical - Travels back to the first point after hitting the last point.
    /// BackAndForth - Travels forward through the points, then backward through the points.
    /// OneTime - Travels forward through the points only once.
    /// </summary>
    [Tooltip("How the object moves from point to point. Cyclical - Travels straight back to the first point after hitting the last point. BackAndForth - Travels forward through the points, then backward through the points. OneTime - Travels forward through the points only once.")]
    public TravelStyle travelStyle;

    [Space(15, order = 5)]
    #endregion

    #region Destination information
    [Header("Destination Information", order = 6)]
    [Space(5, order = 7)]
    /// <summary>
    /// The current point the object is heading towards.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Transform currentPoint;

    /// <summary>
    /// The last point the object was at.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Transform previousPoint;

    /// <summary>
    /// The index of the current point the object is heading towards.
    /// </summary>
    private int currentPointIndex;

    [Space(15, order = 8)]
    #endregion

    #region Movement Flags
    [Header("Movement Flags", order = 9)]
    [Space(5, order = 10)]

    /// <summary>
    /// Starts the object moving when the player stands on it.
    /// </summary>
    [SerializeField]
    public bool startMoving;

    /// <summary>
    /// Whether or not the object is finished moving (only applies to OneTime moving objects).
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private bool isDoneMoving;

    /// <summary>
    /// Whether or not the object is moving backwards through the point point list
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private bool isMovingBackwards;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Called once, the first time the object is enabled.
    /// </summary>
    private void Start() {
      // Start the cycle at the first point in the list.
      ResetValues();
      acceleration = acceleration * speed;
    }

    /// <summary>
    /// Called once every Time.FixedDeltaTime seconds.
    /// </summary>
    private void FixedUpdate() {
      waitTimer -= Time.fixedDeltaTime;
      if (startMoving && !isDoneMoving && !IsWaiting()) {

        transform.position = CalculatePosition(
          previousPoint.position,
          transform.position,
          currentPoint.position
        );

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
    /// Calculate the object's next position, factoring in any easing. See the following for an idea of the
    /// acceleration/deceleration equations going on here: https://www.desmos.com/calculator/5mzgdhojxk
    /// </summary>
    /// <param name="p0">The starting position of the object</param>
    /// <param name="p1">The current position of the object</param>
    /// <param name="p2">The ending position of the object</param>
    /// <returns>The next position of the object.</returns>
    private Vector2 CalculatePosition(Vector2 p0, Vector2 p1, Vector2 p2) {
      Vector2 distTotal = p2 - p0;
      Vector2 distCovered = p1 - p0;

      // don't divide by 0.
      if (distTotal.magnitude == 0) return p1;

      float percentCovered = distCovered.magnitude / distTotal.magnitude;

      // Make acceleration buttery smoooooooth.
      percentCovered = EaseInOut(percentCovered);

      // negative linear slope from acceleration to -acceleration.
      // Allows object to slow down as it approaches its destination without really
      // having to "predict" how close it is.
      float actualAcceleration = acceleration - 2 * (acceleration * percentCovered);

      // Think of this as "uncapped velocity."
      accumulatedAcceleration += actualAcceleration;

      // Only go as fast as the top speed, but don't suddenly stop or go in reverse!
      currentSpeed = Mathf.Max(Mathf.Min(accumulatedAcceleration, speed), 1e-2f);

      Vector2 direction = distTotal.normalized;
      Vector2 velocity = direction * currentSpeed;

      Vector2 newPos = p1 + velocity;

      return newPos;
    }

    /// <summary>
    /// Piecewise quadratic easing. See the following for a visualization: https://www.desmos.com/calculator/n46mhrri9g
    /// </summary>
    /// <param name="t">The value to ease in or out. Should be between 0 and 1. </param>
    /// <returns>The eased value.</returns>
    private float EaseInOut(float t) {
      return (t < 0.5f) ? (2 * t * t) : (-1 + (4 - 2 * t) * t);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
      if (collision.collider.CompareTag("Player") && !startMoving) {
        
        PlayerCharacter player = collision.collider.GetComponent<PlayerCharacter>();
        // PlayerCharacterOld player = collision.collider.GetComponent<PlayerCharacterOld>();

        // Make sure the object doesn't take off without the player on board!
        if (player.IsTouchingGround()) {
          startMoving = true;
        }
      }
    }

    #endregion

    #region Resetting API
    //-------------------------------------------------------------------------
    // Resetting API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Resets the object back to the beginning of the sequence.
    /// </summary>
    public override void ResetValues() {
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

    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Whether or not the object is stopped/waiting at a current point.
    /// </summary>
    /// <returns>True if the object hasn't waited <see cref="pauseTime" /> seconds at the current location yet. False otherwise. </returns>
    public bool IsWaiting() {
      return waitTimer >= 0;
    }

    /// <summary>
    /// Whether or not the object has arrived at the current travel point.
    /// </summary>
    /// <returns>True if the object has traveled to (or past) the destination. False otherwise.</returns>
    public bool HasArrivedAtPoint() {
      Vector2 total = currentPoint.position - previousPoint.position;
      Vector2 current = transform.position - previousPoint.position;
      return total.magnitude <= current.magnitude;
    }

    /// <summary>
    /// Gets the next point to travel to depending on the travel style.
    /// </summary>
    /// <returns>The transform of the next destination.</returns>
    public Transform GetNextTravelPoint() {
      switch (travelStyle) {
        case TravelStyle.Cyclical:
          currentPointIndex = (currentPointIndex + 1) % travelPoints.Count;
          break;
        case TravelStyle.BackAndForth:
          currentPointIndex += (isMovingBackwards ? -1 : 1);
          if (currentPointIndex == 0) {
            isMovingBackwards = false;
          } else if (currentPointIndex == travelPoints.Count - 1) {
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
    #endregion
  }

}