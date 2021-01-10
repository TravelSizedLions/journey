using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;


using HumanBuilders;


namespace HumanBuilders.Tests {
  public class IdleTests : PlayerStateTest<Idle> {


    [Test]
    public void Idle_Can_Jump() {
      SetupTest();

      // Inputs
      player.PressedJump().Returns(true);

      // Perform
      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }


    [Test]
    public void Idle_Can_Run() {
      SetupTest();

      player.TryingToMove().Returns(true);

      state.OnUpdate();

      AssertStateChange<Running>();
    }


    [Test]
    public void Idle_Can_Crouch() {
      SetupTest();

      player.HoldingDown().Returns(true);

      state.OnUpdate();

      AssertStateChange<CrouchStart>();
    }


    [Test]
    public void Idle_Can_WallRun() {
      SetupTest(); 

      player.PressedJump().Returns(true);
      player.IsTouchingLeftWall().Returns(true);

      state.OnUpdate();

      AssertStateChange<WallRun>();
    }

    [Test]
    public void Idle_Can_Fall() {
      SetupTest();

      player.IsTouchingGround().Returns(false);

      state.OnFixedUpdate(); 
      
      AssertStateChange<SingleJumpFall>();
    }

    [UnityTest]
    public IEnumerator Idle_Velocity_Zero() {
      SetupTest();

      physics.Awake();

      state.Inject(player, physics, settings);

      state.OnStateEnter();

      yield return null;

      Assert.AreEqual(physics.Velocity, new Vector2(0, 0));
    }
  }
}