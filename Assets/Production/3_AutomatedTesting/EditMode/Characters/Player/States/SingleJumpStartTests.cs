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
      
      settings.IdleThreshold = 0;
      settings.MaxSpeed = 10;
      settings.Acceleration = 1;
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
      
      settings.SingleJumpForce = 16f;
      physics.Vy = 0;

      state.OnStateExit();

      Assert.AreEqual(settings.SingleJumpForce, physics.Vy);
    }

    [Test]
    public void SJumpStart_Can_Rise() {
      SetupTest();

      state.OnSingleJumpFinished(); 
      
      AssertStateChange<SingleJumpRise>();
    }
  }
}