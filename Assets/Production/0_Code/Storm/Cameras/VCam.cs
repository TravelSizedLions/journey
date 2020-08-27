using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

namespace Storm.Cameras {
  public class VCam : MonoBehaviour {


    private GameObject cam;

    private void Awake() {
      cam = transform.GetChild(0).gameObject;

      cam.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player") && !other.isTrigger) {
        cam.SetActive(true);
      }
    }

    private void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player") && !other.isTrigger) {
        cam.SetActive(false);
      }
    }


  }

}