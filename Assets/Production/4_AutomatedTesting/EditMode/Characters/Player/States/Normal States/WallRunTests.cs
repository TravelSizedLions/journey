using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using HumanBuilders;



namespace HumanBuilders.Tests {
  public class WallRunTests: PlayerStateTest<WallRun> {

    [Test]
    public void WallRun_Can_WallJump() {
      SetupTest();
      
      player.PressedJump().Returns(true);

      state.OnUpdate();

      AssertStateChange<WallJump>();
    }

    [Test]
    public void WallRun_Can_WallSlide() {
      SetupTest();

      player.GetHorizontalInput().Returns(0);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(true);
      player.IsFalling().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<WallSlide>();
    }

    [Test]
    public void WallRun_Can_RiseAway() {
      SetupTest();
      
      physics.Vy = 1;

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);

      state.OnFixedUpdate();
      
      AssertStateChange<SingleJumpRise>();
    }

    [Test]
    public void WallRun_Can_FallAway() {
      SetupTest();

      physics.Vy = -1;

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);

      state.OnFixedUpdate();
      
      AssertStateChange<SingleJumpFall>();
    }

    [Test]
    public void WallRun_Can_StartAscension() {
      SetupTest();

      settings.WallRunBuffer = 1f;
      settings.WallRunBoost = 10f;
      settings.WallRunAscensionTime = 1f;
      state.OnStateAdded();

      physics.Vy = 0;
      
      player.IsTouchingGround().Returns(false);
      player.DistanceToGround().Returns(0.5f);
      
      state.OnStateEnter();
      
      Assert.AreEqual(settings.WallRunBoost, physics.Vy);
    }

    [Test]
    public void WallRun_Can_Ascend() {
      SetupTest();

      settings.WallRunBuffer = 1f;
      settings.WallRunBoost = 10f;
      settings.WallRunAscensionTime = 1f;
      settings.WallRunSpeed = 50;
      state.OnStateAdded();

      physics.Vy = 0;
      
      player.IsTouchingGround().Returns(false);
      player.DistanceToGround().Returns(0.5f);
      
      state.OnStateEnter();

      player.GetHorizontalInput().Returns(0);
      player.CanMove().Returns(true);
      player.IsTouchingLeftWall().Returns(true);
      player.IsFalling().Returns(false);

      player.HoldingJump().Returns(true);

      bool ascending = state.Ascend();
      
      Assert.True(ascending);
    }

    [Test]
    public void WallRun_Can_EndAscension() {
      SetupTest();

      settings.WallRunBuffer = 1f;
      settings.WallRunBoost = 10f;
      settings.WallRunAscensionTime = 1f;
      settings.WallRunSpeed = 50;
      state.OnStateAdded();

      physics.Vy = 0;
      
      player.IsTouchingGround().Returns(false);
      player.DistanceToGround().Returns(0.5f);
      
      state.OnStateEnter();

      player.GetHorizontalInput().Returns(0);
      player.CanMove().Returns(true);
      player.IsTouchingLeftWall().Returns(true);
      player.IsFalling().Returns(false);

      player.HoldingJump().Returns(false);

      bool ascending = state.Ascend();
      
      Assert.False(ascending);
    }

    [Test]
    public void WallRun_Ascension_Expires() {
      SetupTest();

      settings.WallRunBuffer = 1f;
      settings.WallRunBoost = 10f;
      settings.WallRunAscensionTime = 0f;
      settings.WallRunSpeed = 50;
      state.OnStateAdded();

      physics.Vy = 0;
      
      player.IsTouchingGround().Returns(false);
      player.DistanceToGround().Returns(0.5f);
      
      state.OnStateEnter();

      player.GetHorizontalInput().Returns(0);
      player.CanMove().Returns(true);
      player.IsTouchingLeftWall().Returns(true);
      player.IsFalling().Returns(false);

      player.HoldingJump().Returns(true);

      bool ascending = state.Ascend();
      
      Assert.False(ascending);
    }
  }
}