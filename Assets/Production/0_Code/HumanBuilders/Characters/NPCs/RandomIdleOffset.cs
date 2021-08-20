using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public class RandomIdleOffset : MonoBehaviour {
    private const string START_TIME = "start_time";

    public Animator anim;

    private void OnEnable() {
      if (anim == null) {
        anim = GetComponent<Animator>();
      }
      
      if (AnimationTools.HasParameter(anim, START_TIME)) {
        anim?.SetFloat(START_TIME, Random.Range(0f, 1f));
      }
    }
  }
}