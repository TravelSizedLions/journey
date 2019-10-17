using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Characters.Player;
using Storm.TransitionSystem;

namespace Storm.Cameras {
    public class TargettingCamera : MonoBehaviour
    {
        #region Offset Parameters
        [Header("Offsets", order=0)]
        [Space(5, order=1)]

        /// <summary>
        /// How much the camera should be offset from the player vertically.
        /// </summary>
        [Tooltip("How much the camera should be offset from the player vertically.")]
        public float verticalOffset;

        /// <summary>
        /// How much the camera should be offset from the player horizontally.
        /// </summary>
        [Tooltip("How much the camera should be offset from the player horizontally.")]
        public float horizontalOffset;

        /// <summary>
        /// The calculated offset vector when centering on a target.
        /// </summary>
        private Vector3 centeredOffset;

        /// <summary>
        /// The calculated offset vector when offsetting to the left of a target.
        /// </summary>
        private Vector3 leftOffset;

        /// <summary>
        /// The calculated offset vector when offsetting to the right of a target.
        /// </summary>
        private Vector3 rightOffset;

        [Space(15, order=2)]
        #endregion


        #region Targetting Speed Parameters
        [Header("Targetting Speed", order=3)]
        [Space(5, order=4)]

        /// <summary>
        /// How quickly to pan the camera to a vCam target.
        /// </summary>
        [Tooltip("How quickly to pan the camera to a vCam target. 0 - No panning, 1 - Instantaneous")]
        [Range(0,1)]
        public float vCamPanSpeed;

        /// <summary>
        /// How quickly to pan the camera to the player if their the target.
        /// </summary>
        [Tooltip("How quickly to pan the camera to the player if their the target. 0 - No panning, 1 - Instantaneous")]
        [Range(0,1)]
        public float playerPanSpeed;

        /// <summary>
        /// How quickly to zoom the camera in and out.
        /// </summary>
        [Tooltip("How quickly to zoom the camera in and out. 0 - No panning, 1 - Instantaneous")]
        [Range(0,1)]
        public float zoomSpeed;

        #endregion


        #region Static Variables

        /// <summary>
        /// The transform that the Camera is actually tracking.
        /// </summary>
        public static Transform target;

        /// <summary>
        /// The name of a virtual camera to snap to at the beginning of a scene, if applicable.
        /// </summary>
        private static string virtualCameraName;

        /// <summary>
        /// A reference to the player
        /// </summary>
        public static PlayerCharacter player;

        /// <summary>
        /// Whether or not the camera should center on the target instead of offsetting.
        /// </summary>
        private static bool isCentered;

        /// <summary>
        /// The target's camera settings, such as orthographic size (zoom).
        /// </summary>
        private static Camera targetSettings;

        /// <summary>
        /// The camera settings that the targetting camera defaults to if not given a virtual camera to target.
        /// </summary>
        private static Camera defaultSettings;

        /// <summary>
        /// A reference parameter used by SmoothDamp in the FixedUpdate function
        /// </summary>
        private Vector3 velocity;

        #endregion

        void Start() {
            centeredOffset = new Vector3(0, 0, -10);
            leftOffset = new Vector3(-horizontalOffset, verticalOffset, -10);
            rightOffset = new Vector3(-horizontalOffset, verticalOffset, -10);


            Debug.Log("Camera Startup");
            defaultSettings = GetComponent<Camera>();
            if (player == null) {
                player = FindObjectOfType<PlayerCharacter>();
                if (player == null) return;
            }

            if (virtualCameraName != null && virtualCameraName != "") {

                // Try to find the correct virtual camera view and have the Trailing Camera snap to it.
                foreach (VirtualCamera cam in FindObjectsOfType<VirtualCamera>()) {
                    if (cam.name == virtualCameraName) {
                        // Make this virtual camera the active camera view.
                        cam.Activate();
                        SnapToTarget();
                        return;
                    }
                }

                throw new UnityException("Could not find Virtual Camera \"" + virtualCameraName + "\" in the current scene.");
            } else {
                ClearTarget();
                SnapToSpawn();
            }
        }

        void FixedUpdate() {
            if (target == null) {
                return;
            }

            if (player == null) {
                player = FindObjectOfType<PlayerCharacter>();
                if (player == null) return;
            }

            
            float futureSize = targetSettings.orthographicSize;
            float smoothing = (target == player.transform) ? playerPanSpeed : vCamPanSpeed;
            Vector3 futurePos = getFuturePosition();
            
            // interpolate camera zoom
            Camera.main.orthographicSize = interpolate(Camera.main.orthographicSize, targetSettings.orthographicSize, zoomSpeed);
            
            // interpolate camera position
            transform.position = Vector3.SmoothDamp(transform.position, futurePos, ref velocity, smoothing);
        }


        private Vector3 getFuturePosition() {
            Vector3 pos;

            // if following the player
            if (target == player.transform) {
                pos = player.transform.position;

                // choose appropriate camera offset.
                if (isCentered) {
                    pos += centeredOffset;
                } else if (player.normalMovement.isFacingRight) {
                    pos += rightOffset;
                } else if (!player.normalMovement.isFacingRight) {
                    pos += leftOffset;
                }
                
            // moving to a predefined location
            } else {
                pos = target.transform.position + centeredOffset;
            }

            return pos;
        }


        private float interpolate(float x, float y, float a) {
            return x*a + y*(1-a);
        }


        public void SetPlayerSmoothing(float smoothing) {
            playerPanSpeed = smoothing;
        }

        public void SetTargetSmoothing(float smoothing) {
            vCamPanSpeed = smoothing;
        }

        public static void SetTarget(string vcamName) {
            virtualCameraName = vcamName;
        }

        public static void SetTarget(Camera cameraSettings) {
            //Debug.Log("Setting Target!");
            targetSettings = cameraSettings;
            isCentered = true;
            target = cameraSettings.transform;
        }

        public static void ClearTarget() {
            //Debug.Log("Clearing Target!");
            targetSettings = defaultSettings;
            target = player.transform;
            isCentered = false;
        }

        public void SnapToTarget() {
            transform.position = target.position;
            if (target == player.transform) {
                if (player.normalMovement.isFacingRight) {
                    transform.position += rightOffset;
                } else {
                    transform.position += leftOffset;
                }
            } else if (isCentered) {
                transform.position += centeredOffset;
            }
        }

        public void SnapToSpawn() {
            Vector3 pos = TransitionManager.Instance.GetCurrentSpawnPosition();
            bool isFacingRight = TransitionManager.Instance.GetCurrentSpawnFacing();

            Debug.Log("Facing " + (isFacingRight ? "Right!" : "Left!"));

            transform.position = pos + (isFacingRight ? rightOffset : leftOffset);
        }

        public void SetCentered(bool centered) {
            isCentered = centered;
        }
    }

}
