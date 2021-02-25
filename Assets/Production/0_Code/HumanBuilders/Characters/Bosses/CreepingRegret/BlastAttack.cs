using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// An attack that fires multiple projectiles in a circular radius all at once.
  /// </summary>
  public class BlastAttack : Resetting {

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The projectile prefab that gets fired.
    /// </summary>
    [Tooltip("The projectile prefab that gets fired.")]
    public GameObject projectile;

    /// <summary>
    /// The number of projectiles that get fired.
    /// </summary>
    [Tooltip("The number of projectiles that get fired.")]
    public int NumberOfProjectiles;

    /// <summary>
    /// The speed of the projectiles.
    /// </summary>
    [Min(0f)]
    [Tooltip("The speed of the projectiles.")]
    public float Speed;

    /// <summary>
    /// Where the projectiles originate from.
    /// </summary>
    [Tooltip("Where the projectiles originate from.")]
    public Transform Source;

    /// <summary>
    /// The maximum number of waves that are kept alive at a time.
    /// </summary>
    [Tooltip("The maximum number of waves that are kept alive at a time.")]
    [Min(1)]
    public int MaxWaves;

    /// <summary>
    /// Cached list of projectile waves that have been spawned.
    /// </summary>
    private List<List<GameObject>> waves;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      waves = new List<List<GameObject>>();
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Perform the attack.
    /// </summary>
    public void PerformBlastAttack() {
      if (waves.Count == MaxWaves) {
        RemoveOldestWave();
      }

      List<GameObject> wave = SpawnWave();
      waves.Add(wave);
    }

    /// <summary>
    /// Remove the oldest wave if necessary.
    /// </summary>
    private void RemoveOldestWave() { 
      List<GameObject> oldestWave = waves[0];

      foreach (GameObject proj in oldestWave) {
        Destroy(proj);
      }

      waves.RemoveAt(0);
    }

    /// <summary>
    /// Spawn a new wave of projectiles.
    /// </summary>
    /// <returns>The new wave.</returns>
    private List<GameObject> SpawnWave() {
      List<GameObject> wave = new List<GameObject>();

      for (int i = 0; i < NumberOfProjectiles; i++) {
        float angle = i * (360 / NumberOfProjectiles);
        GameObject fired = Instantiate(
          projectile,
          Source.position,
          Quaternion.Euler(0, 0, angle)
        );

        fired.transform.parent = transform;

        float rads = Mathf.Deg2Rad * angle;

        Rigidbody2D rb = fired.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(Mathf.Cos(rads), Mathf.Sin(rads), 0);
        rb.velocity *= Speed;

        wave.Add(fired);
      }

      return wave;
    }

    //-------------------------------------------------------------------------
    // Resetting Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Clear out the waves of projectiles.
    /// </summary>
    public override void ResetValues() {
      for (int i = 0; i < waves.Count; i++) {
        RemoveOldestWave();
      }
    }
  }
}