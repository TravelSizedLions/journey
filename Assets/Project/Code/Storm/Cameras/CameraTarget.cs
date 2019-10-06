using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Cameras {

    public class CameraTarget : MonoBehaviour {

        public bool freezeX;

        public bool freezeY;

        private Camera cameraSettings;
        private static TargettingCamera cam;

        // Start is called before the first frame update
        void Start(){
            cameraSettings = GetComponent<Camera>();
            if (cameraSettings == null) {
                cameraSettings = transform.parent.GetComponentInChildren<Camera>();
            }

            if (cam == null) {
                cam = FindObjectOfType<TargettingCamera>();
            }
        }

        public void OnTriggerEnter2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                cam.SetTarget(cameraSettings);
                cam.SetFreezeX(freezeX);
                cam.SetFreezeY(freezeY);
            }
        }

        public void OnTriggerStay2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                if (cam.target.transform.position != cameraSettings.transform.position) {
                    GameManager.Instance.resets.Reset();
                    cam.SetTarget(cameraSettings);
                    cam.SetFreezeX(freezeX);
                    cam.SetFreezeY(freezeY);
                }
            }
        }

        public void OnTriggerExit2D(Collider2D col) {
            if (col.gameObject.CompareTag("Player")) {
                // In case 2 zones overlap
                if (cam.target == cameraSettings.transform) {
                    cam.ClearTarget();
                    cam.ClearFreezeX();
                    cam.ClearFreezeY();
                }
            }
        }

    }

}
