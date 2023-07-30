using System.Collections.Generic;
using HumanBuilders.Attributes;
using UnityEngine;

namespace HumanBuilders {
  public class JustLoopTheSprite : MonoBehaviour {
    public int FrameRate = 60;
    public int StartingFrame;
    public List<Sprite> Sprites;


    private SpriteRenderer spriteRenderer;
    private float timer = 0;
    private int index;

    private void Awake() {
      timer = 1f/FrameRate;
      spriteRenderer = GetComponent<SpriteRenderer>();
      index = StartingFrame;
      spriteRenderer.sprite = Sprites[index];
    }

    public void Update() {
      timer -= Time.deltaTime;
      if (timer < 0) {
        timer = 1f/FrameRate;
        index = (index + 1) % Sprites.Count;
        spriteRenderer.sprite = Sprites[index];
      }
    }
  }
}