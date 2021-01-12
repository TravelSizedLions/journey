using UnityEngine;
using NUnit.Framework;
using NSubstitute;
using System;

namespace HumanBuilders.Tests {
  public class StateDriverTests {

    PlayerCharacter player;

    FiniteStateMachine fsm;

    StateDriver driver;

    private void SetupTest() {
      player = TestingTools.ConstructPlayer();
      fsm = player.GetComponent<FiniteStateMachine>();
      driver = StateDriver.For<Idle>(); 
    }


    [Test]
    public void For_Null_Type_Returns_Null() {
      Assert.IsNull(StateDriver.For((Type)null));
    }

    [Test]
    public void For_Type_Handed_Type_Returned() {
      Assert.True(StateDriver.For<Idle>().GetType().IsAssignableFrom(typeof(StateDriver<Idle>)));
    }

    [Test]
    public void For_Abstract_Type_Returns_Null() {
      Assert.IsNull(StateDriver.For<State>());
    }

    [Test]
    public void For_Non_State_Returns_Null() {
      Assert.IsNull(StateDriver.For(typeof(StateDriverTests)));
    }

    [Test]
    public void StartMachine_Runs_After_Call() {
      SetupTest();

      driver.StartMachine(fsm);

      Assert.True(fsm.Running);
    }

    [Test]
    public void StartMachine_Contains_State() {
      SetupTest();
      driver.StartMachine(fsm);
      Assert.NotNull(fsm.GetComponentInChildren<Idle>());
    }

    [Test]
    public void StartMachine_Contains_Only_One_State() {
      SetupTest();
      driver.StartMachine(fsm);
      driver.StartMachine(fsm);
      driver.StartMachine(fsm);

      int num = fsm.GetComponentsInChildren<Idle>().Length ;
      Debug.Log(num);
      Assert.True(num == 1);
    }

    [Test]
    public void IsInState_InState_ReturnsTrue() {
      SetupTest();

      driver.StartMachine(fsm);

      Assert.True(driver.IsInState(fsm));
    }

    [Test]
    public void IsInState_NotInState_ReturnsFalse() {
      SetupTest();

      driver.StartMachine(fsm);
      StateDriver driverB = StateDriver.For<Running>();

      Assert.False(driverB.IsInState(fsm));
    }

    [Test]
    public void IsInState_InState_Not_Running_ReturnsTrue() {
      SetupTest();
      driver.StartMachine(fsm);
      fsm.Pause();
      Assert.True(driver.IsInState(fsm));
    }

    [Test]
    public void IsInState_NotInState_Not_Running_ReturnsFalse() {
      SetupTest();
      driver.StartMachine(fsm);
      fsm.Pause();
      StateDriver driverB = StateDriver.For<Running>();
      Assert.False(driverB.IsInState(fsm));
    }
  }

}