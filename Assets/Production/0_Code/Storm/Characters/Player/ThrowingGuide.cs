using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


namespace Storm.Characters.Player {
  public class ThrowingGuide : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The maximum length of the arrow guide.
    /// </summary>
    [Tooltip("The maximum length of the arrow guide.")]
    public float MaxLength;

    /// <summary>
    /// The offset from the real throwing position.
    /// </summary>
    [Tooltip("The offset from the real throwing position.")]
    public Vector2 Offset;

    /// <summary>
    /// The base of the arrow.
    /// </summary>
    [FoldoutGroup("Prefabs")]
    [Tooltip("The base of the arrow.")]
    public SpriteRenderer Base;

    /// <summary>
    /// The middle section of the arrow.
    /// </summary>
    [FoldoutGroup("Prefabs")]
    [Tooltip("The middle section of the arrow.")]
    public SpriteRenderer Middle;

    /// <summary>
    /// The end cap of the arrow.
    /// </summary>
    [FoldoutGroup("Prefabs")]
    [Tooltip("The end cap of the arrow.")]
    public SpriteRenderer Head;


    /// <summary>
    /// The instance of the base of the arrow.
    /// </summary>
    private SpriteRenderer baseInstance;

    /// <summary>
    /// The instances of the arrow's mid segments.
    /// </summary>
    private SpriteRenderer[] middleInstances;

    /// <summary>
    /// The instance of the arrow's head.
    /// </summary>
    private SpriteRenderer headInstance;

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private IPlayer player;

    /// <summary>
    /// Whether or not the guide has extended out to its maximum length.
    /// </summary>
    private bool fullyExtended;

    /// <summary>
    /// Whether or not the guide is being drawn.
    /// </summary>
    [Tooltip("Whether or not the guide is being drawn.")]
    [SerializeField]
    [ReadOnly]
    private bool visible;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Start() {
      player = GameManager.Player;

      baseInstance = Instantiate(Base, Vector3.zero, Quaternion.identity);
      baseInstance.transform.parent = transform;

      int numInstances = (int)((MaxLength-0.5f)/0.5f);
      middleInstances = new SpriteRenderer[numInstances];

      for (int i = 0; i < numInstances; i++) {
        middleInstances[i] = Instantiate(Middle, Vector3.zero, Quaternion.identity);
        middleInstances[i].transform.parent = transform;
      }

      headInstance = Instantiate(Head, Vector3.zero, Quaternion.identity);
      headInstance.transform.parent = transform;

      RemoveGuide();
    }

    private void LateUpdate() {
      if (player == null) {
        player = GameManager.Player;
      }

      if (player != null) {
        if (player.CarriedItem != null) {
          DrawGuide();
        } else if (visible) {
          RemoveGuide();
        }
      }
    }


    #endregion

    #region Helper Methods

    /// <summary>
    /// Draw the throwing guide.
    /// </summary>
    private void DrawGuide() {
      visible = true;

      Vector2 direction = player.GetThrowingDirection(false);
      Vector2 position = player.GetThrowingPosition();
      float angleDeg = Mathf.Rad2Deg*Mathf.Atan2(direction.y, direction.x) - 90;
      float length = Mathf.Min(MaxLength, direction.magnitude);

      PlaceBase(position, direction, angleDeg, length);
      PlaceMidSections(position, direction, angleDeg, length);
      PlaceHead(position, direction, angleDeg, length);
    }


    /// <summary>
    /// Draw the base segment of the throwing guide.
    /// </summary>
    /// <param name="angleDeg">The angle the arrow should be placed at.</param>
    private void PlaceBase(Vector2 position, Vector2 direction, float angleDeg, float length) {
      if (length > 0.5f) {
        baseInstance.transform.position = position + direction.normalized + Offset;
        baseInstance.transform.eulerAngles = new Vector3(0, 0, angleDeg);
        baseInstance.enabled = true;
      } else {
        baseInstance.enabled = false;
      }

    }

    /// <summary>
    /// Draw the midsections of the throwing guide.
    /// </summary>
    /// <param name="angleDeg">The angle the arrow should be placed at.</param>
    private void PlaceMidSections(Vector2 position, Vector2 direction, float angleDeg, float length) {
      fullyExtended = true;
      if (length > 0.5f) {
        for (int i = 2; i < middleInstances.Length; i++) {
          SpriteRenderer section = middleInstances[i];

          float dist = (i+1)*(0.5f);

          if (dist < length) {
            section.transform.eulerAngles = new Vector3(0, 0, angleDeg);
            section.transform.position = position + direction.normalized*dist + Offset;
            section.enabled = true;
          } else {
            section.enabled = false;
            fullyExtended = false;
          }
        }
      } else {
        fullyExtended = false;
        for (int i = 0; i < middleInstances.Length; i++) {
          middleInstances[i].enabled = false;
        }
      }


    }

    /// <summary>
    /// Draw the head of the throwing guide.
    /// </summary>
    /// <param name="angleDeg">The angle the arrow should be placed at.</param>
    private void PlaceHead(Vector2 position, Vector2 direction, float angleDeg, float length) {
      if (length > 0.5f && fullyExtended) {
        headInstance.transform.eulerAngles = new Vector3(0, 0, angleDeg);
        headInstance.transform.position = position + direction.normalized*length + Offset;
        headInstance.enabled = true;
      } else {
        headInstance.enabled = false;
      }
    }

    private void RemoveGuide() {
      visible = false;

      headInstance.enabled = false;
      baseInstance.enabled = false;
      foreach (SpriteRenderer section in middleInstances) {
        section.enabled = false;
      }
    }

    #endregion

  }

}