# Storm.Subsystems.FSM
A framework for building agents that require a State Machine. To see how it's used, check out the [Player Character](https://github.com/hiltonjp/journey/tree/master/Assets/Production/0_Code/Storm/Characters/Player).

## The FiniteStateMachine Class
This class houses the state graph as well as the engine that runs it. Fortunately, leveraging the state machine inside another Monobehaviour is as simple as this:

```C#
/// <summary>
/// This could be a player character, enemy, NPC, or boss battle.
/// </summary>
public class Agent : MonoBehaviour {

  private FiniteStateMachine FSM;

  private void Start() {
    FSM = AddComponent<FiniteStateMachine>();
    State startState = AddComponent<StartState>();
    FSM.StartMachine(startState);
    
    // The finite state machine will run independently 
    // from this point forward.
  }
}

```

Things to know:
* Only one State in the machine will be enabled at a time. You can see which state is being used in the inspector by looking at which State is enabled.
* Multiple `FiniteStateMachines` can be running on the same class.
* States will be added dynamically to the game object as they're activated. 
* You can, but probably don't need to, inherit from this class.

## The State Class
All inheritors of the State class have access to the following methods for you to override:
* [OnStateAdded()](#OnStateAdded-and-OnStateAddedGeneral)
* [OnStateAddedGeneral()](#OnStateAdded-and-OnStateAddedGeneral)
* [OnUpdate()](#OnUpdate-and-OnFixedUpdate)
* [OnFixedUpdate()](#OnUpdate-and-OnFixedUpdate)
* [OnStateEnter()](#Changing-States)
* [OnStateExit()](#Changing-States)

There's no hard requirement for you to override all of these methods. They're just there if needed. All States also have access to the following properties:
* [AnimParam](#inheriting-from-the-state-class): The name of the animation trigger parameter associated with the state.
* FSM: A reference to the state machine the state belongs to, though likely you won't really need it.

Not to mention the method you'll likely be invoking most, `ChangeToState<S>()`{:.C#}, which triggers a state transition.

### Inheriting from the State class
Every state should be tied to a specific animation. When setting up a subclass of the State class, you'll need to specify an animation trigger parameter in Awake():

```C#
public Class ExampleState : State {

  // Fields
  ...

  private void Awake() {
    // This is what will be passed to 
    // Animator.SetTrigger() upon entering the state.
    AnimParam = "example_trigger";
  }
  
  // OnUpdate(), OnFixedUpdate(), etc.
  ...
}
```

Since every State has it's own animation and corresponding trigger parameter, there's no need to set up specific transitions in the animation controller for the state machine. In fact, doing so is more likely to lead to visual bugs. Instead, connect the corresponding animation to the controller's "Any State" node. That way, when you're wiring up the states through code, you can confidently make changes to state transitions without worrying about missing the corresponding change in the animation controller.

### OnStateAdded() and OnStateAddedGeneral()
The `OnStateAdded()` method is called the first time a state is entered. This will allow you to do just-in-time one time setup for the particular state. If there's one time setup that all states in the StateMachine share, create an intermediary state and implement `OnStateAddedGeneral()`, which will get called just before `OnStateAdded()`. Take a look at the [PlayerState](https://github.com/hiltonjp/journey/blob/master/Assets/Production/0_Code/Storm/Characters/Player/States/PlayerState.cs) class for an example of this.


### OnUpdate() and OnFixedUpdate()
The methods `OnUpdate()` and `OnFixedUpdate()` behave exactly as you would expect from their Unity counterparts, but are controlled by the FiniteStateMachine the State belongs to in order to prevent potential race conditions. It's for that reason that you should always use these methods within State classes.

```C#
public Class ExampleState : State {

  // Fields, Awake(), etc.
  ...

  public override void OnUpdate() {
    if (Input.GetAxis("Horizontal") == 0) {
      ChangeToState<NextState>();
    }
  }
  
  public override void OnFixedUpdate() {
    rigidbody.velocity = new Vector2(1f, 0f);
  }
  
  // OnStateAdded(), OnStateEnter(), OnStateExit(), etc.
  ...
}

```

### Changing States
Every state in the state machine can change to any other state through the `ChangeToState<TargetState>()` method. There are methods that fire before entering the state and after exiting to perform special setups/actions.

```C#
/// <summary>
/// An example state.
/// </summary>
public Class ExampleState : State {
  // Fields
  ...
  
  // Set up animation parameter in Awake()
  ...
  
  // This fires once per frame.
  public override void OnUpdate() {
    if (Input.GetButtonDown("Jump")) {
      ChangeToState<NextState>();
    }
  }
  
  // Fires every time you exit this state.
  public override void OnStateExit() {
    Debug.Log("Leaving State");
  }
}
  
  
/// <summary>
/// The next State to fire.
/// </summary>
public Class NextState : State {
  // Fires every time you enter this state.
  public override void OnStateEnter() {
    Debug.Log("Entering State!");
  }
}
```


