using System.Collections;
using System.Collections.Generic;
using Storm.Cameras;
using Storm.Flexible;
using Storm.Flexible.Interaction;
using Storm.Subsystems.Reset;
using Storm.Subsystems.Transitions;
using UnityEngine;

namespace Storm.Characters.Bosses {
  public class MegaEye : BossWeakSpot {
    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// A component for shaking the eye.
    /// </summary>
    private Shaking shaking;

    /// <summary>
    /// A component that causes the camera to shake when the eye opens.
    /// </summary>
    private CameraShaker cameraShake;

    /// <summary>
    /// The list of smaller eyes.
    /// </summary>
    private MiniEye[] miniEyes;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected new void Awake() {
      base.Awake();
      shaking = GetComponent<Shaking>();
      cameraShake = GetComponent<CameraShaker>();
      miniEyes = FindObjectsOfType<MiniEye>();
    }

    private void Start() {
      foreach (MiniEye eye in miniEyes) {
        eye.OnEyeClose += TryExpose;
      }
    }
    #endregion

    #region Boss Weak Spot API
    //-------------------------------------------------------------------------
    // Boss Weak Spot API
    //-------------------------------------------------------------------------


    /// <summary>
    /// Open a random set of eyes.
    /// /// </summary>
    public void OpenRandomEyes(int number) {
      if (number > miniEyes.Length || AllMiniEyesOpen()) {
        return;
      } else if (number == miniEyes.Length) {
        // Just open all of the eyes instead going through the random selection
        // process.
        OpenAllMiniEyes();
      } else {

        // Randomly open a subset of the eyes on the scene.
        for (int i = 0; i < number; i++) {
          bool opened = false;

          // Open a random eye. If the selected eye is already open,
          // choose a different one.
          while (!opened) {
            int index = Random.Range(0, miniEyes.Length);
            if (!miniEyes[index].Exposed) {
              miniEyes[index].Expose();
              opened = true;
            }
          }
        }

      }
    }

    /// <summary>
    /// Opens all of the boss' mini eyes.
    /// </summary>
    public void OpenAllMiniEyes(bool expose = true) {
      foreach (MiniEye eye in miniEyes) {
        if (expose) {
          eye.Expose();
        } else {
          eye.Open();
        }        
      }
    }

    /// <summary>
    /// Close all mini eyes.
    /// </summary>
    public void CloseAllMiniEyes() {
      foreach (MiniEye eye in miniEyes) {
        eye.Close(true);
      }
    }


    public void UnexposeAllMiniEyes() {
      foreach (MiniEye eye in miniEyes) {
        eye.Unexpose();
      }
    }

    /// <summary>
    /// Open the eye.
    /// </summary>
    public void Open() {
      animator.SetBool("open", true);
      shaking.Shake();
      cameraShake.Shake();
    }

    
    /// <summary>
    /// Close the eye.
    /// </summary>
    public void Close() {
      animator.SetBool("open", false);
    }
    #endregion

    #region Weak Spot Interface
    //-------------------------------------------------------------------------
    // Weak Spot Interface
    //-------------------------------------------------------------------------
    protected override bool DamageCondition(Collider2D col) {
      Carriable carriable = col.transform.root.GetComponentInChildren<Carriable>();
      return carriable != null &&
        col == carriable.Collider &&
        Exposed;
    }

    protected override void OnExposed() {
      StartCoroutine(_WaitToOpen());
    }

    /// <summary>
    /// Close the eye.
    /// </summary>
    /// <param name="col">The collider of the object that hit the eye.</param>
    protected override void OnHit(Collider2D col) {
      // Stop the object that hit the eye.
      Carriable carriable = col.transform.root.GetComponent<Carriable>();
      if (carriable != null) {
        carriable.Physics.Velocity = Vector2.zero;
      }

      // Have the eye shake and flash red.
      shaking.Shake();
      animator.SetTrigger("damage");

      boss.TakeDamage(1);

      // Close the eye.
      Close();
    }

    protected override void OnUnexposed() {}

    public void TryExpose() {
      Debug.Log("Trying to expose.");

      if (AnyMiniEyesOpen()) {
        Debug.Log("Not all eyes closed");
        return;
      }

      // Opens the eye.
      Expose();
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// Open all of the eyes in the scene (including the decorative ones).
    /// </summary>
    public void OpenPropEyes() {
      StartCoroutine(_OpenPropEyes());
    }

    private IEnumerator _OpenPropEyes() {
      List<PropEye> closedEyes = new List<PropEye>(transform.root.GetComponentsInChildren<PropEye>(true));

      for (int size = closedEyes.Count; size > 0; size--) {
        int index = Random.Range(0, size);

        PropEye eye = closedEyes[index];
        closedEyes.Remove(eye);
        eye.Open();

        yield return new WaitForSeconds(0.05f);

      }

      yield return new WaitForSeconds(2f);

      TransitionManager.MakeTransition("end_card");
    }

    /// <summary>
    /// Whether or not any mini eyes are open.
    /// </summary>
    private bool AnyMiniEyesOpen() {
      foreach (MiniEye eye in miniEyes) {
        if (eye.Exposed) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Whether or not all mini eyes are currently open.
    /// </summary>
    private bool AllMiniEyesOpen() {
      foreach (MiniEye eye in miniEyes) {
        if (!eye.Exposed) {
          return false;
        }
      }

      return true;
    }

    private IEnumerator _WaitToOpen() {
      yield return new WaitForSeconds(1f);
      Open();
    }


    #endregion
  }
}