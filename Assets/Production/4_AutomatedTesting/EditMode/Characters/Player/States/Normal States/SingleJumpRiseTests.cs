using NUnit.Framework;
using NSubstitute;

namespace HumanBuilders.Tests {
  public class SingleJumpRiseTests: PlayerStateTest<SingleJumpRise> {
    [Test]
    public void SJumpRise_Can_StartDoubleJump() {
      SetupTest();
      
      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.PressedJump().Returns(true);
      player.DistanceToGround().Returns(2);
      player.DistanceToWall().Returns(3);

      state.OnUpdate();
      
      AssertStateChange<DoubleJumpStart>();
    }

    [Test]
    public void SJumpRise_Update_NoOp() {
      SetupTest();
      
      player.PressedJump().Returns(false);
      
      state.OnUpdate();

      AssertNoStateChange<SingleJumpStart>();
      AssertNoStateChange<WallJump>();
      AssertNoStateChange<DoubleJumpStart>();
    }

    [Test]
    public void SJumpRise_Can_BufferedJump() {
      SetupTest();
      
      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.PressedJump().Returns(true);
      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(0.7f);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void SJumpRise_Can_Fall() {
      SetupTest();
      
      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(false);
      player.IsFalling().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<SingleJumpFall>();
    }

    [Test]
    public void SJumpRise_OnFixedUpdate_NoOp() {
      SetupTest();

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(false);
      player.IsFalling().Returns(false);
      player.IsTouchingLeftWall().Returns(false);
      player.IsTouchingRightWall().Returns(false);
      
      state.OnFixedUpdate();

      AssertNoStateChange<SingleJumpFall>();
      AssertNoStateChange<WallJump>();
    }

    [Test]
    public void SJumpRise_OnFixedUpdate_CanWallSlide_From_NoWallJump() {
      SetupTest();
      player.GetHorizontalInput().Returns(1);
      player.IsFalling().Returns(false);
      player.IsTouchingRightWall().Returns(true);
      player.IsWallJumping().Returns(false);

      state.OnFixedUpdate();

      AssertStateChange<WallSlide>();
    }

    [Test]
    public void SJumpRise_OnFixedUpdate_CanWallSlide_From_WallJump() {
      SetupTest();
      player.GetHorizontalInput().Returns(1);
      player.IsFalling().Returns(false);
      player.IsTouchingRightWall().Returns(true);
      player.IsWallJumping().Returns(true);

      state.OnFixedUpdate();

      AssertStateChange<WallSlide>();
    }
  }
}
