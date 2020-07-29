using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Flexible {
  public class ParticleTrigger : MonoBehaviour {

    private ParticleSystem particles;

    private void Awake() {
      particles = GetComponent<ParticleSystem>();
    }

    public void PlayParticles(float delay) {
      particles.Play();
    }


    public void PauseParticles() {
      particles.Play();
    }


    public void PlayForSeconds(float seconds, float delay) {
      StartCoroutine(_PlayForSeconds(seconds, delay));
    }


    private IEnumerator _Play(float delay) {
      yield return new WaitForSeconds(delay);

      particles.Play();
    }


    private IEnumerator  _PlayForSeconds(float seconds, float delay) {
      yield return new WaitForSeconds(delay);

      particles.Play();

      yield return new WaitForSeconds(seconds);

      particles.Pause();
    } 
  }

}