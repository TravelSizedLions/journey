using UnityEngine;
using UnityEngine.SceneManagement;

using Storm.Extensions;
using Storm.TransitionSystem;

namespace Storm.DialogSystem {
    [RequireComponent(typeof(DialogManager))]
    public class CutsceneDialogManager : MonoBehaviour {

        private DialogManager manager;

        private GameObject cutscene;

        private string nextScene;

        #region Unity Functions
        //---------------------------------------------------------------------
        // Unity Functions
        //---------------------------------------------------------------------
        public void Awake() {
            cutscene = GameObject.FindGameObjectWithTag("Cutscene");
            if (manager == null) {
                manager = GetComponent<DialogManager>();
            }
        }

        public void Update() {
           if (manager.isInConversation && Input.GetKeyDown(KeyCode.Space)) {
                manager.NextSentence();
                if (!manager.isInConversation) {
                    TransitionManager.Instance.MakeTransition(nextScene);
                    TransitionManager.Instance.postTransitionEvents.AddListener(this.OnCutsceneEnd);
                }
            }
        }

        #endregion

        #region Dialog Handling
        //---------------------------------------------------------------------
        // Dialog Handling Functions
        //---------------------------------------------------------------------

        public void StartDialog() {
            cutscene.SetActive(true);
            manager.StartDialog();
        }

        #endregion

        #region Getters / Setters
        public void SetNextScene(string scene) {
            nextScene = scene;
        }

        public void SetCurrentDialog(DialogGraph dialog) {
            manager.SetCurrentDialog(dialog);
        }

        public void OnCutsceneBegin() {
            //
        }

        public void OnCutsceneEnd() {
            cutscene.SetActive(false);
        }
        #endregion

    }
}
