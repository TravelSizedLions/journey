using UnityEditor;
using UnityEngine;

namespace HumanBuilders {
  [InitializeOnLoad]
  public static class ProjectWindowDetails {
    static ProjectWindowDetails() {
      EditorApplication.projectWindowItemOnGUI += DrawAssetDetails;
    }
    private static void DrawAssetDetails(string guid, Rect rect) {
      if (Application.isPlaying || Event.current.type != EventType.Repaint) {
        return;
      }
      // // Right align label:
      // const int width = 250;
      // rect.x += rect.width - width;
      // rect.width = width;
      // GUI.Label(rect, guid);

      string path = AssetDatabase.GUIDToAssetPath(guid);
      if (!string.IsNullOrEmpty(path)) {
         ScriptableObject obj = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
         
      }
    }

    static void IconGUI ( string s, Rect r ) {
        string fileName = AssetDatabase.GUIDToAssetPath( s );
        int index = fileName.LastIndexOf( '.' );
        if ( index == -1 ) return;
        string fileType = fileName.Substring( fileName.LastIndexOf( "." ) + 1 );
        r.width = r.height;
        switch ( fileType ) {
            case "cs":
                //Put your icon images somewhere in the project, and refer to them with a string here
                GUI.DrawTexture( r, (Texture2D) AssetDatabase.LoadAssetAtPath( "Assets/Editor/Icons/Icon1.psd", typeof( Texture2D ) ) );
                break;
            case "psd":
                GUI.DrawTexture( r, (Texture2D) AssetDatabase.LoadAssetAtPath( "Assets/Editor/Icons/Icon2.psd", typeof( Texture2D ) ) );
                break;
            case "png":
                GUI.DrawTexture( r, (Texture2D) AssetDatabase.LoadAssetAtPath( "Assets/Editor/Icons/Icon3.psd", typeof( Texture2D ) ) );
                break;
        }
    }

    private static bool IsMainListAsset(Rect rect) {
      // Don't draw details if project view shows large preview icons:
      if (rect.height > 20) {
        return false;
      }
      // Don't draw details if this asset is a sub asset:
      if (rect.x > 16) {
        return false;
      }
      return true;
    }
  }
}