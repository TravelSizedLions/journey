using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace TSL.Subsystems.WorldView {
  [CustomNodeGraphEditor(typeof(WorldViewGraph))]
  public class WorldViewGraphEditor : NodeGraphEditor {
    public override bool CanRemove(Node node) {
      return false;
    }

    public override string GetNodeMenuName(Type type) {
      return null;
    }

    public override void AddContextMenuItems(GenericMenu menu, Type compatibleType = null, NodePort.IO direction = NodePort.IO.Input) {
      menu.AddItem(new GUIContent("Rebuild From Scratch"), false, Rebuild);
      menu.AddItem(new GUIContent("Sync Scenes"), false, SyncScenes);
      menu.AddItem(new GUIContent("Sync Connections"), false, SyncConnections);
      menu.AddItem(new GUIContent("Sync All"), false, FullSync);
      base.AddContextMenuItems(menu, compatibleType, direction);
    }

#if UNITY_EDITOR
    public void Rebuild() {
      WorldViewSynchronizer.Disable();
      if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
        (WorldViewWindow.current.graph as WorldViewGraph).Rebuild();
      }
      WorldViewSynchronizer.Enable();
    }

    public void FullSync() {
      WorldViewSynchronizer.Disable();
      if((WorldViewWindow.current.graph as WorldViewGraph).FullSync()) {
        AssetDatabase.SaveAssets();
      }
      WorldViewSynchronizer.Enable();
    }

    public void SyncScenes() {
      WorldViewSynchronizer.Disable();
      if((WorldViewWindow.current.graph as WorldViewGraph).SyncScenes()) {
        AssetDatabase.SaveAssets();
      }
      WorldViewSynchronizer.Enable();
    }

    public void SyncConnections() {
      WorldViewSynchronizer.Disable();
      if((WorldViewWindow.current.graph as WorldViewGraph).SyncConnections()) {
        AssetDatabase.SaveAssets();
      }
      WorldViewSynchronizer.Enable();
    }


#endif
  }
}