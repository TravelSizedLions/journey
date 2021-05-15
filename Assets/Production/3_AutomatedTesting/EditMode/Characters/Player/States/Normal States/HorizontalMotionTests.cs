using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;


using HumanBuilders;



namespace HumanBuilders.Tests {

  // Using Running State, since HorizontalMotion is Abstract, and therefore
  // can't be added as a component.
  public class HorizontalMotionTests : PlayerStateTest<Running> {

    #region GetFacing Tests
    [Test]
    public void HMotion_GetFacing_Left() {
      SetupTest();

      settings.IdleThreshold = 0.2f;
      state.OnStateAdded();

      physics.Vx = -1;
      
      Facing facing = state.GetFacing();

      Assert.AreEqual(facing, Facing.Left);
    }

    [Test]
    public void HMotion_GetFacing_None_When_Slight_Left() {
      SetupTest();

      settings.IdleThreshold = 0.2f;
      state.OnStateAdded();

      physics.Vx = -0.1f;

      Facing facing = state.GetFacing();

      Assert.AreEqual(Facing.None, facing);
    }

    [Test]
    public void HMotion_GetFacing_None_When_Slight_Right() {
      SetupTest();

      settings.IdleThreshold = 0.2f;
      state.OnStateAdded();

      physics.Vx = 0.1f;

      Facing facing = state.GetFacing();

      Assert.AreEqual(Facing.None, facing);
    }

    [Test]
    public void HMotion_GetFacing_Right() {
      SetupTest();

      settings.IdleThreshold = 0.2f;

      state.OnStateAdded();

      physics.Vx = 1;

      Facing facing = state.GetFacing();

      Assert.AreEqual(facing, Facing.Right);
    }
    #endregion

    #region Buffered Jump Tests
    [Test]
    public void HMotion_BufferedJump_Too_Far_From_Floor() {
      SetupTest();

      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToGround().Returns(2);
      player.DistanceToWall().Returns(4);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(false, jumped);
    }

    [Test]
    public void HMotion_BufferedJump_Too_Far_From_Wall() {
      SetupTest();

      settings.WallJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToGround().Returns(4);
      player.DistanceToWall().Returns(2);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(false, jumped);
    }

    [Test]
    public void HMotion_BufferedJump_Close_To_Wall_Not_Moving() {
      SetupTest();

      settings.WallJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToWall().Returns(0.5f);
      player.DistanceToGround().Returns(4);
      player.GetHorizontalInput().Returns(0);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(false, jumped);
    }

    [Test]
    public void HMotion_BufferedJump_GroundJump_Clear() {
      SetupTest();

      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(10);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(true, jumped);
      AssertStateChange<SingleJumpStart>();
    }

    public void HMotion_BufferedJump_GroundJump_Close() {
      SetupTest();

      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(0.51f);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(true, jumped);
      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void HMotion_BufferedJump_WallJump_Clear() {
      SetupTest();

      settings.WallJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToGround().Returns(10);
      player.DistanceToWall().Returns(0.5f);
      player.GetHorizontalInput().Returns(1);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(true, jumped);
      AssertStateChange<WallJump>();
    }

    [Test]
    public void HMotion_BufferedJump_WallJump_Close() {
      SetupTest();

      settings.WallJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToGround().Returns(0.51f);
      player.DistanceToWall().Returns(0.5f);
      player.GetHorizontalInput().Returns(1);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(true, jumped);
      AssertStateChange<WallJump>();
    }
    #endregion

    #region Horizontal Motion Tests


    [Test]
    public void HMotion_TryingToMove_MovementDisabled_NoMotion() {
      SetupTest();

      settings.MaxSpeed = 1;
      settings.Deceleration = 0.5f;
      state.OnStateAdded();

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(false);
      player.IsTouchingGround().Returns(true);

      physics.Velocity = new Vector2(1, 0);

      state.MoveHorizontally();

      Assert.AreEqual(0.5f, physics.Vx);
    }

    #region Testing Player Transform Platform De-parenting
    [Test]
    public void HMotion_Check_CanMove_Left() {
      SetupTest();

      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      state.OnStateAdded();

      player.GetHorizontalInput().Returns(-1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(true);
      player.IsPlatformMomentumEnabled().Returns(false);

      player.Physics.Velocity = new Vector2(0, 0);

      state.MoveHorizontally();

      Assert.AreEqual(-10f, physics.Vx);
    }

    [Test]
    public void HMotion_Check_CanMove_Right() {
      SetupTest();

      settings.Acceleration = 1;
      settings.MaxSpeed = 10;
      state.OnStateAdded();

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(true);
      player.IsPlatformMomentumEnabled().Returns(false);

      player.Physics.Velocity = new Vector2(0, 0);

      state.MoveHorizontally();

      Assert.AreEqual(10f, physics.Vx);
    }


    [Test]
    public void HMotion_Check_Can_Accelerate() {
      SetupTest();

      settings.Acceleration = 0.5f;
      settings.MaxSpeed = 10;
      state.OnStateAdded();

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(true);
      player.IsPlatformMomentumEnabled().Returns(false);

      player.Physics.Velocity = new Vector2(0, 0);

      state.MoveHorizontally();

      Assert.AreEqual(5f, physics.Vx);
    }

    [Test]
    public void HMotion_Check_Agility() {
      SetupTest();

      settings.Acceleration = 0.1f;
      settings.Agility = 5;
      settings.MaxSpeed = 10;
      state.OnStateAdded();

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(true);
      player.IsPlatformMomentumEnabled().Returns(false);

      player.Physics.Velocity = new Vector2(0, 0);

      state.MoveHorizontally();

      player.GetHorizontalInput().Returns(-1);

      state.MoveHorizontally();

      Assert.AreEqual(-4f, physics.Vx);
    }

    [Test]
    public void HMotion_Check_MaxSpeed() {
      SetupTest();

      settings.Acceleration = 0.1f;
      settings.MaxSpeed = 1;
      state.OnStateAdded();

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(true);
      player.IsPlatformMomentumEnabled().Returns(false);

      player.Physics.Velocity = new Vector2(0, 0);

      for (int i = 0; i < 10; i++) {
        state.MoveHorizontally();
      }

      Assert.AreEqual(settings.MaxSpeed, physics.Vx);
    }
    #endregion
    #endregion
  }
}