using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HumanBuilders {
  [ExecuteAlways]
  public class SceneMenuItem : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name of the scene.
    /// </summary>
    public string Scene { get { return sceneName; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The name of the scene.
    /// </summary>
    [SerializeField]
    [Tooltip("The name of the scene.")]
    [ReadOnly]
    private string sceneName;

    /// <summary>
    /// The prefab to use when displaying spawn points on the menu.
    /// </summary>
    [Tooltip("The prefab to use when displaying spawn points on the menu.")]
    [SerializeField]
    private GameObject spawnItemPrefab;

    [SerializeField]
    private TextMeshProUGUI sceneTextMesh;

    [SerializeField]
    private GameObject content = null;

    [SerializeField]
    private GameObject arrowOpened = null;
    
    [SerializeField]
    private GameObject arrowClosed = null;

    private bool contentLoaded = false;


    public void SetScene(string sceneName) {
      this.sceneName = sceneName;
      if (sceneTextMesh != null) {
        sceneTextMesh.text = sceneName;
      }
    }

    public void ToggleFoldout() {
      if (content != null) {
        content.SetActive(!content.activeSelf);
        if (!contentLoaded && content.activeSelf) {
          if (ScenesMenu.MapData.ContainsKey(sceneName)) {
            foreach (string spawnName in ScenesMenu.MapData[sceneName]) {
              GameObject go = Instantiate(spawnItemPrefab, content.transform);
              SpawnMenuItem spawnItem = go.GetComponentInChildren<SpawnMenuItem>(true);
              spawnItem.SetSpawn(sceneName, spawnName);
            }
          }

          if (content.transform.childCount == 0) {
            GameObject go = Instantiate(spawnItemPrefab, content.transform);
            SpawnMenuItem spawnItem = go.GetComponentInChildren<SpawnMenuItem>(true);
            spawnItem.SetSpawn(sceneName, "");
          }

          contentLoaded = true;
        }
      }

      if (arrowOpened != null) {
        arrowOpened.SetActive(!arrowOpened.activeSelf);
      }

      if (arrowClosed != null) {
        arrowClosed.SetActive(!arrowClosed.activeSelf);
      }

      LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
    }
  }
}