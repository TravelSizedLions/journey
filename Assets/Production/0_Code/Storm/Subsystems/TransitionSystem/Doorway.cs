using System.Collections;
using System.Collections.Generic;
using Storm.Characters.PlayerOld;
using Storm.Attributes;

using UnityEngine;
using Storm.Characters.Player;

namespace Storm.Subsystems.Transitions {

  /// <summary>
  /// A 
  /// </summary>
  public class Doorway : MonoBehaviour {

    #region Variables
    #region Scene Change Info 
    [Header("Scene Change Info", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The name of the scene this doorway connects to. Does not need to be a full or relative path, or include the scene's file extension.
    /// </summary>
    [Tooltip("The name of the scene this doorway connects to. Does not need to be a full or relative path, or include the scene's file extension.")]
    [SerializeField]
    private string sceneName = "";

    /// <summary>
    /// The name of the spawn point the player will be placed at in the next scene.
    /// If none is specified, the player's spawn will be set to wherever the player 
    /// game object is currently located in-editor in the next scene.
    /// </summary>
    [Tooltip("The name of the spawn point the player will be placed at in the next scene.\nIf none is specified, the player's spawn will be set to wherever the player game object is currently located in-editor in the next scene.")]
    [SerializeField]
    private string spawnName = "";

    [Space(10, order=2)]
    #endregion

    #region Visual Indication
    [Header("Visual Indication", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// The prefab used to prompt the player for a button input.
    /// </summary>
    [Tooltip("The prefab used to prompt the player for a button input.")]
    [SerializeField]
    private GameObject playerPromptPrefab = null;

    #endregion

    /// <summary>
    /// The actual instance of the player prompt that pops up when the player 
    /// is close by the door.
    /// </summary>
    private GameObject playerPromptInstance;

    /// <summary>
    /// The location the player prompt is placed (above the doorway).
    /// </summary>
    private Vector3 promptLocation;

    /// <summary>
    /// A reference to the player.
    /// </summary>  
    private PlayerCharacter player;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      var collider = GetComponent<BoxCollider2D>();
      float verticalOffset = collider.bounds.extents.y + 1.5f;
      promptLocation = new Vector3(transform.position.x, transform.position.y + verticalOffset, 0);
    }

    private void Update() {
        // Move to another level when the player presses up within the
        // boundaries of the door.
        if (player != null && player.HoldingUp()) {
          TransitionManager.Instance.MakeTransition(sceneName, spawnName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        player = collider.GetComponent<PlayerCharacter>();

        // Show the "enter door" indicator.
        if (playerPromptPrefab != null) {
          playerPromptInstance = Instantiate(
            playerPromptPrefab,
            promptLocation,
            Quaternion.identity
          );

          // Make sure the indicator is on the same layer so it doesn't end up
          // behind anything on accident.
          SpriteRenderer doorSprite = GetComponent<SpriteRenderer>();
          SpriteRenderer promptSprite = playerPromptInstance.GetComponent<SpriteRenderer>();
          promptSprite.sortingLayerName = doorSprite.sortingLayerName;
          promptSprite.sortingOrder = doorSprite.sortingOrder - 1;
        }
      }
    }

    private void OnTriggerExit2D(Collider2D collider) {
      if (collider.CompareTag("Player")) {
        var player = collider.gameObject.GetComponent<PlayerCharacterOld>();

        // Remove the "enter door" indicator.
        if (playerPromptInstance != null) {
          playerPromptInstance.transform.parent = null;
          Destroy(playerPromptInstance);
          playerPromptInstance = null;
        }
      }
    }

    #endregion
  }
}