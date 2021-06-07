using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HumanBuilders {
  public static class CameraUtils {


    public static void DrawCameraBox(Vector3 pos, float ortho, float aspect, Color color, float thickness) {
      float vExtent = ortho;
      float hExtent = aspect*vExtent;

      Vector3 bottomLeft = pos + new Vector3(-hExtent, -vExtent, 0);
      Vector3 topLeft = pos + new Vector3(-hExtent, vExtent, 0);
      Vector3 topRight = pos + new Vector3(hExtent, vExtent, 0);
      Vector3 bottomRight = pos + new Vector3(hExtent, -vExtent, 0);
      
      #if UNITY_EDITOR
      Handles.DrawBezier(bottomLeft, topLeft, bottomLeft, topLeft, color, null, thickness);
      Handles.DrawBezier(topLeft, topRight, topLeft, topRight, color, null, thickness);
      Handles.DrawBezier(topRight, bottomRight, topRight, bottomRight, color, null, thickness);
      Handles.DrawBezier(bottomRight, bottomLeft, bottomRight, bottomLeft, color, null, thickness);
      #endif
    }  
  }
}