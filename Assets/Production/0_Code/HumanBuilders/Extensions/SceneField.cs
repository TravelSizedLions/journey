using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HumanBuilders {
  [System.Serializable]
  public class SceneField {
    [SerializeField]
    private Object m_SceneAsset = null;
    [SerializeField]
    private string m_SceneName = "";
    public string SceneName {
      get { return m_SceneName; }
    }

    public Object SceneAsset {
      get { return m_SceneAsset; }
      set {
        m_SceneAsset = value;
        m_SceneName = value.name ?? "";
      }
    }
    // makes it work with the existing Unity methods (LoadLevel/LoadScene)
    public static implicit operator string(SceneField sceneField) {
      return sceneField.SceneName;
    }

    public override bool Equals(System.Object obj) {
      if (obj.GetType() != this.GetType()) {
        return false;
      }

      SceneField field = (SceneField) obj;
      return (
        m_SceneName == field.m_SceneName &&
        m_SceneAsset == field.m_SceneAsset
      );
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }
  }

#if UNITY_EDITOR
  [CustomPropertyDrawer(typeof(SceneField))]
  public class SceneFieldPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
      EditorGUI.BeginProperty(_position, GUIContent.none, _property);
      SerializedProperty sceneAsset = _property.FindPropertyRelative("m_SceneAsset");
      SerializedProperty sceneName = _property.FindPropertyRelative("m_SceneName");
      _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
      if (sceneAsset != null) {
        sceneAsset.objectReferenceValue = EditorGUI.ObjectField(_position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
        if (sceneAsset.objectReferenceValue != null) {
          sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
        }
      }
      EditorGUI.EndProperty();
    }
  }
#endif
}