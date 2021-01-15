using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders {

  public static class CleanUpMissingScripts {
    [MenuItem("GameObject/Remove Missing Scripts", false, 48)]
    public static void CleanupMissingScripts() {

      for (int sIndex = 0; sIndex < SceneManager.sceneCount; sIndex++) {
        Scene scene = SceneManager.GetSceneAt(sIndex);

        int sceneTotal = 0;

        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject gameObject in rootObjects) {
          sceneTotal += CleanUpGameObject(gameObject);
        }

        Debug.Log("Removed " + sceneTotal + " missing scripts from scene \"" + scene.name  + ".\"");
      }

      AssetDatabase.Refresh();

    }


    public static int CleanUpGameObject(GameObject gameObject) {
      int removed = 0;
      if (gameObject.transform.childCount == 0) {
        removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
        if (removed > 0) {
          EditorUtility.SetDirty(gameObject);
        }
      } else {
        foreach (Transform child in gameObject.transform) {
          removed += CleanUpGameObject(child.gameObject);
        }
      }

      return removed;
    }
  }
}