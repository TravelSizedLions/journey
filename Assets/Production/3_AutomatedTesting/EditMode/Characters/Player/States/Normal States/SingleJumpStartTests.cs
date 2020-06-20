using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Storm.Characters.Player;
using Storm.Components;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Characters.Player {
  public class SingleJumpStartTests : PlayerStateTest<SingleJumpStart> {

    [Test]
    public void SJumpStart_Can_StartDoubleJump() {
      SetupTest();
      
      player.PressedJump().Returns(true);

      state.OnUpdate();

      AssertStateChange<DoubleJumpStart>();
    }

    [Test]
    public void SJumpStart_Can_Move_InState() {
      SetupTest();
      
      movementSettings.IdleThreshold = 0;
      movementSettings.MaxSpeed = 10;
      movementSettings.Acceleration = 1;
      state.OnStateAdded();

      physics.Velocity = Vector2.zero;

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(true);

      state.OnFixedUpdate();

      Assert.Greater(physics.Vx, 0);
    }

    [Test]
    public void SJumpStart_Applies_Jump_On_Exit() {
      SetupTest();
      
      movementSettings.SingleJumpForce = 16f;
      physics.Vy = 0;

      state.OnStateExit();

      Assert.AreEqual(movementSettings.SingleJumpForce, physics.Vy);
    }

    [Test]
    public void SJumpStart_Can_Rise() {
      SetupTest();

      state.OnSingleJumpFinished(); 
      
      AssertStateChange<SingleJumpRise>();
    }
  }
}