using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Storm.Characters.Player;
using Storm.Services;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Characters.Player {
  public class DoubleJumpFallTests : PlayerStateTest<DoubleJumpFall> {

    #region  Unit Tests
    [Test]
    public void DJumpFall_Can_WallSlide() {
      SetupTest();

      player.IsTouchingLeftWall().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<WallSlide>();
    }

    [Test]
    public void DJumpFall_Can_StartRoll() {
      SetupTest();

      settings.IdleThreshold = 1;
      state.OnStateAdded();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);
      physics.Velocity = new Vector2(2, 0);

      state.OnFixedUpdate();

      AssertStateChange<RollStart>();
    }

    [Test]
    public void DJumpFall_Can_StartCrouch() {
      SetupTest();

      settings.IdleThreshold = 1;
      state.OnStateAdded();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);
      physics.Velocity = new Vector2(0.5f, 0);
      player.HoldingDown().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<CrouchStart>();
    }

    [Test]
    public void DJumpFall_Can_Land() {
      SetupTest();

      settings.IdleThreshold = 1;
      state.OnStateAdded();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsTouchingGround().Returns(true);
      physics.Velocity = new Vector2(0.5f, 0);
      player.HoldingDown().Returns(false);

      state.OnFixedUpdate();

      AssertStateChange<Land>();
    }
    #endregion

    #region Integration Tests

    /// <summary>
    /// Tests if the player can perform a buffered jump (restarting the jump state loop when close enough to the ground)
    /// </summary>
    [Test]
    public void DJumpFall_Can_BufferedJump() {
      SetupTest();

      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.PressedJump().Returns(true);
      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(1);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }
    #endregion
  }
}