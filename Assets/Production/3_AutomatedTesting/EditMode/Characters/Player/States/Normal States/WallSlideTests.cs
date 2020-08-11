using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;
using Storm.Characters;

namespace Tests.Characters.Player {
  public class WallSlideTests : PlayerStateTest<WallSlide> {

    [Test]
    public void WallSlide_Can_WallJump() {
      SetupTest();
      
      player.PressedJump().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<WallJump>();
    }


    [Test]
    public void WallSlide_Can_Fall() {
      SetupTest();

      player.GetHorizontalInput().Returns(0);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false); 
    
      state.OnFixedUpdate();

      AssertStateChange<SingleJumpFall>();
    }

    [Test]
    public void WallSlide_Can_TransitionToIdle() {
      SetupTest();

      state.SetWallFacing(Facing.Left);

      player.GetHorizontalInput().Returns(0);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(true);
      player.IsTouchingGround().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<Idle>();
    }

    [Test]
    public void WallSlide_SlowsFall() {
      SetupTest();
      
      settings.WallSlideDeceleration = 0.5f;
      state.OnStateAdded();
      state.OnStateEnter();
      state.SetWallFacing(Facing.Left);

      physics.Velocity = new Vector2(0, -10f);

      player.GetHorizontalInput().Returns(0);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(true);
      player.IsTouchingGround().Returns(false);

      state.OnFixedUpdate();

      Assert.AreEqual(-5f, physics.Vy);
    }

    [Test]
    public void WallSlide_SlowsFall_With_LeftInput() {
      SetupTest();
      
      settings.WallSlideDeceleration = 0.5f;
      state.OnStateAdded();
      state.OnStateEnter();
      state.SetWallFacing(Facing.Left);

      physics.Velocity = new Vector2(-100, 0);
      player.GetHorizontalInput().Returns(-1);
      player.CanMove().Returns(true);

      player.IsTouchingLeftWall().Returns(true);
      player.IsTouchingGround().Returns(false);

      state.OnFixedUpdate();

      Assert.AreEqual(0f, physics.Vx);
    }

    [Test]
    public void WallSlide_SlowsFall_With_RightInput() {
      SetupTest();
      
      settings.WallSlideDeceleration = 0.5f;
      state.OnStateAdded();
      state.OnStateEnter();
      state.SetWallFacing(Facing.Right);

      physics.Velocity = new Vector2(100, 0);
      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);

      player.IsTouchingRightWall().Returns(true);
      player.IsTouchingGround().Returns(false);

      state.OnFixedUpdate();

      Assert.AreEqual(0f, physics.Vx);
    }
  }
}