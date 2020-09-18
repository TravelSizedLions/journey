using UnityEngine;
using Storm.Subsystems.Save;
using UnityEngine.SceneManagement;

namespace Storm.Flexible {

  /// <summary>
  /// A component for storing the whether or not a particle system is playing
  /// within a scene.
  /// </summary>
  [RequireComponent(typeof(ParticleSystem))]
  [RequireComponent(typeof(GuidComponent))]
  public class SaveParticleState : MonoBehaviour, IStorable {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The unique ID for this game object.
    /// </summary>
    private GuidComponent guid;

    /// <summary>
    /// The particle system to save.
    /// </summary>
    private ParticleSystem particles;

    /// <summary>
    /// Whether or not the particle system is emitting.
    /// </summary>
    private bool isEmitting;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      particles = GetComponent<ParticleSystem>();
      guid = GetComponent<GuidComponent>();

      if (particles != null && guid != null) {
        Retrieve();
      } else {
        if (particles == null) {
          Debug.LogWarning("There's no particle system to retrieve on object \"" + name + ".\"");
        } else {
          Debug.LogWarning("You need to attach a GUID Component onto object \"" + name + ".\"");
        }
      }
    }


    private void Update() {
      isEmitting = particles.isEmitting;
    }


    private void OnDestroy() {
      if (particles != null && guid != null) {
        Store();
      } else {
        if (particles == null) {
          Debug.LogWarning("There's no particle system to store on object \"" + name + ".\"");
        } else {
          Debug.LogWarning("You need to attach a GUID Component onto object \"" + name + ".\"");
        }
      }
    }

    #endregion


    #region Storable API
    //-------------------------------------------------------------------------
    // Storable API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Retrieve state information.
    /// </summary>
    public void Retrieve() {
      string key = guid.ToString()+Keys.PARTICLES_ENABLED;
      if (VSave.Get(StaticFolders.ANIMATION, key, out bool value)) {
        if (value) {
          particles.Play();
        } else {
          particles.Stop();
        }
      }
    }

    /// <summary>
    /// Store the animator's state.
    /// </summary>
    public void Store() {
      string key = guid.ToString()+Keys.PARTICLES_ENABLED;
      bool value = isEmitting;
      Debug.Log("EMITTING: " + value);

      VSave.Set(StaticFolders.ANIMATION, key, value);
    }

    #endregion
  }
}