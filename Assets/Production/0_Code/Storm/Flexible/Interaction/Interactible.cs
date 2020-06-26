using Storm.Attributes;
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Flexible.Interaction {
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
      get { return collider != null ? (Vector2)collider.bounds.center : Vector2.positiveInfinity; }
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

    #endregion

    #region Fields
    /// <summary>
    /// The interaction indicator to use for this interactive object.
    /// </summary>
    [Tooltip("The interaction indicator to use for this interactive object.")]
    [SerializeField]
    protected Indicator indicator;


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

    /// <summary>
    /// The area the player needs to be within in order to interact with this object.
    /// </summary>
    protected Collider2D interactibleArea;

    /// <summary>
    /// The physical collider for the object.
    /// </summary>
    protected new Collider2D collider;

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
    private void Awake() {
      player = FindObjectOfType<PlayerCharacter>();
      Collider2D[] cols = GetComponents<Collider2D>();
      
      if (cols[0].isTrigger) {
        interactibleArea = cols[0];
        collider = cols[1];
      } else {
        collider = cols[0];
        interactibleArea = cols[1];
      }

      Physics2D.IgnoreCollision(collider, player.GetComponent<BoxCollider2D>());
    }

    private void OnTriggerEnter2D(Collider2D other) {
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


    private void OnTriggerExit2D(Collider2D other) {
      if (other.CompareTag("Player")) {
        player.RemoveInteractible(this);
        player = null;
      }
    }

    #endregion

    public void Inject(Collider2D collider) {
      this.collider = collider;
    }

    public abstract void OnInteract();

    public abstract bool ShouldShowIndicator();
    
  }

}