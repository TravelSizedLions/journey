using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Cutscenes {

  /// <summary>
  /// Represents a single image in a cutscene.
  /// </summary>
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(Sprite))]
  public class Slide : MonoBehaviour {
    public Sprite image;

    public Animator transitions;

    /// <summary>
    /// Delegate type.
    /// </summary>
    public delegate void SlideDelegate();

    /// <summary>
    /// List of subscribers to be notified when
    /// the slide ends.
    /// </summary>
    private SlideDelegate subscribers;


    // Start is called before the first frame update
    void Awake() {
      image = GetComponent<Sprite>();
      transitions = GetComponent<Animator>();
    }

    /// <summary>
    /// Play the slide.
    /// </summary>
    /// <returns>The slide image that was played</returns>
    public Sprite PlaySlide() {
      transitions.SetTrigger("play");
      return image;
    }

    /// <summary>
    /// End the slide.
    /// </summary>
    public void EndSlide() {
      transitions.SetTrigger("end");
    }


    /// <summary>
    /// 
    /// </summary>
    public void Subscribe(SlideDelegate sub) {
      subscribers += sub;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sub"></param>
    public void Unsubscribe(SlideDelegate sub) {
      subscribers -= sub;
    }

    /// <summary>
    /// Notify subscribers that the slide has ended.
    /// </summary>
    public void OnSlideEnded() {
      subscribers();
    }
  }
}