using System;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

    /// <summary>Designed to sense player collisions </summary>
    /// <remarks>
    /// This class encapsulates all logic dealing with PlayerCharacter collisions.
    /// It uses an array of raycasting information on each side of the character to determine the character's spatial state.
    /// The vast majority of the methods on this class are condition checks for various situations a character could be in.
    /// </remarks>
    public class PlayerCollisionSensor : MonoBehaviour
    {

        #region Sensor Settings
        /// <summary> Whether to sense for collisions </summary>
        public bool sensing;

        /// <summary> How many raycasts per side. Recommended: 3-5 </summary>
        public int granularity;

        /// <summary> How long each raycast is </summary>
        public float sensitivity;

        /// <summary> Which layers to do collision tests for </summary>
        public LayerMask mask;

        /// <summary> The player this collision sensor is for </summary>
        private PlayerCharacter player;

        /// <summary> The collider this collision sensor is for </summary>
        private BoxCollider2D playerCollider;

        #endregion


        #region Counting Properties
        //---------------------------------------------------------------------------------------------
        // Counting Properties
        //---------------------------------------------------------------------------------------------

        /// <summary> The number of collisions on the top side of the character </summary>
        private int topCount;

        /// <summary> The number of collisions on the bottom side of the character </summary>
        private int bottomCount;

        /// <summary> The number of collisions on the left side of the character </summary>
        private int leftCount;

        /// <summary> The number of collisions on the right side of the character </summary>
        private int rightCount;
        #endregion



        #region Sensors
        //---------------------------------------------------------------------------------------------
        // Sensors
        //---------------------------------------------------------------------------------------------

        /// <summary> which top side raycasts returned a collision </summary>
        private bool[] topSensors;

        /// <summary> which bottom side raycasts returned a collision </summary>
        private bool[] bottomSensors;

        /// <summary> which left side raycasts returned a collision </summary>
        private bool[] leftSensors;

        /// <summary> which right side raycasts returned a collision </summary>
        private bool[] rightSensors;
        #endregion



        #region Raycast Offsets
        //---------------------------------------------------------------------------------------------
        // Raycast Offsets
        //---------------------------------------------------------------------------------------------

        /// <summary> The calculated offsets on the top side of the character </summary>
        private Vector3[] topOffsets;

        /// <summary> The calculated offsets on the bottom side of the character </summary>
        private Vector3[] bottomOffsets;

        /// <summary> The calculated offsets on the left side of the character </summary>
        private Vector3[] leftOffsets;

        /// <summary> The calculated offsets on the right side of the character </summary>
        private Vector3[] rightOffsets;
        #endregion



        #region Constructor & Initialization
        /// <summary> Constructor </summary>
        /// <param name="player"> The player to create a sensor for </param>
        /// <param name="granularity"> The number of raycasts per side </param>
        /// <param name="sensitivity"> The length of each raycast </param>
        /// <param name="mask"> Which layers to test collision for </param>
        public void Start()
        {
            player = GetComponentInParent<PlayerCharacter>();
            playerCollider = player.GetComponent<BoxCollider2D>();
            var extents = playerCollider.bounds.extents;
            createOffsets(extents.x, extents.y, granularity);
            createSensors(granularity);
            clearSensors();
        }

        /// <summary> Instantiates positional offsets for the player raycast sensors </summary>
        /// <param name="hExtent"> horizontal extent, player collider width/2 </param>
        /// <param name="vExtent"> vertical extent, player collider height/2 </param>
        private void createOffsets(float hExtent, float vExtent, int granularity)
        {
            topOffsets = new Vector3[granularity];
            bottomOffsets = new Vector3[granularity];
            leftOffsets = new Vector3[granularity];
            rightOffsets = new Vector3[granularity];


            float vOffset = (vExtent * 2) / (granularity - 1);
            float hOffset = (hExtent * 2) / (granularity - 1);

            for (int i = 0; i < granularity; i++)
            {
                topOffsets[i] = new Vector3((hOffset * i) - hExtent, vExtent, 0);     // left-to-right
                bottomOffsets[i] = new Vector3((hOffset * i) - hExtent, -vExtent, 0); // left-to-right
                leftOffsets[i] = new Vector3(-hExtent, vExtent - (vOffset * i), 0);   // top-to-bottom
                rightOffsets[i] = new Vector3(hExtent, vExtent - (vOffset * i), 0);   // top-to-bottom
            }
        }

        /// <summary> Instantiates the array of sensors </summary>
        /// <param name="granularity"> the number of sensors on each side. </param>
        private void createSensors(int granularity)
        {
            topSensors = new bool[granularity];
            rightSensors = new bool[granularity];
            bottomSensors = new bool[granularity];
            leftSensors = new bool[granularity];
        }
        #endregion



        #region Basic Interface
        //---------------------------------------------------------------------------------------------
        // General Utility
        //---------------------------------------------------------------------------------------------

        /// <summary> Sets whether to perform sensing </summary>
        public void SetEnableSensing(bool sensing)
        {
            this.sensing = sensing;
        }

        /// <summary> Enables sensing </summary>
        public void EnableSensing()
        {
            sensing = true;
        }

        /// <summary> Disables sensing </summary>
        public void DisableSensing()
        {
            sensing = false;
        }

        /// <summary> Set all sensors on all sides to false </summary>
        public void clearSensors()
        {
            resetSensor(topSensors);
            resetSensor(rightSensors);
            resetSensor(bottomSensors);
            resetSensor(leftSensors);
        }

        /// <summary> Set all sensors on one side to false </summary>
        /// <param name="sensor"> One side of the character's sensors. </param>
        private void resetSensor(bool[] sensor)
        {
            for (int i = 0; i < granularity; i++)
            {
                sensor[i] = false;
            }
        }


        /// <summary> Sense collisions on all sides. Should be called once per update. </summary>
        public void sense()
        {
            if (!sensing) return;
            topCount = 0;
            bottomCount = 0;
            leftCount = 0;
            rightCount = 0;
            senseSide(ref topOffsets, ref topSensors, ref topCount, Vector3.up);
            senseSide(ref bottomOffsets, ref bottomSensors, ref bottomCount, Vector3.down);
            senseSide(ref leftOffsets, ref leftSensors, ref leftCount, Vector3.left);
            senseSide(ref rightOffsets, ref rightSensors, ref rightCount, Vector3.right);
        }


        /// <summary> Sense collisions on one side </summary>
        /// <param name="offsets"> One side's calculated offsets </param>
        /// <param name="sensors"> One side of the character's sensors </param>
        /// <param name="count"> A count of one side's collisions </param>
        /// <param name="direction"> The direction of the raycasts </param>
        private void senseSide(ref Vector3[] offsets, ref bool[] sensors, ref int count, Vector3 direction)
        {
            count = 0;
            for (int i = 0; i < granularity; i++)
            {
                sensors[i] = isTouching(offsets[i], direction);
                if (sensors[i])
                {
                    count++;
                }
            }
        }


        /// <summary> Make one raycast collision check </summary>
        /// <param name="offset"> The offset from the player's center </param>
        /// <param name="direction"> The direction of the raycast </param>
        /// <returns> True if the raycast returns a collision </returns>
        private bool isTouching(Vector3 offset, Vector3 direction)
        {
            
            var hit = Physics2D.Raycast(playerCollider.bounds.center + offset, direction, sensitivity, mask);
            var start = playerCollider.bounds.center + offset;
            var end = start + direction*sensitivity;
            if (hit.collider == null) {
                Debug.DrawLine(start, end, Color.red);
            } else {
                Debug.DrawLine(start, end, Color.green);
            }
            return hit.collider != null;
        }
        #endregion



        #region Condition Checks
        //---------------------------------------------------------------------------------------------
        // Condition Checks
        //---------------------------------------------------------------------------------------------

        /// <summary> Whether or not the character is touching a ceiling. </summary>
        /// <remarks> Supposed to return true if and only if the character is actually touching a ceiling </remarks>
        /// <returns> Return true if and only if the character is actually touching a ceiling </remarks>
        public bool isTouchingCeiling()
        {
            return allTouchingCeiling() || (topCount > 0 && leftCount == 0 && rightCount == 0);
        }

        /// <summary> Whether or not the character is touching a floor. </summary>
        /// <remarks> Supposed to return true if and only if the character is acutally touching a floor </remarks>
        /// <returns> Return true if and only if the character is actually touching a floor </remarks>
        public bool isTouchingFloor()
        {
            return allTouchingFloor() || (bottomCount > 0 && leftCount == 0 && rightCount == 0);
        }

        /// <summary> Whether or not the character is touching a right-hand wall. </summary>
        /// <remarks> Supposed to return true if and only if the character is acutally touching a right-hand wall </remarks>
        /// <returns> Return true if and only if the character is actually touching a right-hand wall </remarks>
        public bool isTouchingRightWall()
        {
            return allTouchingRightWall() || (rightCount > 0 && topCount == 0 && bottomCount == 0);
        }

        /// <summary> Whether or not the character is touching a left-hand wall. </summary>
        /// <remarks> Supposed to return true if and only if the character is acutally touching a left-hand wall </remarks>
        /// <returns> Return true if and only if the character is actually touching a left-hand wall </remarks>
        public bool isTouchingLeftWall()
        {
            return allTouchingLeftWall() || (leftCount > 0 && topCount == 0 && bottomCount == 0);
        }

        /// <summary> Checks if all top sensors have fired </summary>
        /// <returns> Returns true if and only if all sensors have fired at the top side </returns>
        public bool allTouchingCeiling()
        {
            return topCount == granularity;
        }

        /// <summary> Checks if all bottom sensors have fired </summary>
        /// <returns> Returns true if and only if all sensors have fired at the bottom side </returns>
        public bool allTouchingFloor()
        {
            return bottomCount == granularity;
        }

        /// <summary> Checks if all left sensors have fired </summary>
        /// <returns> Returns true if and only if all sensors have fired at the left side </returns>
        public bool allTouchingLeftWall()
        {
            return leftCount == granularity;
        }

        /// <summary> Checks if all right sensors have fired </summary>
        /// <returns> Returns true if and only if all sensors have fired at the right side </returns>
        public bool allTouchingRightWall()
        {
            return rightCount == granularity;
        }




        /// <summary> Checks if the character is in the top left corner </summary>
        public bool inTopLeftCorner()
        {
            return allTouchingCeiling() && allTouchingLeftWall();
        }

        /// <summary> Checks if the character is in the top right corner </summary>
        public bool inTopRightCorner()
        {
            return allTouchingCeiling() && allTouchingRightWall();
        }

        /// <summary> Checks if the character is in the bottom left corner </summary>
        public bool inBottomLeftCorner()
        {
            return allTouchingFloor() && allTouchingLeftWall();
        }

        /// <summary> Checks if the character is in the bottom right corner </summary>
        public bool isBottomRightCorner()
        {
            return allTouchingFloor() && allTouchingRightWall();
        }





        /// <summary> Checks if the character is on a slim piece of ground (ledge) against a wall </summary>
        public bool onLedge()
        {
            return bottomCount > 1 && (allTouchingLeftWall() || allTouchingRightWall());
        }

        /// <summary> Checks if the character is on a slim piece of ground (ledge) against a left wall </summary>
        public bool onLeftLedge()
        {
            return allTouchingLeftWall() && bottomCount > 1;
        }

        /// <summary> Checks if the character is on a slim piece of ground (ledge) against a right wall </summary>
        public bool onRightLedge()
        {
            return allTouchingRightWall() && bottomCount > 1;
        }





        /// <summary> Checks if player is squished vertically against two opposing objects </summary>
        public bool squishedVertically()
        {
            return topCount > 0 && bottomCount > 0;
        }

        /// <summary> Checks if player is squished horizontally against two opposing objects </summary>
        public bool squishedHorizontally()
        {
            return leftCount > 0 && rightCount > 0;
        }





        /// <summary> Check if the character is in the air. </summary>
        public bool isAirborn()
        {
            return noneTouchingCeiling() &&
                   noneTouchingGround() &&
                   noneTouchingLeftWall() &&
                   noneTouchingRightWall();
        }

        /// <summary> Checks if the character has no collisions on the top side of the character. </summary>
        public bool noneTouchingCeiling()
        {
            return topCount == 0;
        }

        /// <summary> Checks if the character has no collisions on the bottom side of the character. </summary>
        public bool noneTouchingGround()
        {
            return bottomCount == 0;
        }

        /// <summary> Checks if the character has no collisions on the left side of the character. </summary>
        public bool noneTouchingLeftWall()
        {
            return leftCount == 0;
        }

        /// <summary> Checks if the character has no collisions on the right side of the character. </summary>
        public bool noneTouchingRightWall()
        {
            return rightCount == 0;
        }





        /// <summary> hitting the corner of a barrier with the edge of your head </summary>
        public bool bonking()
        {
            return bonkingLeft() ^ bonkingRight();
        }

        /// <summary> hitting the left corner of a barrier with the edge of your head </summary>
        public bool bonkingLeft()
        {
            return (topCount == 1 && topSensors[0]) ^
                    (leftCount == 1 && leftSensors[0]);
        }

        /// <summary> hitting the right corner of a barrier with the edge of your head </summary>
        public bool bonkingRight()
        {
            return (topCount == 1 && topSensors[granularity - 1]) ^
                   (rightCount == 1 && rightSensors[0]);
        }





        /// <summary> colliding with the top of a cliff just shy of landing on it. </summary>
        public bool stubbing()
        {
            return stubbingLeft() ^ stubbingRight();
        }


        /// <summary> colliding with the top of a left-hand cliff just shy of landing on it. </summary>
        public bool stubbingLeft()
        {
            return (leftCount == 1 && leftSensors[granularity - 1]) &&
                  !(bottomCount == 1 && bottomSensors[0]);

        }

        /// <summary> colliding with the top of a right-hand cliff just shy of landing on it. </summary>
        public bool stubbingRight()
        {
            return (rightCount == 1 && leftSensors[granularity - 1]) &&
                  !(bottomCount == 1 && bottomSensors[granularity - 1]);

        }





        /// <summary> Character is leaning over the edge he/she's standing on. </summary>
        public bool leaning()
        {
            return leaningLeft() || leaningRight();
        }

        /// <summary> Character is leaning over a left edge </summary>
        public bool leaningLeft()
        {
            return (bottomCount == 1 && bottomSensors[0]) &&
                  !(leftCount == 1 && leftSensors[granularity - 1]);
        }

        /// <summary> Character is leaning over a right edge </summary>
        public bool leaningRight()
        {
            return (bottomCount == 1 && bottomSensors[granularity - 1]) &&
                  !(rightCount == 1 && leftSensors[granularity - 1]);
        }





        /**
          Ambiguous corner cases. Only one sensor fires on two different sides,
          but in a corner at the same time.

          TODO: determine what the player should do in these cases
        */
        public bool touchingTopLeftCorner()
        {
            return (topCount == 1 && topSensors[0]) &&
                   (leftCount == 1 && leftSensors[0]);
        }

        public bool touchingTopRightCorner()
        {
            return (topCount == 1 && topSensors[granularity - 1]) &&
                   (rightCount == 1 && rightSensors[0]);
        }

        public bool touchingBottomLeftCorner()
        {
            return (bottomCount == 1 && bottomSensors[0]) &&
                   (leftCount == 1 && leftSensors[granularity - 1]);
        }

        public bool touchingBottomRightCorner()
        {
            return (bottomCount == 1 && bottomSensors[granularity - 1]) &&
                   (rightCount == 1 && rightSensors[granularity - 1]);
        }
        #endregion
    }
}