using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Bosses {

  /// <summary>
  /// An eye that stares at the player.
  /// </summary>
  public class StaringEye : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// How strong the pupil's gravitation is towards the player.
    /// </summary>
    [SerializeField]
    [Tooltip("How strong the pupil's gravitation is towards the player.")]
    private float gravitationStrength = 1f;

    /// <summary>
    /// The thing that the eye should be staring at.
    /// </summary>
    private Transform target;

    /// <summary>
    /// The rigidbody attached to this game object.
    /// </summary>
    private Rigidbody2D rb;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    // Start is called before the first frame update
    private void Start() {
      rb = GetComponent<Rigidbody2D>();
      target = GameManager.Player.transform;
    }

    // Update is called once per frame
    private void FixedUpdate() {
      rb.AddForce((target.position - transform.position).normalized*gravitationStrength);
    }
    #endregion


  }

}