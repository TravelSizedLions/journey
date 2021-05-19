using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HumanBuilders {
  [ExecuteAlways]
  public class SceneMenuItem : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public string Scene { get { return sceneName; } }
    public List<SpawnMenuItem> SpawnPoints { get { return spawns; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private string sceneName;
    private List<SpawnMenuItem> spawns;

    [SerializeField]
    private GameObject content;

    [SerializeField]
    private GameObject arrowOpened;
    
    [SerializeField]
    private GameObject arrowClosed;


    private void Awake() {
      spawns = new List<SpawnMenuItem>();
    }

    public void SetScene(string scene) {
      sceneName = scene;
      // Get all spawn point names in scene.
      // SetSpawnPoints()
    }

    public void SetSpawnPoints(List<SpawnMenuItem> spawnPoints) {
      spawns = spawnPoints;
      // Clear out dynamic content section
      
      // For each spawn point
      //  - instantiate SpawnMenuItem
      //  - set spawn data into item
      //  - Make dynamic content section parent of newly created item.
    }


    public void ToggleFoldout() {
      if (content != null) {
        content.SetActive(!content.activeSelf);
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