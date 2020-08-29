using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;
using Storm.Subsystems.FSM;

namespace Tests.Subsystems.FSM {

  public class StateTest<S> where S : State {

    protected GameObject go;

    protected IStateMachine FSM;

    protected S state;

    protected virtual void SetupTest() {
      FSM = Substitute.For<IStateMachine>();

      go = new GameObject();
      state = go.AddComponent<S>();
      
      state.Inject(FSM);
    }

    /// <summary>
    /// Asserts that the player received a request to change to the provided state
    /// </summary>
    /// <typeparam name="NextState">The expected state transition</typeparam>
    protected void AssertStateChange<NextState>() where NextState : State {
      FSM.Received().OnStateChange(Arg.Any<State>(), Arg.Any<NextState>());
    }

    /// <summary>
    /// Asserts that the player did not receive a request to change to the provided state
    /// </summary>
    /// <typeparam name="NextState">The expected state transition</typeparam>
    protected void AssertNoStateChange<NextState>() where NextState : State {
      FSM.DidNotReceive().OnStateChange(Arg.Any<State>(), Arg.Any<NextState>());
    }
  }
}