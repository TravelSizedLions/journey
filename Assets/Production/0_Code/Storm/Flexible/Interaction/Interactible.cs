using System.Collections.Generic;
using Storm.Attributes;
using Storm.Characters.Player;
using Storm.Subsystems.Transitions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Storm.Flexible.Interaction {
  /// <summary>
  /// The base class for objects the player can interact with in the environment.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public abstract class Interactible : MonoBehaviour {
    

    #region Properties

    /// <summary>
    /// Whether or not the player is still interacting with this object.
    /// </summary>
    public bool StillInteracting {
      get { return interacting; }
    }

    /// <summary>
    /// The center of the interactive object.
    /// </summary>
    public Vector2 Center {
      get { return col != null ? (Vector2)col.bounds.center : Vector2.positiveInfinity; }
    }

    /// <summary>
    /// Whether or not the indicator for this interactive object should be
    /// placed over the player. If true, the indicator will float over the
    /// player. Otherwise, the indicator will float over the object.
    /// </summary>
    /// <value></value>
    public bool IndicateOverPlayer {
      get { return indicateOnPlayer; }
    }

    /// <summary>
    /// The name of the indicator.
    /// </summary>
    public string IndicatorName {
      get { return indicator != null ? indicator.Name : ""; }
    }

    /// <summary>
    /// Offset for the indicator, relative to the parent transform.
    /// </summary>
    public Vector2 Offset {
      get { return offset; }
    }


    /// <summary>
    /// The game object the interaction indicator will be placed over.
    /// </summary>
    public Transform IndicatorTarget {
      get { return indicatorTarget; }
    }

    /// <summary>
    /// The indicator to use for this interactive object.
    /// </summary>
    public Indicator Indicator {
      get { return indicator; }
    }

    /// <summary>
    /// The area the player needs to be standing within to interact with this object.
    /// </summary>
    public Collider2D InteractiveArea {
      get { return interactibleArea; }
    }

    /// <summary>
    /// The physical collider for this object.
    /// </summary>
    public Collider2D Collider {
      get { return col; }
    }

    #endregion

    #region Fields
    [Header("Indicator Settings", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The interaction indicator to use for this interactive object.
    /// </summary>
    [Tooltip("The interaction indicator to use for this interactive object.")]
    [SerializeField]
    protected Indicator indicator;


    /// <summary>
    /// The game object the interaction indicator will be placed over. If left
    /// empty, this will default to the game object the script is on.
    /// </summary>
    [Tooltip("The game object the interaction indicator will be placed over. If left empty, this will default to the game object the script is on.")]
    [SerializeField]
    protected Transform indicatorTarget;


    /// <summary>
    /// Whether or not the indicator should be placed over the player. If true,
    /// the indicator will be parented to the player on the same sprite render
    /// layer and at render sort order - 1.
    /// </summary>
    [Tooltip("Whether or not the indicator should be placed over the player. If true,\nthe indicator will be parented to the player on the same sprite render\nlayer and at render sort order - 1.")]
    [SerializeField]
    protected bool indicateOnPlayer;

    /// <summary>
    /// Offset for the indicator, relative to the parent transform.
    /// </summary>
    [Tooltip("Offset for the indicator over either this object or the player.")]
    [SerializeField]
    protected Vector2 offset;

    [Space(10, order=2)]

    /// <summary>
    /// The area the player needs to be within in order to interact with this object.
    /// </summary>
    protected Collider2D interactibleArea;

    /// <summary>
    /// The physical collider for the object.
    /// </summary>
    protected Collider2D col;

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    protected IPlayer player;

    /// <summary>
    /// Whether the interaction with this object is still going on.
    /// </summary>
    protected bool interacting;

    /// <summary>
    /// Whether or not the indicator for this indicator has been registered.
    /// </summary>
    protected bool registered;
    #endregion

    #region Unity API
    protected void Awake() {
      player = FindObjectOfType<PlayerCharacter>();

      if (indicatorTarget == null) {
        indicatorTarget = transform;
      }

      Collider2D[] cols = gameObject.GetComponentsInChildren<Collider2D>();
      SetColliders(cols);
    }
    
    private void SetColliders(Collider2D[] cols) {
      // Assign colliders based on their properties.
      if (cols.Length > 1) {
        if (cols[0].isTrigger) {
          interactibleArea = cols[0];
          col = cols[1];
        } else {
          col = cols[0];
          interactibleArea = cols[1];
        }
      } else if (cols.Length == 1) {
        if (cols[0].isTrigger) {
          interactibleArea = cols[0];
        } else {
          col = cols[0];
        }
      } else {
        Debug.LogWarning("This interactible needs a trigger collider attached to function! GameObject name: " + name);
      }
    }

    protected virtual void OnDestroy() {
      interacting = false;
      if (player != null) {
      player.RemoveInteractible(this);
      }
    }

    protected void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        player = other.GetComponent<PlayerCharacter>();
        TryRegister();
        player.AddInteractible(this);
      }
    }

    private void TryRegister() {
      if (!registered) {
        registered = true;
        player.RegisterIndicator(indicator);
      }
    }

    protected void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        if (player != null) {
          player.RemoveInteractible(this);
          player = null;
        }
      }
    }

    #endregion

    public void Inject(Collider2D collider) {
      this.col = collider;
    }

    #region Abstract Interface
    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    /// <seealso cref="Carriable.OnInteract" />
    /// <seealso cref="TransitionDoor.OnInteract" />
    /// <seealso cref="Talkative.OnInteract" />
    public abstract void OnInteract();

    /// <summary>
    /// Whether or not the indicator for this interactible should be shown.
    /// </summary>
    /// <remarks>
    /// This is used when this particular interactive object is the closest to the player. If the indicator can be shown
    /// that usually means it can be interacted with.
    /// </remarks>
    /// <seealso cref="Carriable.ShouldShowIndicator" />
    /// <seealso cref="TransitionDoor.ShouldShowIndicator" />
    /// <seealso cref="Talkative.ShouldShowIndicator" />
    public abstract bool ShouldShowIndicator();
    #endregion
  }

}
