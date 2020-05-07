using System;
using System.Linq;
using System.Collections.Generic;
using Storm.Attributes;
using Storm.Flexible;
using UnityEngine;


namespace Storm.Characters.PlayerOld {

  /// <summary>
  /// Designed to sense player collisions
  /// </summary>
  /// <remarks>
  /// This class encapsulates all logic dealing with PlayerCharacter collisions.
  /// It uses an array of raycasting information on each side of the character to determine the character's where collisions are coming from.
  /// The vast majority of the methods on this class are condition checks for various situations a character could be in.
  /// </remarks>
  public class PlayerCollisionSensor : MonoBehaviour {

    #region Sensor Settings

    /// <summary> How many raycasts per side. Recommended: 3-5 </summary>
    [Tooltip("How many raycasts per side. Recommended: 3-5")]
    public int Granularity = 3;

    /// <summary>
    /// How long each vertical raycast is
    /// </summary>
    [Tooltip("How long each vertical raycast is")]
    public float VerticalSensitivity;

    /// <summary>
    /// How long each horizontal raycast is
    /// </summary>
    [Tooltip("How long each horizontal raycast is")]
    public float HorizontalSensitivity;

    /// <summary> 
    /// Which layers to do collision tests for 
    /// </summary>
    [Tooltip("Which layers to do collision tests for")]
    public LayerMask Mask;

    /// <summary> 
    /// Whether to sense for collisions 
    /// </summary>
    [Tooltip("Whether to sense for collisions")]
    public bool Sensing = true;

    /// <summary> 
    /// The player this collision sensor is for 
    /// </summary>
    private PlayerCharacter player;

    /// <summary> 
    /// The collider this collision sensor is for 
    /// </summary>
    private BoxCollider2D playerCollider;

    #endregion

    #region Counting Properties
    //---------------------------------------------------------------------------------------------
    // Counting Properties
    //---------------------------------------------------------------------------------------------

    /// <summary> 
    /// The number of collisions on the top side of the character 
    /// </summary>
    private int topCount;

    /// <summary> 
    /// The number of collisions on the bottom side of the character 
    /// </summary>
    private int bottomCount;

    /// <summary> 
    /// The number of collisions on the left side of the character 
    /// </summary>
    private int leftCount;

    /// <summary> 
    /// The number of collisions on the right side of the character 
    /// </summary>
    private int rightCount;

    /// <summary>
    /// Whether or not the player has touched something deadly.
    /// </summary>
    private bool touchedDeadlyObject;
    #endregion

    #region Sensors
    //---------------------------------------------------------------------------------------------
    // Sensors
    //---------------------------------------------------------------------------------------------

    /// <summary> 
    /// which top side raycasts returned a collision
    /// </summary>
    private bool[] topSensors;

    /// <summary> 
    /// which bottom side raycasts returned a collision 
    /// </summary>
    private bool[] bottomSensors;

    /// <summary> 
    /// which left side raycasts returned a collision 
    /// </summary>
    private bool[] leftSensors;

    /// <summary> 
    /// which right side raycasts returned a collision 
    /// </summary>
    private bool[] rightSensors;
    #endregion

    #region Raycast Offsets
    //---------------------------------------------------------------------------------------------
    // Raycast Offsets
    //---------------------------------------------------------------------------------------------

    /// <summary> 
    /// The calculated offsets on the top side of the character 
    /// </summary>
    private Vector3[] topOffsets;

    /// <summary> 
    /// The calculated offsets on the bottom side of the character 
    /// </summary>
    private Vector3[] bottomOffsets;

    /// <summary> 
    /// The calculated offsets on the left side of the character 
    /// </summary>
    private Vector3[] leftOffsets;

    /// <summary> 
    /// The calculated offsets on the right side of the character
    ///  </summary>
    private Vector3[] rightOffsets;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    public void Start() {
      player = GetComponentInParent<PlayerCharacter>();
      playerCollider = player.GetComponent<BoxCollider2D>();
      var extents = playerCollider.bounds.extents;
      CreateOffsets(extents.x, extents.y, Granularity);
      CreateSensors(Granularity);
      ClearSensors();
    }

    /// <summary> Instantiates positional offsets for the player raycast sensors </summary>
    /// <param name="hExtent"> horizontal extent, player collider width/2 </param>
    /// <param name="vExtent"> vertical extent, player collider height/2 </param>
    private void CreateOffsets(float hExtent, float vExtent, int granularity) {
      topOffsets = new Vector3[granularity];
      bottomOffsets = new Vector3[granularity];
      leftOffsets = new Vector3[granularity];
      rightOffsets = new Vector3[granularity];


      float vOffset = (vExtent * 2) / (granularity - 1);
      float hOffset = (hExtent * 2) / (granularity - 1);

      for (int i = 0; i < granularity; i++) {
        topOffsets[i] = new Vector3((hOffset * i) - hExtent, vExtent, 0); // left-to-right
        bottomOffsets[i] = new Vector3((hOffset * i) - hExtent, -vExtent, 0); // left-to-right
        leftOffsets[i] = new Vector3(-hExtent, vExtent - (vOffset * i), 0); // top-to-bottom
        rightOffsets[i] = new Vector3(hExtent, vExtent - (vOffset * i), 0); // top-to-bottom
      }
    }

    /// <summary> 
    /// Instantiates the array of sensors 
    /// </summary>
    /// <param name="granularity"> the number of sensors on each side. </param>
    private void CreateSensors(int granularity) {
      topSensors = new bool[granularity];
      rightSensors = new bool[granularity];
      bottomSensors = new bool[granularity];
      leftSensors = new bool[granularity];
    }
    #endregion

    #region Public Interface
    //---------------------------------------------------------------------------------------------
    // Public Utility
    //---------------------------------------------------------------------------------------------

    /// <summary> 
    /// Sets whether to perform sensing
    /// </summary>
    public void SetEnableSensing(bool sensing) {
      this.Sensing = sensing;
    }

    /// <summary> 
    /// Enables sensing 
    /// </summary>
    public void EnableSensing() {
      Sensing = true;
    }

    /// <summary> 
    /// Disables sensing 
    /// </summary>
    public void DisableSensing() {
      Sensing = false;
    }

    /// <summary> 
    /// Set all sensors on all sides to false 
    /// </summary>
    public void ClearSensors() {
      ResetSensor(topSensors);
      ResetSensor(rightSensors);
      ResetSensor(bottomSensors);
      ResetSensor(leftSensors);
    }

    /// <summary> 
    /// Set all sensors on one side to false 
    /// </summary>
    /// <param name="sensor"> One side of the character's sensors. </param>
    private void ResetSensor(bool[] sensor) {
      for (int i = 0; i < Granularity; i++) {
        sensor[i] = false;
      }
    }


    /// <summary> 
    /// Sense collisions on all sides. Should be called once per update. 
    /// </summary>
    public void Sense() {
      if (!Sensing) return;
      topCount = 0;
      bottomCount = 0;
      leftCount = 0;
      rightCount = 0;
      touchedDeadlyObject = false;

      SenseSide(ref topOffsets, ref topSensors, ref topCount, ref touchedDeadlyObject, Vector3.up, VerticalSensitivity);
      SenseSide(ref bottomOffsets, ref bottomSensors, ref bottomCount, ref touchedDeadlyObject, Vector3.down, VerticalSensitivity);
      SenseSide(ref leftOffsets, ref leftSensors, ref leftCount, ref touchedDeadlyObject, Vector3.left, HorizontalSensitivity);
      SenseSide(ref rightOffsets, ref rightSensors, ref rightCount, ref touchedDeadlyObject, Vector3.right, HorizontalSensitivity);
    }


    /// <summary> Sense collisions on one side </summary>
    /// <param name="offsets"> One side's calculated offsets </param>
    /// <param name="sensors"> One side of the character's sensors </param>
    /// <param name="count"> A count of one side's collisions </param>
    /// <param name="direction"> The direction of the raycasts </param>
    /// <param name="sensitivity"> how long each raycast is </param>
    private void SenseSide(ref Vector3[] offsets, ref bool[] sensors, ref int count, ref bool deadly, Vector3 direction, float sensitivity) {
      count = 0;
      for (int i = 0; i < Granularity; i++) {
        sensors[i] = IsTouching(offsets[i], direction, sensitivity, ref deadly);
        if (sensors[i]) {
          count++;
        }
      }
    }


    /// <summary> Make one raycast collision check </summary>
    /// <param name="offset"> The offset from the player's center </param>
    /// <param name="direction"> The direction of the raycast </param>
    /// <param name="sensitivity"> how long each raycast is </param>
    /// <returns> True if the raycast returns a collision </returns>
    private bool IsTouching(Vector3 offset, Vector3 direction, float sensitivity, ref bool deadly) {

      RaycastHit2D[] hitArr = Physics2D.RaycastAll(playerCollider.bounds.center + offset, direction, sensitivity, Mask);
      var start = playerCollider.bounds.center + offset;
      var end = start + direction * sensitivity;

      List<RaycastHit2D> hits = new List<RaycastHit2D>(hitArr);

      if (hits.Count == 0) {
        Debug.DrawLine(start, end, Color.red);
      } else {
        Debug.DrawLine(start, end, Color.green);
        deadly = deadly || hits.Any(hit => hit.collider.GetComponent<Deadly>() != null);
      }
      
      return hits.Count > 0 && hits.Any(hit => hit.collider.enabled && !hit.collider.isTrigger);
    }
    #endregion

    #region Condition Checks
    //---------------------------------------------------------------------------------------------
    // Condition Checks
    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Whether or not the character is touching a deadly object.
    /// </summary>
    /// <returns> Returns true if and only if the character is touching a "Deadly" Game Object.</returns>
    public bool IsTouchingDeadlyObject() {
      return touchedDeadlyObject;
    }

    /// <summary> 
    /// Whether or not the character is touching a ceiling. 
    /// </summary>
    /// <returns> Return true if and only if the character is actually touching a ceiling </remarks>
    public bool IsTouchingCeiling() {
      return AllTouchingCeiling() || (topCount > 0 && leftCount == 0 && rightCount == 0);
    }

    /// <summary> 
    /// Whether or not the character is touching a floor.
    /// </summary>
    /// <returns> Return true if and only if the character is actually touching a floor </remarks>
    public bool IsTouchingFloor() {
      return AllTouchingFloor() || (bottomCount > 0 && leftCount == 0 && rightCount == 0);
    }

    /// <summary> 
    /// Whether or not the character is touching a right-hand wall. 
    /// </summary>
    /// <returns> Return true if and only if the character is actually touching a right-hand wall </remarks>
    public bool IsTouchingRightWall() {
      return AllTouchingRightWall() || (rightCount > 0 && topCount == 0 && bottomCount == 0);
    }

    /// <summary> 
    /// Whether or not the character is touching a left-hand wall. 
    /// </summary>
    /// <returns> Return true if and only if the character is actually touching a left-hand wall </remarks>
    public bool IsTouchingLeftWall() {
      return AllTouchingLeftWall() || (leftCount > 0 && topCount == 0 && bottomCount == 0);
    }

    /// <summary>
    /// Checks if all top sensors have fired 
    /// </summary>
    /// <returns> Returns true if and only if all sensors have fired at the top side </returns>
    public bool AllTouchingCeiling() {
      return topCount == Granularity;
    }

    /// <summary> 
    /// Checks if all bottom sensors have fired 
    /// </summary>
    /// <returns> Returns true if and only if all sensors have fired at the bottom side </returns>
    public bool AllTouchingFloor() {
      return bottomCount == Granularity;
    }

    /// <summary> 
    /// Checks if all left sensors have fired 
    /// </summary>
    /// <returns> Returns true if and only if all sensors have fired at the left side </returns>
    public bool AllTouchingLeftWall() {
      return leftCount == Granularity;
    }

    /// <summary> 
    /// Checks if all right sensors have fired 
    /// </summary>
    /// <returns> Returns true if and only if all sensors have fired at the right side </returns>
    public bool AllTouchingRightWall() {
      return rightCount == Granularity;
    }



    /// <summary> 
    /// Checks if the character is in the top left corner 
    /// </summary>
    public bool InTopLeftCorner() {
      return AllTouchingCeiling() && AllTouchingLeftWall();
    }

    /// <summary> 
    /// Checks if the character is in the top right corner 
    /// </summary>
    public bool InTopRightCorner() {
      return AllTouchingCeiling() && AllTouchingRightWall();
    }

    /// <summary> 
    /// Checks if the character is in the bottom left corner 
    /// </summary>
    public bool InBottomLeftCorner() {
      return AllTouchingFloor() && AllTouchingLeftWall();
    }

    /// <summary> 
    /// Checks if the character is in the bottom right corner 
    /// </summary>
    public bool IsBottomRightCorner() {
      return AllTouchingFloor() && AllTouchingRightWall();
    }



    /// <summary> 
    /// Checks if the character is on a slim piece of ground (ledge) against a wall
    /// </summary>
    public bool IsOnLedge() {
      return bottomCount > 1 && (AllTouchingLeftWall() || AllTouchingRightWall());
    }

    /// <summary> 
    /// Checks if the character is on a slim piece of ground (ledge) against a left wall 
    /// </summary>
    public bool IsOnLeftLedge() {
      return AllTouchingLeftWall() && bottomCount > 1;
    }

    /// <summary> 
    /// Checks if the character is on a slim piece of ground (ledge) against a right wall 
    /// </summary>
    public bool IsOnRightLedge() {
      return AllTouchingRightWall() && bottomCount > 1;
    }



    /// <summary> 
    /// Checks if player is squished vertically against two opposing objects 
    /// </summary>
    public bool IsSquishedVertically() {
      return topCount > 0 && bottomCount > 0;
    }

    /// <summary>
    /// Checks if player is squished horizontally against two opposing objects
    /// </summary>
    public bool IsSquishedHorizontally() {
      return leftCount > 0 && rightCount > 0;
    }



    /// <summary> 
    /// Check if the character is in the air. 
    /// </summary>
    public bool IsAirborn() {
      return NoneTouchingCeiling() &&
        NoneTouchingGround() &&
        NoneTouchingLeftWall() &&
        NoneTouchingRightWall();
    }

    /// <summary> 
    /// Checks if the character has no collisions on the top side of the character. 
    /// </summary>
    public bool NoneTouchingCeiling() {
      return topCount == 0;
    }

    /// <summary> 
    /// Checks if the character has no collisions on the bottom side of the character. 
    /// </summary>
    public bool NoneTouchingGround() {
      return bottomCount == 0;
    }

    /// <summary> 
    /// Checks if the character has no collisions on the left side of the character. 
    /// </summary>
    public bool NoneTouchingLeftWall() {
      return leftCount == 0;
    }

    /// <summary> 
    /// Checks if the character has no collisions on the right side of the character. 
    /// </summary>
    public bool NoneTouchingRightWall() {
      return rightCount == 0;
    }



    /// <summary> 
    /// hitting the corner of a barrier with the edge of your head 
    /// </summary>
    public bool IsBonking() {
      return IsBonkingLeft() ^ IsBonkingRight();
    }

    /// <summary> 
    /// hitting the left corner of a barrier with the edge of your head 
    /// </summary>
    public bool IsBonkingLeft() {
      return (topCount == 1 && topSensors[0]) ^
        (leftCount == 1 && leftSensors[0]);
    }

    /// <summary> 
    /// hitting the right corner of a barrier with the edge of your head 
    /// </summary>
    public bool IsBonkingRight() {
      return (topCount == 1 && topSensors[Granularity - 1]) ^
        (rightCount == 1 && rightSensors[0]);
    }



    /// <summary> 
    /// colliding with the top of a cliff just shy of landing on it. 
    /// </summary>
    public bool IsStubbing() {
      return IsStubbingLeft() ^ IsStubbingRight();
    }


    /// <summary> 
    /// colliding with the top of a left-hand cliff just shy of landing on it. 
    /// </summary>
    public bool IsStubbingLeft() {
      return (leftCount == 1 && leftSensors[Granularity - 1]) &&
        !(bottomCount == 1 && bottomSensors[0]);

    }

    /// <summary> 
    /// colliding with the top of a right-hand cliff just shy of landing on it. 
    /// </summary>
    public bool IsStubbingRight() {
      return (rightCount == 1 && leftSensors[Granularity - 1]) &&
        !(bottomCount == 1 && bottomSensors[Granularity - 1]);

    }



    /// <summary> 
    /// Character is leaning over the edge he/she's standing on. 
    /// </summary>
    public bool IsLeaning() {
      return IsLeaningLeft() || IsLeaningRight();
    }

    /// <summary>
    ///  Character is leaning over a left edge 
    /// </summary>
    public bool IsLeaningLeft() {
      return (bottomCount == 1 && bottomSensors[0]) &&
        !(leftCount == 1 && leftSensors[Granularity - 1]);
    }

    /// <summary> 
    /// Character is leaning over a right edge 
    /// </summary>
    public bool IsLeaningRight() {
      return (bottomCount == 1 && bottomSensors[Granularity - 1]) &&
        !(rightCount == 1 && leftSensors[Granularity - 1]);
    }



    /**
      Ambiguous corner cases. Only one sensor fires on two different sides,
      but in a corner at the same time.

      TODO: determine what the player should do in these cases
    */
    public bool IsTouchingTopLeftCorner() {
      return (topCount == 1 && topSensors[0]) &&
        (leftCount == 1 && leftSensors[0]);
    }

    public bool IsTouchingTopRightCorner() {
      return (topCount == 1 && topSensors[Granularity - 1]) &&
        (rightCount == 1 && rightSensors[0]);
    }

    public bool IsTouchingBottomLeftCorner() {
      return (bottomCount == 1 && bottomSensors[0]) &&
        (leftCount == 1 && leftSensors[Granularity - 1]);
    }

    public bool IsTouchingBottomRightCorner() {
      return (bottomCount == 1 && bottomSensors[Granularity - 1]) &&
        (rightCount == 1 && rightSensors[Granularity - 1]);
    }
    #endregion
  }
}