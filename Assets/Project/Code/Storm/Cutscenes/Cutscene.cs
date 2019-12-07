using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Storm.TransitionSystem;


namespace Storm.Cutscenes {

    /// <summary>
    /// A class for playing a cutscene slideshow to the player.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class Cutscene : MonoBehaviour
    {
        public List<Sprite> Images;

        private Image screen;

        public string NextScene;

        public string NextSpawn;

        private int currentImage;



        // Start is called before the first frame update
        void Awake()
        {
            if (Images == null) {
                Images = new List<Sprite>();
            }

            currentImage = 0;
            
            screen = GetComponent<Image>();

            if (Images.Count > 0) {
                screen.sprite = Images[0];
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) {
                // if it's not the last image:
                //  Go to the next image.
                if (currentImage != Images.Count-1) {
                    NextImage();
                } else {
                    ChangeScenes();
                }
            }
        }

        public void NextImage() {
            currentImage++;
            screen.sprite = Images[currentImage];
        }


        public void ChangeScenes() {
            TransitionManager.Instance.MakeTransition(NextScene, NextSpawn);
        }
    }

}