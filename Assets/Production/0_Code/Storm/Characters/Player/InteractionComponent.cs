using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    /// The indicator for the closest interactible.
    /// </summary>
    /// <seealso cref="InteractionComponent.ClosestIndicator" />
    Indicator ClosestIndicator { get; }

    /// <summary>
    /// The interactible that the player is currently closest to that he isn't
    /// already interacting with.
    /// </summary>
    /// <seealso cref="InteractionComponent.ClosestInteractible" />
    Interactible ClosestInteractible { get; }

    /// <summary>
    /// The interactible that the player is currently interacting with.
    /// </summary>
    /// <seealso cref="InteractionComponent.CurrentInteractible" />
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
    void AddInteractible(PhysicalInteractible interactible);

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <seealso cref="InteractionComponent.RemoveInteractible" />
    /// <param name="interactible">The object to remove.</param>
    void RemoveInteractible(PhysicalInteractible interactible);

    /// <summary>
    /// Interact with the given object.
    /// </summary>
    /// <param name="interactible">The object to interact with</param>
    /// <seealso cref="InteractionComponent.Interact" />
    void Interact(Interactible interactible);

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
    /// The indicator for the closest interactible.
    /// </summary>
    public Indicator ClosestIndicator {
      get { return closestIndicator; }
    }

    /// <summary>
    /// The interactible that the player is currently closest to in the scene.
    /// </summary>
    public Interactible ClosestInteractible { 
      get {
        if (closestInteractible != null &&
            closestInteractible.StillInteracting) {
          return closestInteractible;
        } else {
          return null;
        }
      }
    }

    /// <summary>
    /// The interactible the player is currently interacting with.
    /// </summary>
    [ShowInInspector]
    [PropertyTooltip("The interactible the player is currently interacting with.")]
    [Sirenix.OdinInspector.ReadOnly]
    public Interactible CurrentInteractible { 
      get { return currentInteractible; }
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
    [Sirenix.OdinInspector.ReadOnly]
    private static Indicator closestIndicator;

    /// <summary>
    /// The sprite renderer for the current interaction indicator.
    /// </summary>
    private static SpriteRenderer closestIndicatorSprite;
    #endregion

    #region Interactible Fields
    /// <summary>
    /// The list of interactible objects that are close enough to the player to
    /// be interacted with.
    /// </summary>
    private static List<PhysicalInteractible> interactibles;

    /// <summary>
    /// The interactive object that's currently the closest to the player. If
    /// this is null, there's no interactive object that's close enough to be
    /// interacted with.
    /// </summary>
    [SerializeField]
    [Sirenix.OdinInspector.ReadOnly]
    private static PhysicalInteractible closestInteractible;

    /// <summary>
    /// The closest interactible's sprite.
    /// </summary>
    private static SpriteRenderer closestInteractibleSprite;


    /// <summary>
    /// The interactible the player is currently interacting with.
    /// </summary>
    private static Interactible currentInteractible;
    #endregion
    #endregion

    #region Constructors
    private void Start() {
      player = GetComponent<PlayerCharacter>();
      playerCollider = player.GetComponent<Collider2D>();
      fsm = player.GetComponent<FiniteStateMachine>();
      playerSprite = player.GetComponent<SpriteRenderer>();

      indicators = new Dictionary<string, Indicator>();
      interactibles = new List<PhysicalInteractible>();
    }
    #endregion

    #region Unity API
    private void Update() {
      if (currentInteractible != null) {
        if (closestIndicatorSprite != null) {
          closestIndicatorSprite.enabled = false;
        }
        
        return;
      }

      if (interactibles.Count > 0) {
        PhysicalInteractible interactible = GetClosest();
        
        if (closestInteractible != interactible) {
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
    private PhysicalInteractible GetClosest() {
      if (interactibles.Count == 0) {
        return null;
      }

      PhysicalInteractible closest = interactibles[0];
      float closestDist = DistanceFromPlayer(closest);
      foreach(PhysicalInteractible next in interactibles) {
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
    private float DistanceFromPlayer(PhysicalInteractible interactible) {
      return (interactible.Center - (Vector2)playerCollider.bounds.center).magnitude;
    }
    #endregion


    /// <summary>
    /// Update whether or not the indicator should be visiable.
    /// </summary>
    private void UpdateIndicatorVisibility() {
      if (closestIndicatorSprite != null) {
        closestIndicatorSprite.enabled = closestInteractible.ShouldShowIndicator();
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
      Transform parent = closestInteractible.IndicateOverPlayer ? player.transform : closestInteractible.IndicatorTarget;

      // Set the current indicator.
      closestIndicator = GetIndicator(closestInteractible.IndicatorName);

      if (closestIndicator != null) {

        // Set the indicator on the proper parent.
        closestIndicator.transform.parent = parent;
        closestIndicator.transform.localPosition = Vector2.zero;

        // move the offset of the indicator if necessary.
        closestIndicator.transform.localPosition += (Vector3)closestInteractible.Offset;

        // make the indicator visible.
        closestIndicatorSprite = closestIndicator.GetComponent<SpriteRenderer>();
        closestIndicatorSprite.enabled = closestInteractible.ShouldShowIndicator();
      } else {
        // Reset 
        Debug.LogWarning("Indicator destroyed. Re-registering...");
        closestInteractible.Indicator.Instantiated = false;
        indicators.Remove(closestInteractible.IndicatorName);
        RegisterIndicator(closestInteractible.Indicator);

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
      if (closestIndicator != null) {
        closestIndicator.transform.parent = null;
        closestIndicator = null;
        closestIndicatorSprite.enabled = false;
        closestIndicatorSprite = null;
      }
    }
    #endregion

    #region Interactible Handling
    /// <summary>
    /// Update the current interactible information.
    /// </summary>
    /// <param name="interactible">The interactible to set.</param>
    private void UpdateCurrentInteractible(PhysicalInteractible interactible) {
      RemoveCurrentInteractible();
      SetCurrentInteractible(interactible);
    }
    
    /// <summary>
    /// Remove the current interactible.
    /// </summary>
    private static void RemoveCurrentInteractible() {
      if (closestIndicator != null) {
        closestInteractible = null;
        closestInteractibleSprite = null;
      }
    }

    /// <summary>
    /// Set information about the current interactible.
    /// </summary>
    /// <param name="interactible">The interactible to set.</param>
    private void SetCurrentInteractible(PhysicalInteractible interactible) {
      closestInteractible = interactible;
      closestInteractibleSprite = closestInteractible.GetComponent<SpriteRenderer>();
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
    void IInteractionComponent.AddInteractible(PhysicalInteractible interactible) => InteractionComponent.AddInteractible(interactible);
    public static void AddInteractible(PhysicalInteractible interactible) {
      interactibles.Add(interactible);
    }

    /// <summary>
    /// Remove an object from the list of objects the player is close enough to interact with.
    /// </summary>
    /// <param name="interactible">The object to remove.</param>
    void IInteractionComponent.RemoveInteractible(PhysicalInteractible interactible) => InteractionComponent.RemoveInteractible(interactible);
    public static void RemoveInteractible(PhysicalInteractible interactible) {
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
        Interact(closestInteractible);
      }
    }

    /// <summary>
    /// Interact with the given object.
    /// </summary>
    public void Interact(Interactible interactible) {
      if (currentInteractible == null) {
        currentInteractible = interactible;
      } 

      currentInteractible.OnInteract();

      if (!currentInteractible.StillInteracting) {
        currentInteractible = null;
      } else {
        fsm.Signal(currentInteractible.gameObject);
      }
    }

    /// <summary>
    /// Whether or not you can interact with the current object.
    /// </summary>
    private bool CanInteract() {
      if (currentInteractible == null) {

        Debug.Log("closestInteractible: " + closestInteractible != null);
        if (closestInteractible == null) {
          return false;
        }

        Debug.Log("closestIndicatorSprite: " + closestIndicatorSprite != null);

        if (closestIndicatorSprite == null) {
          return false;
        }

        Debug.Log("fsm: " + fsm != null);

        if (fsm == null) {
          return false;
        }

        Debug.Log("closest enabled: " + closestIndicatorSprite.enabled);
        return closestIndicatorSprite.enabled;
      } 
        
      Debug.Log("interacting: " + currentInteractible.StillInteracting);
      return currentInteractible.StillInteracting;
    }

    #endregion

  }
}