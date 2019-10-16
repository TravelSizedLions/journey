using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Cameras {
    
    /// <summary>
    /// A virtual camera used to frame sections of a level 
    /// </summary>
    public class CameraTarget : MonoBehaviour {

        [Tooltip("")]
        /// <summary>
        /// 
        /// </summary>
        public bool freezeX;

        [Tooltip("")]
        /// <summary>
        /// 
        /// </summary>
        public bool freezeY;

        private Camera cameraSettings;
        private static TargettingCamera cam;

        // Start is called before the first frame update
        /// <summary>
        ///
        /// </summary>
        void Awake(){
            cameraSettings = GetComponent<Camera>();
            if (cameraSettings == null) {
                cameraSettings = transform.parent.GetComponentInChildren<Camera>();
            }

            if (cam == null) {
                cam = FindObjectOfType<TargettingCamera>();
            }
        }

        public void Activate() {
            TargettingCamera.SetTarget(cameraSettings);
        }

        public void Deactivate() {
            TargettingCamera.ClearTarget();
        }

        /// <summary>
        ///
        /// </summary>
        public void OnTriggerEnter2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                Activate();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void OnTriggerStay2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                if (TargettingCamera.target.transform.position != cameraSettings.transform.position) {
                    GameManager.Instance.resets.Reset();
                    Activate();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void OnTriggerExit2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                // In case 2 zones overlap
                if (TargettingCamera.target == cameraSettings.transform) {
                    Deactivate();
                }
            }
        }

    }

}
