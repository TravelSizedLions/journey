using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

using HumanBuilders;

using UnityEngine;
using UnityEngine.TestTools;

namespace HumanBuilders.Tests {
  public class SingleJumpFallTests : PlayerStateTest<SingleJumpFall> {


    [Test]
    public void SJumpFall_Use_CoyoteTime() {
      SetupTest();
      
      player.PressedJump().Returns(true);
      player.InCoyoteTime().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void SJumpFall_Use_CoyoteTime_Called() {
      SetupTest();
      
      player.PressedJump().Returns(true);
      player.InCoyoteTime().Returns(true);

      state.OnUpdate();

      player.Received().UseCoyoteTime();
    }

    [Test]
    public void SJumpFall_Can_StartDoubleJump() {
      SetupTest();

      settings.GroundJumpBuffer = 1f;
      state.OnStateAdded();
      
      player.PressedJump().Returns(true);
      player.InCoyoteTime().Returns(false);

      player.DistanceToGround().Returns(2f);
      player.DistanceToWall().Returns(3f);

      state.OnUpdate();
      
      AssertStateChange<DoubleJumpStart>();
    }


    [Test]
    public void SJumpFall_Can_BufferedJump() {
      SetupTest();
      
      settings.GroundJumpBuffer =  1;
      state.OnStateAdded();

      player.PressedJump().Returns(true);
      player.InCoyoteTime().Returns(false);

      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(10);
      
      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }
    

    [Test]
    public void SJumpFall_Can_WallSlide() {
      SetupTest();

      player.IsTouchingLeftWall().Returns(true);

      state.OnFixedUpdate(); 
      
      AssertStateChange<WallSlide>();
    }


    [Test]
    public void SJumpFall_Can_StartRoll() {
      SetupTest();

      settings.IdleThreshold = 0;
      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      settings.RollOnLand = 0;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0, 0);

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<RollStart>();
    }

    [Test]
    public void SJumpFall_Can_StartCrouch_Moving() {
      SetupTest();
      
      settings.IdleThreshold = 0;
      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      settings.RollOnLand = 100;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0, 0);

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);

      player.HoldingDown().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<CrouchStart>();
    }

    [Test]
    public void SJumpFall_Can_Land_Moving() {
      SetupTest();
      
      settings.IdleThreshold = 0;
      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      settings.RollOnLand = 100;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0, 0);

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);

      player.HoldingDown().Returns(false);

      state.OnFixedUpdate();

      AssertStateChange<Land>();
    }


    [Test]
    public void SJumpFall_Can_Uncrouch_NotMoving() {
      SetupTest();

      settings.IdleThreshold = 100;
      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      settings.RollOnLand = 0;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0, 0);

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<CrouchEnd>();
    }

    [Test]
    public void SJumpFall_Can_StartCrouch_NotMoving() {
      SetupTest();
      
      settings.IdleThreshold = 100;
      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      settings.RollOnLand = 100;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0, 0);

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);

      player.HoldingDown().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<CrouchStart>();
    }

    [Test]
    public void SJumpFall_Can_Land_NotMoving() {
      SetupTest();
      
      settings.IdleThreshold = 100;
      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      settings.RollOnLand = 100;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0, 0);

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);

      player.HoldingDown().Returns(false);

      state.OnFixedUpdate();

      AssertStateChange<Land>();
    }
  }
}