using System.Collections;
using NSubstitute;
using NUnit.Framework;
using Storm.Characters.Player;
using Storm.Services;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
  public class DoubleJumpRiseTests : StateTest<DoubleJumpRise> {

    #region Unit Tests
    [Test]
    public void DJumpRise_Can_WallRun() {
      SetupTest();

      player.IsTouchingLeftWall().Returns(true);
      
      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<DoubleJumpRise>(), Arg.Any<WallRun>());
    }

    [Test]
    public void DJumpRise_Can_Fall() {
      SetupTest();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsFalling().Returns(true);

      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<DoubleJumpRise>(), Arg.Any<DoubleJumpFall>());
    }
    #endregion

    #region Integration Tests
    [Test]
    public void DJump_Can_BufferedJump() {
      SetupTest();

      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      player.IsFalling().Returns(false);
      player.PressedJump().Returns(true);

      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(1);

      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<DoubleJumpRise>(), Arg.Any<SingleJumpStart>());
    }
    #endregion

  }
}