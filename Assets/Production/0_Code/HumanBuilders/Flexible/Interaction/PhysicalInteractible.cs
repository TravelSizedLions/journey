using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  [RequireComponent(typeof(GuidComponent))]
  public abstract class PhysicalInteractible : Interactible, ITriggerableParent {
    public Vector2 Center {
      get { 
        if (col != null) {
          return (Vector2)col.bounds.center;
        } else if (interactibleArea != null) {
          return (Vector2)interactibleArea.bounds.center;
        }

        return Vector2.positiveInfinity;
      }
    }

    public bool IndicateOverPlayer {
      get { return indicateOnPlayer; }
    }

    public string IndicatorName {
      get { return indicator != null ? indicator.Name : ""; }
    }

    public Vector2 Offset {
      get { return offset; }
    }

    public Transform IndicatorTarget {
      get { return indicatorTarget; }
    }

    public Indicator Indicator {
      get { return indicator; }
    }

    public Collider2D InteractiveArea {
      get { return interactibleArea; }
    }

    public Collider2D Collider {
      get { return col; }
    }

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
    [LabelText("Interactive Area")]
    public Collider2D interactibleArea;

    /// <summary>
    /// The physical collider for the object.
    /// </summary>
    [LabelText("Physical Collider")]
    public Collider2D col;

    /// <summary>
    /// Whether or not the indicator for this indicator has been registered.
    /// </summary>
    protected bool registered;

    [FoldoutGroup("Enter & Exit Callbacks")]
    [PropertyOrder(998)]
    public UnityEvent OnEnterArea;

    [FoldoutGroup("Enter & Exit Callbacks")]
    [PropertyOrder(999)]
    public UnityEvent OnExitArea;

    protected override void Awake() {
      base.Awake();

      if (indicatorTarget == null) {
        indicatorTarget = transform;
      }

      if (interactibleArea == null && col == null) {
        Collider2D[] cols = gameObject.GetComponentsInChildren<Collider2D>();
        SetColliders(cols);
      }

    }
    
    /// <summary>
    /// One collider will be a physical collider, and the other's a trigger
    /// area. This determines which collider is which and stores them correctly.
    /// </summary>
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

    protected override void OnDestroy() {
      if (player != null) {
        player.RemoveInteractible(this);
      }

      base.OnDestroy();
    }

    protected void OnTriggerEnter2D(Collider2D other) {
      if (other.CompareTag("Player") && interactibleArea.IsTouching(other)) {
        player = other.GetComponent<PlayerCharacter>();
        TryRegister();
        player.AddInteractible(this);
        OnEnterArea?.Invoke();
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
        OnExitArea?.Invoke();
      }
    }

    public void Inject(Collider2D collider) {
      this.col = collider;
    }

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

    public void PullTriggerEnter2D(Collider2D col) {
      OnTriggerEnter2D(col);
    }

    public void PullTriggerStay2D(Collider2D col) {}

    public void PullTriggerExit2D(Collider2D col) {
      OnTriggerExit2D(col);
    }
  }
}
