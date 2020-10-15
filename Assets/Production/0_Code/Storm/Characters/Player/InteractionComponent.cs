using System.Collections.Generic;
using Storm.Attributes;
using Storm.Flexible.Interaction;
using Storm.Subsystems.FSM;
using UnityEngine;


namespace Storm.Characters.Player {

  /// <summary>
  /// Interface for the class responsible for handling player-environment interaciton.
  /// </summary>
  public interface IInteractionComponent {
    /// <summary>
    /// The current indicator.
    /// </summary>
    /// <seealso cref="InteractionComponent.CurrentIndicator" />
    Indicator CurrentIndicator { get; }

    /// <summary>
    /// The interactible that the player is currently interacting with.
    /// </summary>
    /// <seealso cref="CurrentInteractible.CurrentInteractble" />
    Interactible CurrentInteractible { get; }

    /// <summary>
    /// Register an indicator to the cache.
    /// </summary>
    /// <param name="indicator">The indicator prefab to register. The first time the indicator is
    /// actually used in the scene, the prefab will be instantiated and the
    /// instance will be cached in its place.</param>
    /// <seealso cref="InteractionComponent.RegisterIndicator" />
    void RegisterIndicator(Indicator indicator);

    /// <summary>
    /// Clear indicator cache.
    /// </summary>
    /// <seealso cref="InteractionComponent.ClearIndicators" />
    void ClearIndicators();

    /// <summary>
    /// Add an object to the list of objects the player is close enough to interact with.
    /// </summary>
    /// <seealso cref="InteractionComponent.AddInteractible" />
    /// <param name="interactible">The object to add.</param>
    void AddInteractible(Interactible interactible);

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <seealso cref="InteractionComponent.RemoveInteractible" />
    /// <param name="interactible">The object to remove.</param>
    void RemoveInteractible(Interactible interactible);

    /// <summary>
    /// Interact with the closest interactible object.
    /// </summary>
    /// <seealso cref="InteractionComponent.Interact" />
    void Interact();
  }

  /// <summary>
  /// The delegate component for dealing with environment interactions.
  /// </summary>
  public class InteractionComponent : MonoBehaviour, IInteractionComponent {

    #region Properties
    /// <summary>
    /// The current indicator.
    /// </summary>
    public Indicator CurrentIndicator {
      get { return currentIndicator; }
    }

    /// <summary>
    /// The interactible that the player is currently interacting with.
    /// </summary>
    public Interactible CurrentInteractible { 
      get {
        if (currentInteractible.StillInteracting) {
          return currentInteractible;
        } else {
          return null;
        }
      }
    }
    #endregion

    #region Fields
    #region Player information
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private PlayerCharacter player;

    /// <summary>
    /// The player's collider.
    /// </summary>
    private Collider2D playerCollider;

    /// <summary>
    /// The player's state machine.
    /// </summary>
    private FiniteStateMachine fsm;

    /// <summary>
    /// The player's sprite renderer
    /// </summary>
    private SpriteRenderer playerSprite;
    #endregion

    #region Indicator Fields
    /// <summary>
    /// A pool of indicator instances. Every interactible will try to register
    /// their indicator to this cache.
    /// </summary>
    private static Dictionary<string, Indicator> indicators;


    /// <summary>
    /// The currently active interaction indicator.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private static Indicator currentIndicator;

    /// <summary>
    /// The sprite renderer for the current interaction indicator.
    /// </summary>
    private static SpriteRenderer currentIndicatorSprite;
    #endregion

    #region Interactible Fields
    /// <summary>
    /// The list of interactible objects that are close enough to the player to
    /// be interacted with.
    /// </summary>
    private static List<Interactible> interactibles;

    /// <summary>
    /// The interactive object that's currently the closest to the player. If
    /// this is null, there's no interactive object that's close enough to be
    /// interacted with.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private static Interactible currentInteractible;

    /// <summary>
    /// The current interactible's sprite.
    /// </summary>
    private static SpriteRenderer currentInteractibleSprite;
    #endregion
    #endregion

    #region Constructors
    private void Start() {
      player = GetComponent<PlayerCharacter>();
      playerCollider = player.GetComponent<Collider2D>();
      fsm = player.GetComponent<FiniteStateMachine>();
      playerSprite = player.GetComponent<SpriteRenderer>();

      indicators = new Dictionary<string, Indicator>();
      interactibles = new List<Interactible>();
    }
    #endregion

    #region Unity API
    private void Update() {
      if (currentInteractible != null && currentInteractible.StillInteracting) {
        currentIndicatorSprite.enabled = false;
        return;
      }

      if (interactibles.Count > 0) {
        Interactible interactible = GetClosest();
        
        if (currentInteractible != interactible) {
          UpdateCurrentInteractible(interactible);
          // Debug.Log("Updating closest to \"" + interactible.name + ".\"");
          
          UpdateCurrentIndicator();
        }

        UpdateIndicatorVisibility();
      }
    }

    #region Searching Over Interactibles
    /// <summary>
    /// Retrieves the closest interactible object that the player is close enough to interact with.
    /// </summary>
    /// <returns>The closest interactible object.</returns>
    private Interactible GetClosest() {
      if (interactibles.Count == 0) {
        return null;
      }

      Interactible closest = interactibles[0];
      float closestDist = DistanceFromPlayer(closest);
      foreach(Interactible next in interactibles) {
        float dist = DistanceFromPlayer(next);
        if (dist < closestDist) {
          closest = next;
          closestDist = dist;
        }
      }

      return closest;
    }

    /// <summary>
    /// How far away an interactible is from the player.
    /// </summary>
    /// <param name="interactible">The interactible to check.</param>
    /// <returns>The distance from the center of the player to the center of the
    /// interactible.</returns>
    private float DistanceFromPlayer(Interactible interactible) {
      return (interactible.Center - (Vector2)playerCollider.bounds.center).magnitude;
    }
    #endregion


    /// <summary>
    /// Update whether or not the indicator should be visiable.
    /// </summary>
    private void UpdateIndicatorVisibility() {
      if (currentIndicatorSprite != null) {
        currentIndicatorSprite.enabled = currentInteractible.ShouldShowIndicator();
      }
    }

    #region Indicator Handling
    /// <summary>
    /// Update the current indicator information.
    /// </summary>
    private void UpdateCurrentIndicator() {
      RemoveCurrentIndicator();
      SetCurrentIndicator();
    }

    /// <summary>
    /// Set information about the current indicator.
    /// </summary>
    private void SetCurrentIndicator() {
      Transform parent = currentInteractible.IndicateOverPlayer ? player.transform : currentInteractible.IndicatorTarget;
      SpriteRenderer targetSprite = currentInteractible.IndicateOverPlayer ? playerSprite : currentInteractibleSprite;

      // Set the current indicator.
      currentIndicator = GetIndicator(currentInteractible.IndicatorName);

      if (currentIndicator != null) {

        // Set the indicator on the proper parent.
        currentIndicator.transform.parent = parent;
        currentIndicator.transform.localPosition = Vector2.zero;

        // move the offset of the indicator if necessary.
        currentIndicator.transform.localPosition += (Vector3)currentInteractible.Offset;

        // make the indicator visible.
        currentIndicatorSprite = currentIndicator.GetComponent<SpriteRenderer>();
        currentIndicatorSprite.enabled = currentInteractible.ShouldShowIndicator();
      } else {
        // Reset 
        Debug.LogWarning("Indicator destroyed. Re-registering...");
        currentInteractible.Indicator.Instantiated = false;
        indicators.Remove(currentInteractible.IndicatorName);
        RegisterIndicator(currentInteractible.Indicator);

        // Try again.
        UpdateCurrentIndicator();
      }
    }


    /// <summary>
    /// Get the instance of the indicator with a given name.
    /// </summary>
    /// <param name="name">The name of the indicator to get.</param>
    /// <returns>The indicator with the given name. Instantiates the indicator</returns>
    private Indicator GetIndicator(string name) {
      if (indicators.ContainsKey(name)) {
        Indicator indicator = indicators[name];

        if (indicator.Instantiated) {
          return indicator;
        }

        GameObject go = Instantiate(
          indicator.gameObject,
          Vector2.zero,
          Quaternion.identity
        );

        Indicator instanced = go.GetComponent<Indicator>();
        instanced.Instantiated = true;

        indicators.Remove(name);
        indicators.Add(name, instanced);

        return instanced;

      } 

      return null;
    }

    /// <summary>
    /// Remove the current indicator.
    /// </summary>
    private static void RemoveCurrentIndicator() {
      if (currentIndicator != null) {
        currentIndicator.transform.parent = null;
        currentIndicator = null;
        currentIndicatorSprite.enabled = false;
        currentIndicatorSprite = null;
      }
    }
    #endregion

    #region Interactible Handling
    /// <summary>
    /// Update the current interactible information.
    /// </summary>
    /// <param name="interactible">The interactible to set.</param>
    private void UpdateCurrentInteractible(Interactible interactible) {
      RemoveCurrentInteractible();
      SetCurrentInteractible(interactible);
    }
    
    /// <summary>
    /// Remove the current interactible.
    /// </summary>
    private static void RemoveCurrentInteractible() {
      if (currentIndicator != null) {
        currentInteractible = null;
        currentInteractibleSprite = null;
      }
    }

    /// <summary>
    /// Set information about the current interactible.
    /// </summary>
    /// <param name="interactible">The interactible to set.</param>
    private void SetCurrentInteractible(Interactible interactible) {
      currentInteractible = interactible;
      currentInteractibleSprite = currentInteractible.GetComponent<SpriteRenderer>();
    }
    #endregion
    #endregion

    #region Public Interface

    /// <summary>
    /// Register an indicator to the cache.
    /// </summary>
    /// <param name="indicator">The indicator prefab to register. The first time the indicator is
    /// actually used in the scene, the prefab will be instantiated and the
    /// instance will be cached in its place.</param>
    void IInteractionComponent.RegisterIndicator(Indicator indicator) => InteractionComponent.RegisterIndicator(indicator);
    public static void RegisterIndicator(Indicator indicator) {
      if (!indicators.ContainsKey(indicator.Name)) {
        indicators.Add(indicator.Name, indicator);
      }
    }

    /// <summary>
    /// Clear the indicator cache.
    /// </summary>
    void IInteractionComponent.ClearIndicators() => InteractionComponent.ClearIndicators();
    public static void ClearIndicators() {
      Debug.Log("Clearing indicators.");
      indicators.Clear();
      interactibles.Clear();
      RemoveCurrentIndicator();
      RemoveCurrentInteractible();
    }


    /// <summary>
    /// Add an object to the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to add.</param>
    void IInteractionComponent.AddInteractible(Interactible interactible) => InteractionComponent.AddInteractible(interactible);
    public static void AddInteractible(Interactible interactible) {
      interactibles.Add(interactible);
    }

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to remove.</param>
    void IInteractionComponent.RemoveInteractible(Interactible interactible) => InteractionComponent.RemoveInteractible(interactible);
    public static void RemoveInteractible(Interactible interactible) {
      interactibles.Remove(interactible);
      if (interactibles.Count == 0) {
        RemoveCurrentInteractible();
        RemoveCurrentIndicator();
      }
    }

    /// <summary>
    /// Interact with the closest interactible object.
    /// </summary>
    public void Interact() {
      if (CanInteract()) {
        currentInteractible.OnInteract();

        if (currentInteractible != null) {
          fsm.Signal(currentInteractible.gameObject);
        }
      }
    }

    /// <summary>
    /// Whether or not you can interact with the current object.
    /// </summary>
    private bool CanInteract() {
      if (currentInteractible == null) {
        return false;
      }

      if (currentIndicatorSprite == null) {
        return false;
      }

      if (fsm == null) {
        return false;
      }

      return currentInteractible.StillInteracting || currentIndicatorSprite.enabled;
    }

    #endregion
  }
}