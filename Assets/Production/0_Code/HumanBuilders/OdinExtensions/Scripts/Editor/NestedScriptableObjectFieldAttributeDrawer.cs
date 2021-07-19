using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {

  [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
  public class NestedScriptableObjectFieldAttributeDrawer<T> : OdinAttributeDrawer<NestedScriptableObjectFieldAttribute, T> where T : ScriptableObject {
    string[] assetPaths = new string[0];
    UnityEngine.Object Parent => (UnityEngine.Object) Property.Tree.RootProperty.ValueEntry.WeakSmartValue;

    protected override void Initialize() {
      Attribute.Type = typeof(T);
      base.Initialize();
    }

    protected override void DrawPropertyLayout(GUIContent label) {
      if (assetPaths.Count() == 0)
        assetPaths = GetAllScriptsOfType();
      if (ValueEntry.SmartValue == null && !Application.isPlaying) {
        //Display value dropdown
        EditorGUI.BeginChangeCheck();
        Rect rect = EditorGUILayout.GetControlRect();
        rect = EditorGUI.PrefixLabel(rect, label);
        var valueIndex = SirenixEditorFields.Dropdown(rect, 0, GetDropdownList(assetPaths));
        if (EditorGUI.EndChangeCheck() && valueIndex > 0) {
          T newObject = (T) ScriptableObject.CreateInstance(UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(assetPaths[valueIndex - 1]).GetClass());
          CreateAsset(newObject);
          ValueEntry.SmartValue = newObject;
        }
      } else {
        //Display object field with a delete button
        EditorGUILayout.BeginHorizontal();
        this.CallNextDrawer(label);
        var rect = EditorGUILayout.GetControlRect(GUILayout.Width(20));
        EditorGUI.BeginChangeCheck();
        SirenixEditorGUI.IconButton(rect, EditorIcons.X);
        EditorGUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck()) {
          //If delete button was pressed:
          AssetDatabase.Refresh();
          GameObject.DestroyImmediate(ValueEntry.SmartValue, true);
          AssetDatabase.ForceReserializeAssets(new [] { AssetDatabase.GetAssetPath(Parent) });
          AssetDatabase.SaveAssets();
          AssetDatabase.Refresh();
        }
      }
    }

    protected virtual string[] GetAllScriptsOfType() {
      var items = UnityEditor.AssetDatabase.FindAssets("t:Monoscript", new [] { "Assets/Scripts" })
        .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
        .Where(x => IsCorrectType(UnityEditor.AssetDatabase.LoadAssetAtPath<MonoScript>(x)))
        .ToArray();
      return items;
    }

    protected bool IsCorrectType(MonoScript script) {
      if (script != null) {
        Type scriptType = script.GetClass();
        if (scriptType != null && (scriptType.Equals(Attribute.Type) || scriptType.IsSubclassOf(Attribute.Type)) && !scriptType.IsAbstract) {
          return true;
        }
      }
      return false;
    }

    protected string[] GetDropdownList(string[] paths) {
      List<String> names = paths.Select(s => Path.GetFileName(s)).ToList();
      names.Insert(0, "null");
      return names.ToArray();
    }

    protected void CreateAsset(T newObject) {
      newObject.name = "_" + newObject.GetType().Name;
      AssetDatabase.Refresh();
      AssetDatabase.AddObjectToAsset(newObject, Parent);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    protected virtual void RemoveAsset(ScriptableObject objectToRemove) {
      UnityEngine.Object.DestroyImmediate(objectToRemove, true);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }
  }
}