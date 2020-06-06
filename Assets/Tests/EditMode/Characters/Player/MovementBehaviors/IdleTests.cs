using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;

namespace Tests.Characters.Player {
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

    [UnityTest]
    public IEnumerator Idle_Velocity_Zero() {
      SetupTest();

      physics.Awake();

      state.Inject(player, physics);

      state.OnStateEnter();

      yield return null;

      Assert.AreEqual(physics.Velocity, new Vector2(0, 0));
    }
  }
}