using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DiscGizmo : MonoBehaviour {
#if UNITY_EDITOR
  public float Radius = 0.33f;
  public Color Color = Color.white;

  private void OnDrawGizmos() {
    Handles.color = Color;
    Handles.DrawWireDisc(transform.position, Vector3.forward, Radius);
  }
#endif
}