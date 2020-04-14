using UnityEngine;
using Storm.LevelMechanics.LiveWire;
using Storm.Attributes;
using Storm.UI;

namespace Storm.Characters.Player {

  /// <summary>
  /// The player character's movement mode when being prepared to get shot from
  /// a Spark Launcher. The player chooses the direction of launch with directional
  /// input, and the magnitude of launch by holding down the space bar.
  /// </summary>
  public class AimLiveWireMovement : PlayerBehavior {
  
    #region Variables
    #region UI stuff
    [Header("Indicator", order=0)]
    [Space(5, order=1)]

    //-------------------------------------------------------------------------
    // UI Stuff
    //-------------------------------------------------------------------------
    
    [Tooltip("The prefab used to show which direction the player will launch.")]
    /// <summary>
    /// The prefab used to show which direction the player will launch.
    /// </summary>
    public GameObject LaunchArrowPrefab;
    
    /// <summary>
    /// The actual instance of the lauch direction indicator.
    /// </summary>
    private ChargingArrow launchArrow;

    [Space(15, order=2)]
    #endregion UI stuff

    #region Launch Parameters
    [Header("Launch Parameters", order=3)]
    //-------------------------------------------------------------------------
    // Launch Parameters
    //-------------------------------------------------------------------------

    [Tooltip("How fast the player gravitates towards the launch position. Value between 0 (no attraction) and 1 (snaps to position).")]
    /// <summary>
    /// How fast the player gravitates towards the launch position. Value between 0 (no attraction) and 1 (snaps to position).
    /// </summary>
    [Range(0,1)]
    public float LaunchPadGravitation = 0.2f;

    [Tooltip("The slowest the player can be launched.")]
    /// <summary>
    /// The slowest the player can be launched.
    /// </summary>
    public float MinLaunchSpeed = 16.0f;

    [Tooltip("The fastest the player can be launched.")]
    /// <summary>
    /// The fastest the player can be launched.
    /// </summary>
    public float MaxLaunchSpeed = 60.0f;
    
    [Tooltip("The amount of time it takes to charge to max launch velocity (in seconds).")]
    /// <summary>
    /// The amount of time it takes to charge to max launch velocity (in seconds).
    /// </summary>
    public float MaxChargeTime = 1.0f;
    
    
    /// <summary>
    /// The player launches from this position. When Jerrod first enters this mode,
    /// he gravitates towards this position.
    /// </summary>
    private Vector3 launchPosition;

    /// <summary>
    /// How quickly the player can rotate the launch direction in degrees per physics tick.
    /// </summary>
    [Tooltip("How quickly the player can rotate the launch direction in degrees per physics tick.")]
    public float AimingSpeed = 2.0f;

    /// <summary>
    /// The angle that Jerrod will be launched at in degrees.
    /// </summary>
    [Tooltip("The angle of launch.")]
    [SerializeField]
    [ReadOnly]
    private float angle;

    [Space(15, order=4)]
    #endregion Launch Parameters
    
    #region Appearance Parameters
    [Header("Appearance Parameters", order=5)]
    [Space(5, order=6)]

    /// <summary> 
    /// The scaling factor of the spark visual (in both x and y directions)
    /// </summary>
    [Tooltip("The scaling factor of the spark visual")]
    public float SparkSize = 0.8f;

    /// <summary> 
    /// The scaling vector calculated from sparkSize. 
    /// </summary>
    private Vector2 sparkScale;

    /// <summary> 
    /// Used to save and restore the player's BoxCollider2D dimensions when entering/exiting this PlayerBehavior. 
    /// </summary>
    private Vector2 oldColliderSize;

    /// <summary>
    /// Used to save and restore the player's BoxCollider2D offsets when entering/exiting this PlayerBehavior. 
    /// </summary>
    private Vector2 oldColliderOffset;

    [Space(15, order=7)]
    #endregion

    #region Input Leniency
    [Header("Input Leniency", order=11)]
    [Space(5, order=12)]


    /// <summary>
    /// How long to delay registering player input on activation.
    /// </summary>
    [Tooltip("How long to delay registering player input on activation.")]
    public float InputWaitTime = 0.5f;

      
    /// <summary>
    /// Timer used to momentarily delay registering player input.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private float inputWaitTimer = 0;
    #endregion

    #region Input Flags
    //-------------------------------------------------------------------------
    // Input Flags
    //-------------------------------------------------------------------------
    [Header("Input flags", order=8)]
    [Space(5, order=9)]

    /// <summary> 
    /// Is the player holding the left direction? 
    /// </summary>
    [Tooltip("Is the player holding the left direction?")]
    [SerializeField]
    [ReadOnly]
    private bool left;



    /// <summary> 
    /// Is the player holding the right direction? 
    /// </summary>
    [Tooltip("Is the player holding the right direction?")]
    [SerializeField]
    [ReadOnly]
    private bool right;

    
    /// <summary> 
    /// Is the player holding the space bar?
    /// </summary>
    [Tooltip("Is the player holding the space bar?")]
    [SerializeField]
    [ReadOnly]
    private bool spaceHeld;

    
    /// <summary> 
    /// Did the player release the space bar? 
    /// </summary>
    [Tooltip("Did the player release the space bar?")]
    [SerializeField]
    [ReadOnly]
    private bool spaceReleased;


    #endregion Input Flags
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Fires when the game object is first initialized, before the game actually begins.
    /// The game object does not need to be enabled for Awake to fire.
    ///
    /// Use this instead of the constructor (Think of it as Unity's constructor method).
    /// The Awake() method on all game objects will fire before any Start() method does.
    /// </summary>
    protected override void Awake() {
      base.Awake();
      sparkScale = new Vector2(SparkSize, SparkSize);
    }
    
    
    /// <summary>
    /// Fires the first time the game object is enabled, and after all game objects have fired Awake().
    /// </summary>
    private void Start() {

    }
    
    /// <summary>
    /// Called once per frame of the game.
    ///
    /// Important Note: Input methods must be used here. The flags Unity sets in the background go stale at 
    /// the end of the frame, so putting Input collection anywhere else results in a race condition.
    ///
    /// Another Important Note: Don't do any physics calculations here. That results in the game's physics
    /// being dependent on the game's framerate. Use FixedUpdate() instead.
    /// </summary>
    private void Update() {

      gatherInputs();

      updateIndicator();
      // Direction chosen, preparing to launch.
      
      // Give the player a second to breath.
      if (inputWaitTimer > 0) {
        inputWaitTimer -= Time.deltaTime;
        
      } else if (spaceHeld) {
        // Charge for launch!

        launchArrow.Charge(Time.deltaTime);
        //chargingTimer = (chargingTimer >= maxChargeTime) ? maxChargeTime : chargingTimer + Time.deltaTime;
      } else if (spaceReleased) {
        // SpaceReleased
        
        // Calculate initial launch velocity.
        float percentCharged = launchArrow.GetChargePercentage(); 

        float launchRange = MaxLaunchSpeed-MinLaunchSpeed;

        float magnitude = MinLaunchSpeed + percentCharged*launchRange;

        float rads = Mathf.Deg2Rad*angle;
        Vector3 launchVelocity = new Vector2(Mathf.Cos(rads), Mathf.Sin(rads))*magnitude;
        
        // Fire that sucker into the air.
        player.SwitchBehavior(PlayerBehaviorEnum.BallisticLiveWire);
        player.ballisticLiveWireMovement.SetInitialVelocity(launchVelocity);
      }
        
      
    }
    
    /// <summary> 
    /// Collect player inputs (directional and spacebar). 
    /// </summary>
    private void gatherInputs() {    
      spaceHeld = Input.GetKey(KeyCode.Space);
      
      spaceReleased = Input.GetKeyUp(KeyCode.Space);
      if (spaceReleased) return;
      
      float Haxis = Input.GetAxis("Horizontal");

      left = Haxis > 0;
      right = Haxis < 0;
    }
    

    /// <summary>
    /// Update the rotation of the launch indicator.
    /// </summary>
    private void updateIndicator() {
      launchArrow.transform.eulerAngles = Vector3.forward*angle;
    }
    
    // /// <summary>
    // /// Set the rotation of the launch direction indicator
    // /// </summary>
    // public void SetLaunchIndicatorRotation(Vector2 direction) {
    //   launchIndicator.transform.rotation.SetLookRotation(direction,Vector2.up);
    // }
    
    /// <summary>
    /// Framerate independent updates (i.e., reliably fires every X milliseconds).
    /// Do all of your physics calculations here.
    /// </summary>
    private void FixedUpdate() {
      if (launchPosition != null) {
        updateLaunchRotation();
        Vector3 curPos = transform.position;

        player.transform.position = curPos*(1-LaunchPadGravitation) + launchPosition*LaunchPadGravitation;
      }
    }

    /// <summary>
    /// Update Jerrod's launch direction based on the player's directional input.
    /// </summary>
    private void updateLaunchRotation() {
      if (left) {
        angle = (angle - AimingSpeed)%360;
      } else if (right) {
        angle = (angle + AimingSpeed)%360;
      }

      player.isFacingRight = (angle < 90 && angle > -90) || angle > 270 || angle < -270;
      anim.SetBool("IsFacingRight", player.isFacingRight);
    }
    #endregion Unity Interface
    
    #region Player Behavior API
    //-------------------------------------------------------------------------
    // Player Behavior Interface
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Called every time the player switches to this movement mode.
    /// </summary>
    public override void Activate() {
      if (!enabled) {
        base.Activate();

        // Reset animator
        foreach(var param in anim.parameters) {
          anim.SetBool(param.name, false);
        }
        anim.SetBool("LiveWire", true);

        gameObject.layer = LayerMask.NameToLayer("LiveWire");

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        transform.localScale = sparkScale;
        oldColliderOffset = collider.offset;
        oldColliderSize = collider.size;
        collider.offset = Vector2.zero;
        collider.size = Vector2.one;

        inputWaitTimer = InputWaitTime;
      }
    }
    
    
    /// <summary>
    /// Called every time the player switches away from this movement mode.
    /// </summary>
    public override void Deactivate() {
      if (enabled) {
        base.Deactivate();

        anim.SetBool("LiveWire", false);

        TryRemoveLaunchIndicator();
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;

        gameObject.layer = LayerMask.NameToLayer("Player");

        transform.localScale = Vector2.one;
        collider.offset = oldColliderOffset;
        collider.size = oldColliderSize;
      }
    }
    #endregion

    #region Public Interface
    //-----------------------------------------------------------------------//
    // Public Interface
    //-----------------------------------------------------------------------//

    /// <summary>
    /// Sets the transform position that the player should launch from.
    /// This is usually the position of the Spark Launcher game object.
    /// </summary>
    public void SetLaunchPosition(Vector3 position) {
      launchPosition = position;
      TryAddLaunchIndicator();
    }

    /// <summary> 
    /// Add the directional indicator to the launch position.
    /// </summary>
    public void TryAddLaunchIndicator() {
      if (launchArrow == null) {
        launchArrow = Instantiate(LaunchArrowPrefab, launchPosition, Quaternion.identity).GetComponent<ChargingArrow>();
        launchArrow.SetMaxCharge(MaxChargeTime);
      }
    }
    
    /// <summary>
    /// Remove the launch direction indicator, if it exists.
    /// </summary>
    public void TryRemoveLaunchIndicator() {
      if (launchArrow != null) {
        Destroy(launchArrow.gameObject);
        launchArrow = null;
      }
    }
    #endregion

  }

}