using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;

namespace Tests.Characters.Player {
  public class DoubleJumpStartTests : PlayerStateTest<DoubleJumpStart> {

    [Test]
    public void DJumpStart_Can_Rise() {
      SetupTest();

      player.IsRising().Returns(true);

      state.OnDoubleJumpFinished();

      AssertStateChange<DoubleJumpRise>();
    }

    [Test]
    public void DJumpStart_Can_Fall() {
      SetupTest();

      player.IsRising().Returns(false);

      state.OnDoubleJumpFinished();

      AssertStateChange<DoubleJumpFall>();
    }

    [Test]
    public void DJumpStart_Can_StartRoll() {
      SetupTest();

      movementSettings.IdleThreshold = 0.1f;
      state.OnStateAdded();

      player.IsTouchingGround().Returns(true);
      physics.Vx = 1;

      state.OnFixedUpdate();

      AssertStateChange<RollStart>();
    }

    [Test]
    public void DJumpStart_Can_Land() {
      SetupTest();

      movementSettings.IdleThreshold = 1;
      state.OnStateAdded();

      player.IsTouchingGround().Returns(true);
      physics.Vx = 0.1f;

      state.OnFixedUpdate();

      AssertStateChange<Land>();
    }

    // [Test]
    // public void DJumpStart_Can_WallRun() {
    //   SetupTest();

    //   player.IsTouchingGround().Returns(false);
    //   player.IsTouchingLeftWall().Returns(true);
    //   player.IsRising().Returns(true);

    //   state.OnFixedUpdate();

    //   AssertStateChange<WallRun>();
    // }

    [Test]
    public void DJumpStart_Can_WallSlide() {
      SetupTest();

      player.IsTouchingGround().Returns(false);
      player.IsTouchingLeftWall().Returns(true);
      player.IsRising().Returns(false);

      state.OnFixedUpdate();

      AssertStateChange<WallSlide>();
    }

    [Test]
    public void DJumpStart_Can_Use_CoyoteTime() {
      SetupTest();

      player.PressedJump().Returns(true);
      player.InCoyoteTime().Returns(true);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void DJumpStart_Can_BufferedJump() {
      SetupTest();

      movementSettings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.PressedJump().Returns(true);
      player.InCoyoteTime().Returns(false);

      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(1);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void DJump_Actually_Jumps() {
      SetupTest();

      movementSettings.DoubleJumpForce = 10f;
      physics.Velocity = new Vector2(0, 0);

      state.OnStateEnter();

      Assert.AreEqual(10f, physics.Vy);
    }


    [Test]
    public void DJump_Disables_PlatformMomentum() {
      SetupTest();
      PlayerCharacter p = go.AddComponent<PlayerCharacter>();

      p.EnablePlatformMomentum();
      state.Inject(p, physics, carrySettings, movementSettings);

      state.OnStateEnter();

      Assert.False(p.IsPlatformMomentumEnabled());
    }
  }
}