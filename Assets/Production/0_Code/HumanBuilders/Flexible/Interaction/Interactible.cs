
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace HumanBuilders {
  /// <summary>
  /// The base class for objects the player can interact with in the environment.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public abstract class Interactible : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------

    /// <summary>
    /// Whether or not the player is still interacting with this object.
    /// </summary>
    [PropertyTooltip("Whether or not the player is still interacting with this object.")]
    [ShowInInspector]
    [ReadOnly]
    public bool StillInteracting {
      get { return interacting; }
    }


    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    protected IPlayer player;

    /// <summary>
    /// Whether the interaction with this object is still going on.
    /// </summary>
    protected bool interacting;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    protected virtual void Awake() {
      player = FindObjectOfType<PlayerCharacter>();
    }
    
    protected virtual void OnDestroy() {
      interacting = false;
    }


    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------


    /// <summary>
    /// Force an interaction to end. 
    /// </summary>
    /// <remarks>
    /// This is not meant to do anything special (e.g., it doesn't cause a dialog to
    /// suddenly end, or put down a carriable item). This method simply marks
    /// the interactible as no longer being interacted with.
    /// </remarks>
    public void EndInteraction() {
      interacting = false;
    }

    /// <summary>
    /// What the object should do when interacted with.
    /// </summary>
    /// <seealso cref="Carriable.OnInteract" />
    /// <seealso cref="TransitionDoor.OnInteract" />
    /// <seealso cref="Talkative.OnInteract" />
    public abstract void OnInteract();

  }

}
