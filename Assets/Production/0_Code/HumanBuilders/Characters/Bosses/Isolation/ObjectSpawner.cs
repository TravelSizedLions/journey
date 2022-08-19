using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {


  public class ObjectSpawner : MonoBehaviour {
    public GameObject ObjectPrefab;

    [Range(0, 100)]
    public float YRange;

    public float TimeInterval;

    private float timer;

    public void Awake() {
      timer = TimeInterval;
    }

    public void FixedUpdate() {
      if (timer < 0) {
        SpawnObject();
        timer = TimeInterval;
      } else {
        timer -= Time.fixedDeltaTime;
      }
    }

    public void SpawnObject() {
      Instantiate(
        ObjectPrefab,
        new Vector3(transform.position.x, transform.position.y + Random.Range(-YRange, YRange), transform.position.z),
        Quaternion.identity
      );
    }
  }
}