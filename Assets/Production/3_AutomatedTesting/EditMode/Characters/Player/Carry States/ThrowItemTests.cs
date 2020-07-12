using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using NSubstitute;
using Storm.Characters.Player;
using Storm.Characters;
using Storm.Flexible;
using Storm.Flexible.Interaction;

namespace Tests.Characters.Player {
  public class ThrowItemTests : PlayerStateTest<ThrowItem> {
    [Test]
    public void Can_Wall_Run() {
      SetupTest();
      
      player.PressedJump().Returns(true);
      player.IsTouchingLeftWall().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<WallRun>();
    }

    [Test]
    public void Can_Start_Single_Jump() {
      SetupTest();
      
      player.PressedJump().Returns(true);
      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      state.OnUpdate();
      
      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void Can_Dive() {
      SetupTest();
      
      player.PressedJump().Returns(false);
      player.HoldingDown().Returns(true);
      state.OnUpdate();
      
      AssertStateChange<Dive>();
    }

    [Test]
    public void Can_Idle() {
      SetupTest();
      
      movementSettings.IdleThreshold = 0.1f;
      state.OnStateAdded();

      player.Physics.Vx = 1f;

      player.GetHorizontalInput().Returns(0);
      for (int i = 0; i < 20; i++) {
        state.OnFixedUpdate();
      }
      
      AssertStateChange<Idle>();
    }

    [Test]
    public void Can_Single_Jump_Fall() {
      SetupTest();
      
      player.GetHorizontalInput().Returns(1);
      player.IsTouchingGround().Returns(false);
      player.IsFalling().Returns(true);
      state.OnFixedUpdate();
      
      AssertStateChange<SingleJumpFall>();
    }

    [Test]
    public void Applies_Force_Left() {
      SetupTest();

      carrySettings.ThrowForce = Vector2.one;
      carrySettings.VerticalThrowForce = 1f;

      Carriable c = BuildCarriable();
      player.CarriedItem = c;
      
      player.Physics.Velocity = Vector2.zero;
      player.Facing.Returns(Facing.Left);
      player.HoldingUp().Returns(false);

      state.OnStateEnter();

      Assert.AreEqual(new Vector2(-1, 1), c.Physics.Velocity);
    }

    [Test]
    public void Applies_Force_Right() {
      SetupTest();

      carrySettings.ThrowForce = Vector2.one;
      carrySettings.VerticalThrowForce = 1f;

      Carriable c = BuildCarriable();
      player.CarriedItem = c;
      
      player.Physics.Velocity = Vector2.zero;
      player.Facing.Returns(Facing.Right);
      player.HoldingUp().Returns(false);

      state.OnStateEnter();

      Assert.AreEqual(new Vector2(1, 1), c.Physics.Velocity);
    }

    [Test]
    public void Applies_Force_Up_Left() {
      SetupTest();

      player.Physics.Velocity = new Vector2(-1f, 0);
      carrySettings.ThrowForce = Vector2.one;
      carrySettings.VerticalThrowForce = 2f;
      state.OnStateAdded();

      Carriable c = BuildCarriable();
      player.CarriedItem = c;
      
      player.Facing.Returns(Facing.Left);
      player.HoldingUp().Returns(true);

      state.OnStateEnter();

      Assert.AreEqual(new Vector2(-1, 2), c.Physics.Velocity); 
    }

    [Test]
    public void Applies_Force_Up_Right() {
      SetupTest();

      player.Physics.Velocity = new Vector2(1f, 0);
      carrySettings.ThrowForce = Vector2.one;
      carrySettings.VerticalThrowForce = 2f;
      state.OnStateAdded();

      Carriable c = BuildCarriable();
      player.CarriedItem = c;
      
      player.Facing.Returns(Facing.Right);
      player.HoldingUp().Returns(true);

      state.OnStateEnter();

      Assert.AreEqual(new Vector2(1, 2), c.Physics.Velocity); 
    }

    [Test]
    public void Test_Can_Run() {
      SetupTest();
      
      state.OnThrowItemFinished();
      
      AssertStateChange<Running>();
    }
  }
}