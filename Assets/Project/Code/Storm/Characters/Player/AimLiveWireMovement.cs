using UnityEngine;
using Storm.LevelMechanics.LiveWire;


namespace Storm.Characters.Player {

  /// <summary>
  /// The player character's movement mode when being prepared to get shot from
  /// a Spark Launcher. The player chooses the direction of launch with directional
  /// input, and the magnitude of launch by holding down the space bar.
  /// </summary>
  public class AimLiveWireMovement : PlayerMovement {
  
    #region UI stuff
    //-------------------------------------------------------------------------
    // UI Stuff
    //-------------------------------------------------------------------------
    
    [Tooltip("The prefab used to show which direction the player will launch.")]
    /// <summary>The prefab used to show which direction the player will launch.</summary>
    public GameObject launchIndicatorPrefab;
    
    /// <summary>The actual instance of the lauch direction indicator.</summary>
    private GameObject launchIndicator;
    #endregion UI stuff
  
  
  
    #region Launch Parameters
    //-------------------------------------------------------------------------
    // Launch Parameters
    //-------------------------------------------------------------------------
    
    [Tooltip("The fastest the player can be launched.")]
    /// <summary>The fastest the player can be launched.</summary>
    public float maxLaunchSpeed;
    
    [Tooltip("The amount of time it takes to charge to max launch velocity (in seconds).")]
    /// <summary>The amount of time it takes to charge to max launch velocity (in seconds).</summary>
    public float maxChargeTime;
    
    /// <summary>The amount of time the spacebar has been held.</summary>
    private float chargingTimer;
    
    [Tooltip("How fast the player gravitates towards the launch position. Value between 0 (no attraction) and 1 (snaps to position).")]
    /// <summary>
    /// How fast the player gravitates towards the launch position. Value between 0 (no attraction) and 1 (snaps to position).
    /// </summary>
    public float gravity;
    
    /// <summary>
    /// The player launches from this position. When Jerrod first enters this mode,
    /// he gravitates towards this position.
    /// </summary>
    private Vector3 launchPosition;
    
    /// <summary>
    /// The direction Jerrod will be launched.
    /// Controlled by player directional input.
    /// </summary>
    private Vector2 launchDirection;
    #endregion Launch Parameters
  
  
  
    #region Input Flags
    //-------------------------------------------------------------------------
    // Input Flags
    //-------------------------------------------------------------------------
    
    /// <summary> Which directions the player is holding down. </summary>
    private bool Up;
    private bool Down;
    private bool Left;
    private bool Right;
    private bool SpaceHeld;
    private bool SpaceReleased;
    #endregion Input Flags
  
  
  
  
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
    public void Awake() {
      
    }
    
    
    /// <summary>
    /// Fires the first time the game object is enabled, and after all game objects have fired Awake().
    /// </summary>
    public override void Start() {

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
    public void Update() {
      GatherInputs();
      // If the player is still choosing a direction...
      if (!(SpaceHeld || SpaceReleased)) {
        
        if (!(Up || Down || Left || Right)) {
          // No keys held down
          TryRemoveLaunchIndicator();
        } else {
          TryAddLaunchIndicator();
          UpdateLaunchDirection();
        } 
        
      } else {
        // Direction chosen, preparing to launch.
        
        if (SpaceHeld) {
          // Charge for launch!
          chargingTimer = (chargingTimer >= maxChargeTime) ? maxChargeTime : chargingTimer + Time.deltaTime;
        } else {
          // SpaceReleased
          
          // Calculate initial launch velocity.
          float percentCharged = (chargingTimer/maxChargeTime);
          float magnitude = percentCharged*maxLaunchSpeed;
          Vector2 launchVelocity = launchDirection*magnitude;
          
          // Fire that sucker into the air.
          player.SwitchMovement(PlayerMovementMode.BallisticLiveWire);
          player.ballisticLiveWireMovement.SetInitialVelocity(launchVelocity);
        }
        
      }
    }
    
    /// <summary> 
    /// Collect player inputs (directional & spacebar). 
    /// </summary>
    public void GatherInputs() {    
      SpaceHeld = Input.GetKey(KeyCode.Space);
      if (SpaceHeld) return;
      
      SpaceReleased = Input.GetKeyUp(KeyCode.Space);
      if (SpaceReleased) return;
      
      // Doing this instead of Input.GetKey() to future proof for porting.
      // Think Joystick vs. Arrow keys.
      float Haxis = Input.GetAxis("Horizontal");
      float Vaxis = Input.GetAxis("Vertical");
      
      Up = Vaxis > 0;
      Down = Vaxis < 0;
      Left = Haxis > 0;
      Right = Haxis < 0;
    }
    
    
    /// <summary>
    /// Update Jerrod's launch direction based on the player's directional input.
    /// </summary>
    public void UpdateLaunchDirection() {
      Vector2 oldDirection = launchDirection;
    
      // Rotate indicator based on input direction.
      if (Up) {
        launchDirection =   Directions2D.Up;
      } else if (Up && Right) {
        launchDirection = Directions2D.UpRight;
      } else if (Right) {
        launchDirection = Directions2D.Right;
      } else if (Down && Right) {
        launchDirection = Directions2D.DownRight;
      } else if (Down) {
        launchDirection = Directions2D.Down;
      } else if (Down && Left) {
        launchDirection = Directions2D.DownLeft;
      } else if (Left) {
        launchDirection = Directions2D.Left;
      } else { 
        launchDirection = Directions2D.UpLeft;
      }
      
      // Only bother to update indicator rotation if it actually changed.
      if (launchDirection != oldDirection) {
        SetLaunchIndicatorRotation(launchDirection);
      }
    }
    
    /// <summary> 
    /// Add the directional indicator to the launch position.
    /// </summary>
    public void TryAddLaunchIndicator() {
      if (launchIndicator == null) {
        launchIndicator = Instantiate(
          launchIndicatorPrefab, 
          launchPosition, 
          Quaternion.identity
        );
      }
    }
    
    /// <summary>
    /// Remove the launch direction indicator, if it exists.
    /// </summary>
    public void TryRemoveLaunchIndicator() {
      launchDirection = Vector3.zero;
      if (launchIndicator != null) {
        Destroy(launchIndicator);
        launchIndicator = null;
      }
    }
    
    /// <summary>
    /// Set the rotation of the launch direction indicator
    /// </summary>
    public void SetLaunchIndicatorRotation(Vector2 direction) {
      launchIndicator.transform.rotation.SetLookRotation(direction,Vector2.up);
    }
    
    /// <summary>
    /// Framerate independent updates (i.e., reliably fires every X milliseconds).
    /// Do all of your physics calculations here.
    /// </summary>
    public void FixedUpdate() {
      if (launchPosition != null) {
        Vector3 curPos = transform.position;
        transform.position = curPos*(1-gravity) + launchPosition*gravity;
      }
    }
    #endregion Unity API
    
    
    
    #region PlayerMovement API
    //-------------------------------------------------------------------------
    // PlayerMovement API
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Called every time the player switches to this movement mode.
    /// </summary>
    public override void Activate() {
      base.Activate();
      TryRemoveLaunchIndicator();
      chargingTimer = 0;
      rb.velocity = Vector2.zero;
      anim.SetBool("LiveWire", true);
    }
    
    
    /// <summary>
    /// Called every time the player switches away from this movement mode.
    /// </summary>
    public override void Deactivate() {
      base.Deactivate();
      TryRemoveLaunchIndicator();
      chargingTimer = 0;
      rb.velocity = Vector2.zero;
      anim.SetBool("LiveWire", false);
    }
    
    /// <summary>
    /// Sets the transform position that the player should launch from.
    /// This is usually the position of the Spark Launcher game object.
    /// </summary>
    public void SetLaunchPosition(Vector3 position) {
      launchPosition = position;
    }
    #endregion

  }

}