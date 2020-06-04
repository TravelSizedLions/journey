using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Characters;
using Storm.Services;

namespace Tests {

  public class HorizontalMotionTests {

    #region Fields
    private GameObject go;

    private HorizontalMotion state;

    private IPlayer player;

    private UnityPhysics physics;

    private MovementSettings settings;
    #endregion

    #region Setup
    private void SetupTest() {
      go = new GameObject();
      state = go.AddComponent<HorizontalMotion>();
      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<UnityPhysics>();
      physics.Awake();

      settings = go.AddComponent<MovementSettings>();

      state.Inject(player, physics);
    }
    #endregion

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
      player.Received().OnStateChange(Arg.Any<HorizontalMotion>(), Arg.Any<Jump1Start>());
    }

    public void HMotion_BufferedJump_GroundJump_Close() {
      SetupTest();

      settings.GroundJumpBuffer = 1;
      state.OnStateAdded();

      player.DistanceToGround().Returns(0.5f);
      player.DistanceToWall().Returns(0.51f);

      bool jumped = state.TryBufferedJump();

      Assert.AreEqual(true, jumped);
      player.Received().OnStateChange(Arg.Any<HorizontalMotion>(), Arg.Any<Jump1Start>());
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
      player.Received().OnStateChange(Arg.Any<HorizontalMotion>(), Arg.Any<WallJump>());
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
      player.Received().OnStateChange(Arg.Any<HorizontalMotion>(), Arg.Any<WallJump>());
    }
    #endregion

    #region Horizontal Motion Tests
    // [Test]
    // public void HMotion_Move_No_Input() {
    //   SetupTest();

    //   player.GetHorizontalInput().Returns(0);
    //   player.CanMove();

    // }

    #endregion
  }
}