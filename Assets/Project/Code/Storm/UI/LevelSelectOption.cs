using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Storm.UI {
    
    /// <summary>
    /// One level option on the level select screen.
    /// </summary>
    [System.Serializable]
    public class LevelSelectOption {

        #region Front End

        /// <summary>
        /// The name of the level.
        /// </summary>
        [Tooltip("The name of the level that displays on screen.")]
        public string DisplayName;

        /// <summary>
        /// The image associated with the level.
        /// </summary>
        [Tooltip("The image associated with the level.")]
        public Sprite Sprite;


        [Space(10, order=0)]
        #endregion

        #region Back End

        /// <summary>
        /// The name of the unity scene to load.
        /// </summary>
        [Tooltip("The name of the unity scene to load.")]
        public string SceneName;

        #endregion
    }
}
