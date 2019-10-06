using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.TransitionSystem;

namespace Storm.DialogSystem {

    public class CutsceneDialog : MonoBehaviour {

        public DialogGraph dialog;

        public string nextScene;


        // Start is called before the first frame update
        void Start() {
            dialog = GetComponent<DialogGraph>();
            if (dialog != null) {
                var manager = CutsceneDialogManager.Instance;
                manager.SetCurrentDialog(dialog);
                manager.SetNextScene(nextScene);
                manager.StartDialog();
            }
        }
    }
}

