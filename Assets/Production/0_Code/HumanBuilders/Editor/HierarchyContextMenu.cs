using UnityEditor;
using UnityEngine;

namespace HumanBuilders.Editor {
  public static class HierarchyContextMenu {

    [MenuItem("GameObject/Create Other/Empty At Origin #e")]
    public static void CreateEmptyAtOrigin(MenuCommand menuCommand) {
      GameObject go = new GameObject("empty");

      if (Selection.activeGameObject == null) {
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
      } else {
        GameObjectUtility.SetParentAndAlign(go, Selection.activeGameObject);
      }
      
      Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

      Selection.activeObject = go;
    }

    [MenuItem("GameObject/Create Other/Title Object #t")]
    public static void CreateTitleObject(MenuCommand menuCommand) {
      GameObject go = new GameObject("---- Title ---");

      GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
      
      Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

      Selection.activeObject = go;
    }

  }
}