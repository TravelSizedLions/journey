using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Cameras {
    
    /// <summary>
    /// A virtual camera used to frame sections of a level. 
    /// Makes the camera "stick" to certain locations if the player walks into the appropriate area.
    /// </summary>
    public class VirtualCamera : MonoBehaviour {

        /// <summary>
        /// The camera settings that should be applied to the TargettingCamera when this vCam is activated.
        /// </summary>
        private Camera cameraSettings;

        /// <summary>
        /// A reference to the TargettingCamera.
        /// </summary>
        private static TargettingCamera cam;

        //---------------------------------------------------------------------
        // Unity API
        //---------------------------------------------------------------------

        /// <summary>
        /// Grabs references to the vCam's settings and to the TargettingCamera
        /// </summary>
        private void Awake(){
            cameraSettings = GetComponent<Camera>();
            if (cameraSettings == null) {
                cameraSettings = transform.parent.GetComponentInChildren<Camera>();
            }

            if (cam == null) {
                cam = FindObjectOfType<TargettingCamera>();
            }
        }


        /// <summary>
        /// Activate the vCam if the player moves into it's trigger collider.
        /// </summary>
        /// <param name="col">The collider that's intersecting the vCam collider</param>
        private void OnTriggerEnter2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                Activate();
            }
        }

        /// <summary>
        /// Activate the vCam the player is standing completely within over a partial collision.
        /// </summary>
        /// <param name="col">The collider that's intersecting the vCam collider</param>
        private void OnTriggerStay2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                if (TargettingCamera.target.transform.position != cameraSettings.transform.position) {
                    GameManager.Instance.resets.Reset();
                    Activate();
                }
            }
        }


        /// <summary>
        /// Deactivate the vCam if the player leaves its trigger collider.
        /// </summary>
        /// <param name="col">The collider that's intersecting the vCam collider</param>
        private void OnTriggerExit2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                // In case 2 zones overlap
                if (TargettingCamera.target == cameraSettings.transform) {
                    Deactivate();
                }
            }
        }



        //---------------------------------------------------------------------
        // Public Interface
        //---------------------------------------------------------------------

        /// <summary>
        /// Makes this virtual camera the target of the TargettingCamera
        /// </summary>
        public void Activate() {
            TargettingCamera.SetTarget(cameraSettings);
        }

        /// <summary>
        /// Removes the target from the TargettingCamera
        /// </summary>
        public void Deactivate() {
            TargettingCamera.ClearTarget();
        }

    }

}
