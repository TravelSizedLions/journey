using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.TransitionSystem {
  public class Doorway : MonoBehaviour {
    /// <summary>
    /// The name of the scene this doorway connects to. Does not need to be a full or relative path, or include the scene's file extension.
    /// </summary>
    [Tooltip("The name of the scene this doorway connects to. Does not need to be a full or relative path, or include the scene's file extension.")]
    public string sceneName;

    /// <summary>
    /// The name of the spawn point the player will be placed at in the next scene.
    /// If none is specified, the player's spawn will be set to wherever the player 
    /// game object is currently located in-editor in the next scene.
    /// </summary>
    [Tooltip("The name of the spawn point the player will be placed at in the next scene.\nIf none is specified, the player's spawn will be set to wherever the player game object is currently located in-editor in the next scene.")]
    public string spawnName;

    /// <summary>
    /// The prefab used to prompt the player for a button input.
    /// </summary>
    [Tooltip("The prefab used to prompt the player for a button input.")]
    public GameObject playerPromptPrefab;

    /// <summary>
    /// The actual instance of the player prompt that pops up when the player 
    /// is close by the door.
    /// </summary>
    private GameObject playerPromptInstance;

    /// <summary>
    /// The location the player prompt is placed (above the doorway).
    /// </summary>
    private Vector3 promptLocation;

    public void Awake() {
      var collider = GetComponent<BoxCollider2D>();
      float verticalOffset = collider.bounds.extents.y + 1.5f;
      promptLocation = new Vector3(transform.position.x, transform.position.y + verticalOffset, 0);
    }

    public void OnTriggerEnter2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        var player = collider.gameObject.GetComponent<PlayerCharacter>();

        if (playerPromptPrefab != null) {
          playerPromptInstance = Instantiate(
            playerPromptPrefab,
            promptLocation,
            Quaternion.identity
          );
        }
      }
    }

    public void OnTriggerStay2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
          var manager = GameManager.Instance;
          manager.transitions.MakeTransition(sceneName, spawnName);
        }
      }
    }

    public void OnTriggerExit2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        var player = collider.gameObject.GetComponent<PlayerCharacter>();

        if (playerPromptInstance != null) {
          playerPromptInstance.transform.parent = null;
          Destroy(playerPromptInstance);
          playerPromptInstance = null;
        }
      }
    }
  }
}