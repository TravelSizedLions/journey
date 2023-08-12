using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HumanBuilders {
  public class Glowfly : MonoBehaviour {
    [FoldoutGroup("Direction")]
    public Vector3 Offset;

    [FoldoutGroup("Direction")]
    [Range(0, 360)]
    public float Angle = 120;

    [FoldoutGroup("Direction")]
    public float Rotation = 0;

    [FoldoutGroup("Direction")]
    public float Spread = 2;

    [FoldoutGroup("Radius")]
    [LabelText("Min")]
    public float RadiusInner = 5f;

    [FoldoutGroup("Radius")]
    [LabelText("Max")]
    public float RadiusOuter = 7f;

    [FoldoutGroup("Object Count")]
    [LabelText("Min")]
    public float ObjectCountMin = 2;

    [FoldoutGroup("Object Count")]
    [LabelText("Max")]
    public float ObjectCountMax = 4;

    public GameObject FloatingPrefab;

    public SpriteLibrary ObjectSprites;

    [SerializeField]
    [ReadOnly]
    private List<GameObject> addedFloaters;

    private int layerNumber = 0;
    private int layerID;

    private void OnEnable() {
      List<Vector3> locations = new List<Vector3>();
      addedFloaters = new List<GameObject>();
      var sprite = GetComponentInChildren<SpriteRenderer>();
      layerNumber = sprite.sortingOrder;
      layerID = sprite.sortingLayerID;

      for (int i = 0; i < Random.Range(ObjectCountMin, ObjectCountMax); i++) {
        var loc = PickLocation();
        bool found = false;
        float maxAttempts = 10;
        float attempts = 0;
        while (!found && attempts < maxAttempts) {
          found = true;
          foreach (var prevLoc in locations) {
            var diff = prevLoc - loc;
            if (diff.magnitude < Spread) {
              found = false;
              loc = PickLocation();
              attempts++;
              break;
            }
          }
        }

        locations.Add(loc);
      }


      foreach (var loc in locations) {
        addedFloaters.Add(SpawnFloater(loc));
      }
    }

    private void OnDisable() {
      var list = addedFloaters;
      addedFloaters = new List<GameObject>();
      foreach (var floater in list) {
        Destroy(floater);
      }
    }

    private GameObject SpawnFloater(Vector3 location) {
      Sprite sprite = ObjectSprites[Random.Range(0, ObjectSprites.Count-1)];
      var go = Instantiate(FloatingPrefab, location, Quaternion.identity);
      var spriteRenderer = go.GetComponentInChildren<SpriteRenderer>();
      spriteRenderer.sprite = sprite;
      spriteRenderer.sortingLayerID = layerID;
      spriteRenderer.sortingOrder = layerNumber;
      ParticleSystemRenderer particles = go.GetComponentInChildren<ParticleSystemRenderer>();
      particles.sortingLayerID = layerID;
      particles.sortingOrder = layerNumber;
      go.transform.parent = transform;

      return go;
    }

    private Vector3 PickLocation() {
      Vector3 start = transform.position;
      float minAngle = Rotation - Angle/2;
      float maxAngle = Rotation + Angle/2;
      float perlin = Perlin();
      float chosenAngle = Random.Range(minAngle, maxAngle)*Mathf.Deg2Rad;
      Vector3 angleVec = new Vector2(
        Mathf.Sin(chosenAngle),
        Mathf.Cos(chosenAngle)
      );

      perlin = Perlin();
      Vector3 randomPosition = Random.Range(RadiusInner, RadiusOuter)*angleVec;
      return start + Offset + randomPosition;
    }

    private float Perlin() {
      return Mathf.Clamp(Mathf.PerlinNoise(Random.Range(0f, 1f), Random.Range(0f, 1f)), 0, 1);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos() {
      Handles.color = Color.green;
      float modStartAngle = (Rotation + Angle/2)*Mathf.Deg2Rad;
      var rotVect = new Vector3(
        Mathf.Sin(modStartAngle),
        Mathf.Cos(modStartAngle),
        0
      );

      Handles.DrawWireArc(
        transform.position+Offset, 
        Vector3.forward, 
        rotVect, 
        Angle, 
        RadiusInner
      );

      Handles.DrawWireArc(
        transform.position+Offset, 
        Vector3.forward, 
        rotVect, 
        Angle, 
        RadiusOuter
      );
    }
    #endif
  }
}