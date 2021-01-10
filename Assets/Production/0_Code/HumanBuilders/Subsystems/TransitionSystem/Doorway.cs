// using System.Collections;
// using System.Collections.Generic;
// using HumanBuildersOld;
// 

// using UnityEngine;
// 

// namespace HumanBuilders {

//   /// <summary>
//   /// A 
//   /// </summary>
//   public class Doorway : MonoBehaviour {



//     #region Unity API
//     //-------------------------------------------------------------------------
//     // Unity API
//     //-------------------------------------------------------------------------

//     private void Awake() {
//       var collider = GetComponent<BoxCollider2D>();
//       float verticalOffset = collider.bounds.extents.y + 1.5f;
//       promptLocation = new Vector3(transform.position.x, transform.position.y + verticalOffset, 0);
//     }

//     private void Update() {
//         // Move to another level when the player presses up within the
//         // boundaries of the door.
//         if (player != null && player.PressedUp()) {
//           TransitionManager.Instance.MakeTransition(sceneName, spawnName);
//         }
//     }

//     private void OnTriggerEnter2D(Collider2D collider) {
//       if (collider.CompareTag("Player")) {
//         player = collider.GetComponent<PlayerCharacter>();

//         // Show the "enter door" indicator.
//         if (playerPromptPrefab != null) {
//           playerPromptInstance = Instantiate(
//             playerPromptPrefab,
//             promptLocation,
//             Quaternion.identity
//           );

//           // Make sure the indicator is on the same layer so it doesn't end up
//           // behind anything on accident.
//           SpriteRenderer doorSprite = GetComponent<SpriteRenderer>();
//           SpriteRenderer promptSprite = playerPromptInstance.GetComponent<SpriteRenderer>();
//           promptSprite.sortingLayerName = doorSprite.sortingLayerName;
//           promptSprite.sortingOrder = doorSprite.sortingOrder - 1;
//         }
//       }
//     }

//     private void OnTriggerExit2D(Collider2D collider) {
//       if (collider.CompareTag("Player")) {
//         // Remove the "enter door" indicator.
//         if (playerPromptInstance != null) {
//           playerPromptInstance.transform.parent = null;
//           Destroy(playerPromptInstance);
//           playerPromptInstance = null;
//         }

//         player = null;
//       }
//     }

//     #endregion
//   }
// }