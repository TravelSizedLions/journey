using UnityEditor;
using UnityEngine;

namespace Storm.Editor {

  public class TestWindow : EditorWindow {

    private Rect upperPanel;
    private Rect lowerPanel;

    private Rect resizer;

    private float sizeRatio = 0.5f;

    private bool isResizing;

    private float resizerHeight = 5f;

    private GUIStyle resizerStyle;


    [MenuItem("Window/Test Window")]
    private static void OpenWindow() {
      TestWindow window = GetWindow<TestWindow>();
      window.titleContent = new GUIContent("Test Window");
    }

    private void OnGUI() {
      DrawUpperPanel();
      DrawLowerPanel();
      DrawResizer();

      ProcessEvents(Event.current);

      if (GUI.changed) {
        Repaint();
      }
    }


    private void OnEnable() {
      resizerStyle = new GUIStyle();
      resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
    }

    private void DrawUpperPanel() {
      upperPanel = new Rect(0,0,position.width, position.height*sizeRatio);
    
      GUILayout.BeginArea(upperPanel);
      GUILayout.Label("Upper Panel");
      GUILayout.EndArea();
    }

    private void DrawLowerPanel() {
      lowerPanel = new Rect(0, position.height*sizeRatio, position.width, position.height*(1-sizeRatio));

      GUILayout.BeginArea(lowerPanel);
      GUILayout.Label("Lower Panel");
      GUILayout.EndArea();
    }

    private void DrawResizer() {
      resizer = new Rect(0, (position.height*sizeRatio)-5f, position.width, resizerHeight);

      GUILayout.BeginArea(new Rect(resizer.position + (Vector2.up*5f), new Vector2(position.width,0.5f)), resizerStyle);
      GUILayout.EndArea();

      EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeVertical);
    }

    private void ProcessEvents(Event e) {
      switch (e.type) {
        case EventType.MouseDown: {
          if (e.button == 0 && resizer.Contains(e.mousePosition)) {
            isResizing = true;
          }
          break;
        }
        case EventType.MouseUp: {
          isResizing = false;
          break;
        }
      }

      Resize(e);
    }

    private void Resize(Event e) {
      if (isResizing) {
        sizeRatio = e.mousePosition.y / position.height;
        Repaint();
      }
    }
  }
}