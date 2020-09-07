using UnityEngine;

namespace Storm.Characters.Bosses {

  /// <summary>
  /// 
  /// </summary>
  [RequireComponent(typeof(Animator))]
  public class BossEngine : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The animator for this boss.
    /// </summary>
    private Animator anim;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      anim = GetComponent<Animator>();
    }

    #endregion

  }
}