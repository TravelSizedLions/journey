using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HumanBuilders {

  public class SpawnMenuItem : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    public string Spawn { get { return spawn; }}

    public string Scene { get {return scene; }}

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Tooltip("The text object to populate with the spawn name.")]
    [SerializeField]
    private TextMeshProUGUI textMesh = null;
    private string spawn;
    private string scene;

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void SetSpawn(string scene, string spawn) {
      this.scene = scene;
      this.spawn = spawn;
      if (textMesh != null) {
        textMesh.text = string.IsNullOrEmpty(spawn) ? "scene_start" : spawn;
      }
    }
  }

}