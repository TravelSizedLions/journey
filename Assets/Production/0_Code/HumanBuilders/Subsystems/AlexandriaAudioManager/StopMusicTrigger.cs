using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class StopMusicTrigger : MonoBehaviour, ITriggerable {
    public virtual void Pull() {
      AlexandriaAudioManager.StopMusic();
    }
  }
}