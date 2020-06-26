using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Flexible {
  public class ParticleTrigger : MonoBehaviour {

    private new ParticleSystem particleSystem;

    private void Awake() {
      particleSystem = GetComponent<ParticleSystem>();
    }

    public void PlayParticles(float delay) {
      particleSystem.Play();
    }


    public void PauseParticles() {
      particleSystem.Play();
    }


    public void PlayForSeconds(float seconds, float delay) {
      StartCoroutine(_PlayForSeconds(seconds, delay));
    }


    private IEnumerator _Play(float delay) {
      yield return new WaitForSeconds(delay);

      particleSystem.Play();
    }


    private IEnumerator  _PlayForSeconds(float seconds, float delay) {
      yield return new WaitForSeconds(delay);

      particleSystem.Play();

      yield return new WaitForSeconds(seconds);

      particleSystem.Pause();
    } 
  }

}