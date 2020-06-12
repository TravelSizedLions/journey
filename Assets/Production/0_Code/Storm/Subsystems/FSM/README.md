# Storm.Subsystems.FSM
A framework for building agents that require a State Machine. To see how it's used, check out the [Player Character](https://github.com/hiltonjp/journey/tree/master/Assets/Production/0_Code/Storm/Characters/Player).


## The State Class
All inheritors of the State class have access to the following methods for you to override:
* [OnStateAdded()](### OnStateAdded() and OnStateAddedGeneral())
* [OnStateAddedGeneral()](### OnStateAdded() and OnStateAddedGeneral())
* [OnUpdate()](### OnUpdate() and OnFixedUpdate())
* [OnFixedUpdate()](### OnUpdate() and OnFixedUpdate())
* [OnStateEnter()](### Changing States)
* [OnStateExit()](### Changing States)

There's no hard requirement for you to override all of these methods. They're just there if needed. All States also have access to the following properties:
* `protected string AnimParam`: The name of the animation trigger parameter associated with the state.
* `protected IStateMachine FSM`: A reference to the state machine the state belongs to.

Not to mention the method you'll likely be invoking most, `ChangeToState<S>()`, which triggers a state transition.

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


